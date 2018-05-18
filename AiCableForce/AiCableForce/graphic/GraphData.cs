using System;
using System.Collections.Generic;
using ZedGraph;

namespace AiCableForce.graphic
{
    public class GraphData
    {
        public Guid id; // find corresponding pane base on this
        public string title;
        public string legend_fix;
        public string xname;
        public string xunit;
        public string yname;
        public string yunit;
        public IEnumerable<PointPairList> ptls;

        public Mark mark = new Mark();
        public List<LineItem> lines = new List<LineItem>();
        public Range range = new Range();
        public GraphPane pane;

        public GraphData()
        {
            id = Guid.NewGuid();
        }

        public static GraphData FromSignal(double[] xdatum, double[] datum, double fs, int analysePt, 
            TGraphic GraphicType = TGraphic.Original, string EU = "", string title = "", string tpname = "")
        {
            if (string.IsNullOrEmpty(EU)) EU = "mv";

            var graphData = new GraphData();
            var pts = new PointPairList(xdatum, datum);
            graphData.ptls = new[] { pts };
            graphData.legend_fix = (GraphicType == TGraphic.Original) ?
                string.Format(@"{0}
FS={1} Hz
TP={2}
DT={3:0.####} ms
EU={4}", tpname, fs, datum.Length, 1000 / fs, EU) :
                string.Format(@"{0}
FS={1} Hz
DF={2:0.####} Hz
AP={3}", tpname, fs, fs / analysePt, analysePt);
            switch (GraphicType)
            {
                case TGraphic.Original:
                    graphData.xname = "T";
                    graphData.xunit = "s";
                    graphData.yname = "A";
                    graphData.yunit = EU;
                    break;
                case TGraphic.FreqSpectrum:
                    graphData.xname = "F";
                    graphData.xunit = "Hz";
                    graphData.yname = "A";
                    graphData.yunit = EU;
                    break;
                case TGraphic.TransFuncAmp:
                    graphData.xname = "F";
                    graphData.xunit = "Hz";
                    graphData.yname = "A";
                    graphData.yunit = EU;
                    graphData.title = "幅频曲线图";
                    break;
                case TGraphic.TransFuncPhase:
                    graphData.xname = "F";
                    graphData.xunit = "Hz";
                    graphData.yname = "PH";
                    graphData.yunit = "度";
                    graphData.title = "相频曲线图";
                    break;
                case TGraphic.TransFuncReal:
                    graphData.xname = "F";
                    graphData.xunit = "Hz";
                    graphData.yname = "Cp_r";
                    graphData.yunit = EU + "/" + EU;
                    graphData.title = "实部曲线图";
                    break;
                case TGraphic.TransFuncImag:
                    graphData.xname = "F";
                    graphData.xunit = "Hz";
                    graphData.yname = "Cp_i";
                    graphData.yunit = EU + "/" + EU;
                    graphData.title = "虚部曲线图";
                    break;
                case TGraphic.TransFuncNyq:
                    graphData.xname = "Cp_r";
                    graphData.xunit = EU + "/" + EU;
                    graphData.yname = "Cp_i";
                    graphData.yunit = EU + "/" + EU;
                    graphData.title = "奈奎斯特图";
                    break;
                case TGraphic.TransFuncCoheAmp:
                    graphData.xname = "F";
                    graphData.xunit = "Hz";
                    graphData.yname = "A";
                    graphData.yunit = EU + "/" + EU;
                    graphData.title = "相干幅频曲线图";
                    break;
                case TGraphic.TransFuncCohe:
                    graphData.xname = "F";
                    graphData.xunit = "Hz";
                    graphData.yname = "Co";
                    graphData.yunit = "";
                    graphData.title = "相干曲线图";
                    break;
                case TGraphic.FreqSpectrumPSD:
                    graphData.xname = "F";
                    graphData.xunit = "Hz";
                    graphData.yname = "P";
                    graphData.yunit = "dB";
                    graphData.title = "功率谱密度";
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(title))
                graphData.title = title;
            return graphData;
        }
    }

    public class Mark
    {
        public ArrowObj arrow;
        public int index = 0;
    }

    public enum TGraphLayout
    {
        T_COL,
        T_2COL,
        T_SQUARE,
        T_211
    }

    public enum TMarkType
    {
        Line,
        Triangle
    }
}