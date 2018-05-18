using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AiCableForce.calc;
using AiCableForce.serial;

namespace AiCableForce.noise
{
    /// <summary>
    ///  二次噪声计算类
    /// </summary>
    public class SecondaryNoise
    {
        private List<OctaveBand> _bands = new List<OctaveBand>();

        // 基准加速度 m/s^2
        // 也有用 1e-5
        // ReSharper disable once InconsistentNaming
        private const double a0 = 1e-6;

        public SecondaryNoise()
        {
            LoadStandard();
        }

        private void LoadStandard()
        {
            /**
             * 参考
             *  https://wenku.baidu.com/view/c797bad1b14e852458fb574f.html
             *  以及
             *  <JGJT 170-2009 城市轨道交通引起建筑物振动与二次辐射噪声限值及其测量方法标准>
             */
            _bands = new List<OctaveBand>()
            {
                new OctaveBand(4,3.55,4.47),
                new OctaveBand(5,4.47,5.62),
                new OctaveBand(6.3,5.62,7.08),
                new OctaveBand(8,7.08,8.91),
                new OctaveBand(10,8.91,11.2),
                new OctaveBand(12.5,11.2,14.1,-1),
                new OctaveBand(16,14.1,17.8,-2),
                new OctaveBand(20,17.8,22.4,-4),
                new OctaveBand(25,22.4,28.2,-6),
                new OctaveBand(31.5,28.2,35.5,-8),
                new OctaveBand(40,35.5,44.7,-10),
                new OctaveBand(50,44.7,56.2,-12),
                new OctaveBand(63,56.2,70.8,-14),
                new OctaveBand(80,70.8,89.1,-17),
                new OctaveBand(100,89.1,112,-21),
                new OctaveBand(125,112,141,-25),
                new OctaveBand(160,141,178,-30),
                new OctaveBand(200,178,224,-36)
            };
        }

        public double Calc(VibData data)
        {
            var fs = (int)data.Sdb.diSampleFreq;
            var n = data.Odb.Length / fs;
            var lvzs = new double[n];
            for (var k = 0; k < n; k++)
            {
                var ak = new double[fs];
                Array.Copy(data.Odb, k * fs, ak, 0, fs);
                double[] f, y;
                FourierTransform.Forward(ak, fs, null, out y, out f);
                double sum = 0;
                foreach (var octaveBand in _bands)
                {
                    octaveBand.CalcRms(y, f[1]);
                    sum += octaveBand.Rms * octaveBand.Rms * octaveBand.ZWeigth * octaveBand.ZWeigth;
                }
                var aw = Math.Sqrt(sum);
                lvzs[k] = 20 * Math.Log10(aw / a0);
            }
            return lvzs.Max();
        }
    }

    class OctaveBand
    {
        private double _center;
        private readonly double _low;
        private readonly double _up;

        // Z计权因子
        private double _zw;

        public double ZWeigth
        {
            get { return _zw; }
            private set
            {
                _zw = Math.Pow(10, value / 20); //dB->Value
            }
        }

        public double Rms;

        public OctaveBand(double c, double l, double u, double z = 0)
        {
            _center = c;
            _low = l;
            _up = u;
            ZWeigth = z;
        }

        // awi
        public void CalcRms(double[] y, double df)
        {
            var idx1 = (int)(_low / df); // 不包含
            var idx2 = (int)(_up / df);
            if (idx1 >= (y.Length - 1))
            {
                Rms = 0;
                return;
            }
            if (idx2 >= y.Length)
                idx2 = y.Length - 1;
            var n = idx2 - idx1;
            if (n <= 0)
            {
                Rms = 0;
                return;
            }
            double sum = 0;
            for (var i = idx1 + 1; i <= idx2; i++)
            {
                sum += y[i] * y[i];
            }
            Rms = Math.Sqrt(sum / n);
        }
    }
}
