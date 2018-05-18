using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AiCableForce.calc;
using AiCableForce.serial;
using ZedGraph;
using NUnit.Framework;

namespace AiCableForce.graphic
{
    [TestFixture]
    public partial class Graphic : Form
    {
        private readonly WaveGraph _graph;

        public Graphic() { }

        public Graphic(string title = null, Control parent = null)
        {
            InitializeComponent();

            var zed = new ZedGraphControl
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(zed);
            _graph = new WaveGraph(zed, false)
            {
                ArrowColor = Color.Brown,
                MarkType = TMarkType.Triangle,
                PointSelectRule = 1
            };

            Parent = parent;
            if (!string.IsNullOrEmpty(title))
                Text = title;
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public void Show(double[] x, double[] y,TGraphic graphicType, double fs = 0, int analysePt = 0, string title = "", string legendname = "")
        {
            _graph.Ptls = new List<GraphData> { GraphData.FromSignal(x, y, fs, analysePt, graphicType, "", title, legendname) };
            _graph.Show();
            ShowDialog();
        }

        public void ShowOrg(double[] y, double fs)
        {
            var dt = 1 / fs;
            var t = new double[y.Length];
            for (var i = 0; i < y.Length; i++)
            {
                t[i] = i * dt;
            }
            Show(t, y, TGraphic.Original, fs);
        }

        public CableForceCalcResult ShowAll(VibData data, CableForceCalcParam cp, bool show)
        {
            var ptls = new List<GraphData>();
            var y = data.Odb;
            var fs = data.Sdb.diSampleFreq;
            var dt = 1 / fs;
            var t = new double[y.Length];
            for (var i = 0; i < y.Length; i++)
            {
                t[i] = i * dt;
            }
            ptls.Add(GraphData.FromSignal(t, y, fs, 0, TGraphic.Original, "", "original-data"));
            double[] f, yf;
            if (cp.UsePsd)
            {
                FourierTransform.Periodogram(y, fs, null, out yf, out f);
                if (cp.SmoothPower > 0)
                {
                    for (var i = 0; i < cp.SmoothPower; i++)
                    {
                        yf = Smooth.CubicSmooth5(yf).ToArray();
                    }
                }
                ptls.Add(GraphData.FromSignal(f, yf, fs, 0, TGraphic.FreqSpectrumPSD, "", "power spectral density"));
            }
            else
            {
                FourierTransform.Forward(y, fs, null, out yf, out f);
                yf[0] = 0;
                if (cp.SmoothPower > 0)
                {
                    for (var i = 0; i < cp.SmoothPower; i++)
                    {
                        yf = Smooth.CubicSmooth5(yf).ToArray();
                    }
                }
                ptls.Add(GraphData.FromSignal(f, yf, fs, 0, TGraphic.FreqSpectrum, "", "Frequency Spectrum"));
            }


            double[] yfn;
            FourierTransform.SelfPower(yf, cp.SelfPower, out yfn);
            ptls.Add(cp.UsePsd
                ? GraphData.FromSignal(f, yfn, fs, 0, TGraphic.FreqSpectrumPSD, "", string.Format("power spectral density {0}'（单组谱倍增）", cp.SelfPower))
                : GraphData.FromSignal(f, yfn, fs, 0, TGraphic.FreqSpectrum, "", string.Format("Frequency Spectrum {0}'（单组频谱倍增）", cp.SelfPower)));
            _graph.Ptls = ptls;
            _graph.Show();
            if (show) Show();
            return CalcCable(yfn, f, cp);
        }

        public CableForceCalcResult ShowMulti(VibData data1, VibData data2, CableForceCalcParam cp, bool show)
        {
            var ptls = new List<GraphData>();
            var y1 = data1.Odb;
            var y2 = data2.Odb;
            var fs = data1.Sdb.diSampleFreq;
            double[] f, yf1, yf2;
            if (cp.UsePsd)
            {
                FourierTransform.Periodogram(y1, fs, null, out yf1, out f);
                ptls.Add(GraphData.FromSignal(f, yf1, fs, 0, TGraphic.FreqSpectrumPSD, "", "power spectral density"));
                FourierTransform.Periodogram(y2, fs, null, out yf2, out f);
                ptls.Add(GraphData.FromSignal(f, yf2, fs, 0, TGraphic.FreqSpectrumPSD, "", "power spectral density"));
            }
            else
            {
                FourierTransform.Forward(y1, fs, null, out yf1, out f);
                if (cp.SmoothPower > 0)
                {
                    for (var i = 0; i < cp.SmoothPower; i++)
                    {
                        yf1 = Smooth.CubicSmooth5(yf1).ToArray();
                    }
                }
                ptls.Add(GraphData.FromSignal(f, yf1, fs, 0, TGraphic.FreqSpectrum, "", "Frequency Spectrum"));
                FourierTransform.Forward(y2, fs, null, out yf2, out f);
                if (cp.SmoothPower > 0)
                {
                    for (var i = 0; i < cp.SmoothPower; i++)
                    {
                        yf1 = Smooth.CubicSmooth5(yf1).ToArray();
                    }
                }
                ptls.Add(GraphData.FromSignal(f, yf2, fs, 0, TGraphic.FreqSpectrum, "", "Frequency Spectrum"));
            }

            double[] yfm;
            FourierTransform.MultiPower(yf1, yf2, out yfm);
            ptls.Add(cp.UsePsd
                ? GraphData.FromSignal(f, yfm, fs, 0, TGraphic.FreqSpectrumPSD, "", "power spectral density（两组谱倍增）")
                : GraphData.FromSignal(f, yfm, fs, 0, TGraphic.FreqSpectrum, "", "Frequency Spectrum（两组谱倍增）"));
            _graph.Ptls = ptls;
            _graph.Show();
            if (show) Show();
            return CalcCable(yfm, f, cp);
        }

        public static CableForceCalcResult CalcCable(double[] y, double[] f, CableForceCalcParam cp)
        {
            y[0] = 0;
            var df = f[1] - f[0];
            double ymax, lp, rp;
            int ymaxi, lpi, rpi;
            FindMax(y, 0, y.Length - 1, out ymax, out ymaxi);
            if (Math.Abs(cp.FreqTolerance) < 1e-3 || cp.FreqTolerance > cp.EstimateFreq)
                throw new Exception("frequency tolerance set err.");
            var gapmin = (int)((cp.EstimateFreq - cp.FreqTolerance) / df);
            var gapmax = (int)((cp.EstimateFreq + cp.FreqTolerance) / df) + 1;
            var lfl = ymaxi - gapmax;
            var lfr = ymaxi - gapmin;
            var rfl = ymaxi + gapmin;
            var rfr = ymaxi + gapmax;
            if (lfl < 0) lpi = -1;
            else FindPeak(y, lfr, lfl, ymax * cp.PeakThreshod, out lp, out lpi);
            if (rfr > y.Length - 1) rpi = -1;
            else FindPeak(y, rfl, rfr, ymax * cp.PeakThreshod, out rp, out rpi);
            if (lpi == -1 && rpi == -1) throw new Exception("no peak found around MAX.");
            var gapi = lpi == -1 ? rpi - ymaxi : (rpi == -1 ? ymaxi - lpi : Math.Min(rpi - ymaxi, ymaxi - lpi));
            var masterfre = gapi * df;
            return new CableForceCalcResult
            {
                MasterFreq = masterfre,
                Force = 4 * cp.UnitWeight * Math.Pow(cp.CableLength, 2) * Math.Pow(masterfre, 2) / 1000
            };
        }

        public static void FindMax(double[] a, int start, int end, out double max, out int index)
        {
            max = a[start];
            index = start;
            for (var i = start + 1; i <= end; i++)
            {
                if (a[i] > max)
                {
                    max = a[i];
                    index = i;
                }
            }
        }

        public static void FindPeak(double[] a, int start, int end, double? threshold, out double peak, out int peakindex)
        {
            peak = 0;
            peakindex = -1;
            var interval = Math.Sign(end - start);
            for (var i = start + interval; i != end; i += interval)
            {
                if (a[i] > a[i - interval] && a[i] >= a[i + interval] && a[i] > (threshold ?? 0))
                {
                    peak = a[i];
                    peakindex = i;
                    break;
                }
            }
        }

        [Test]
        public void TestFindPeak()
        {
            var dl = new double[] { 3, 2, 1, 5, 5, 2, 1, 3, 5 };
            double p;
            int pi;
            FindPeak(dl, 0, dl.Length - 1, null, out p, out pi);
            Assert.AreEqual(p, 5);
            Assert.AreEqual(pi, 3);

            FindPeak(dl, dl.Length - 1, 0, null, out p, out pi);
            Assert.AreEqual(p, 5);
            Assert.AreEqual(pi, 4);

            dl = new double[] { 3, 2, 1, 3, 2, 4, 1, 3, 5 };
            FindPeak(dl, 0, dl.Length - 1, null, out p, out pi);
            Assert.AreEqual(p, 3);
            Assert.AreEqual(pi, 3);
            FindPeak(dl, 3, dl.Length - 1, null, out p, out pi);
            Assert.AreEqual(p, 4);
            Assert.AreEqual(pi, 5);
            FindPeak(dl, 0, dl.Length - 1, 3.1, out p, out pi);
            Assert.AreEqual(p, 4);
            Assert.AreEqual(pi, 5);
            FindPeak(dl, 0, 3, 0, out p, out pi);
            Assert.AreEqual(p, 0);
            Assert.AreEqual(pi, -1);
        }

        [Test]
        public void TestFindMax()
        {
            var dl = new double[] { 1, 2, 3, 3, 2, 1, 6, 2, 1, 0 };
            double m;
            int mi;
            FindMax(dl, 0, dl.Length - 1, out m, out mi);
            Assert.AreEqual(6, m);
            Assert.AreEqual(6, mi);

            FindMax(dl, 0, 4, out m, out mi);
            Assert.AreEqual(3, m);
            Assert.AreEqual(2, mi);

            FindMax(dl, 7, dl.Length - 1, out m, out mi);
            Assert.AreEqual(2, m);
            Assert.AreEqual(7, mi);
        }
    }
}
