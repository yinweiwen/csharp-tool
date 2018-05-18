using System;
using System.Diagnostics;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace AiCableForce.calc
{
    public class FourierTransform
    {
        /// <summary>
        /// anapts - 分析点数(null:全程分析)
        /// </summary>
        public static void Forward(double[] a, double fs, int? anapts, out double[] y, out double[] f)
        {
            var len = a.Length;
            var pts = anapts ?? PowerTwo(len);
            var samples = new Complex[pts];
            for (var i = 0; i < Math.Min(len, pts); i++)
            {
                samples[i] = new Complex(a[i], 0);
            }
            Fourier.Forward((Complex[])samples, FourierOptions.Matlab);
            var n = pts / 2;
            var df = fs / pts;
            y = new double[n];
            f = new double[n];
            for (var i = 0; i < n; i++)
            {
                f[i] = i * df;
                y[i] = samples[i].Magnitude * 2 / len;
            }
        }

        /// <summary>
        /// 直接谱估计(周期图法)
        /// P/dB
        /// </summary>
        public static void Periodogram(double[] a, double fs, int? anapts, out double[] y, out double[] f)
        {
            var len = a.Length;
            var pts = anapts ?? PowerTwo(len);
            var samples = new Complex[pts];
            for (var i = 0; i < Math.Min(len, pts); i++)
            {
                samples[i] = new Complex(a[i], 0);
            }
            Fourier.Forward((Complex[])samples, FourierOptions.Matlab);
            var n = pts / 2;
            var df = fs / pts;
            y = new double[n];
            f = new double[n];
            for (var i = 0; i < n; i++)
            {
                f[i] = i * df;
                y[i] = Math.Log10(Math.Pow(samples[i].Magnitude, 2) / len) * 10;
            }
        }

        /// <summary>
        /// 单组频率倍增
        /// </summary>
        public static void SelfPower(double[] a, int pw, out double[] y)
        {
            Debug.Assert(a != null && a.Length != 0);
            y = new double[a.Length];
            for (var i = 0; i < a.Length; i++)
            {
                y[i] = Math.Pow(a[i], pw);
            }
        }

        /// <summary>
        /// 多组频率倍增
        /// </summary>
        public static void MultiPower(double[] a, double[] b, out double[] y)
        {
            Debug.Assert(a != null && b != null && a.Length == b.Length);
            y = new double[a.Length];
            for (var i = 0; i < a.Length; i++)
            {
                y[i] = a[i] * b[i];
            }
        }

        public static int PowerTwo(int v)
        {
            var res = 1;
            while (res < v)
                res <<= 1;
            return res;
        }
    }
}
