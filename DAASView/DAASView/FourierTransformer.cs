using System;
using System.Diagnostics;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace DAASView
{
    public class FourierTransformer
    {
        private static void Bitrp(double[] xreal, double[] ximag, int n)
        {
            // 位反转置换 Bit-reversal Permutation
            int i;
            int p;
            for (i = 1, p = 0; i < n; i *= 2)
            {
                p++;
            }
            for (i = 0; i < n; i++)
            {
                var a = i;
                var b = 0;
                int j;
                for (j = 0; j < p; j++)
                {
                    b = b * 2 + a % 2;
                    a = a / 2;
                }
                if (b > i)
                {
                    var t = xreal[i];
                    xreal[i] = xreal[b];
                    xreal[b] = t;
                    t = ximag[i];
                    ximag[i] = ximag[b];
                    ximag[b] = t;
                }
            }
        }

        private static int FFT(double[] xreal, double[] ximag)
        {
            //n值为2的N次方
            var n = 2;
            while (n <= xreal.Length)
            {
                n *= 2;
            }
            n /= 2;
            // 快速傅立叶变换，将复数 x 变换后仍保存在 x 中，xreal, ximag 分别是 x 的实部和虚部
            var wreal = new double[n / 2];
            var wimag = new double[n / 2];
            int m;
            int j;
            Bitrp(xreal, ximag, n);
            // 计算 1 的前 n / 2 个 n 次方根的共轭复数 W'j = wreal [j] + i * wimag [j] , j = 0, 1, ... , n / 2 - 1
            var arg = -2 * Math.PI / n;
            var treal = Math.Cos(arg);
            var timag = Math.Sin(arg);
            wreal[0] = 1.0f;
            wimag[0] = 0.0f;
            for (j = 1; j < n / 2; j++)
            {
                wreal[j] = wreal[j - 1] * treal - wimag[j - 1] * timag;
                wimag[j] = wreal[j - 1] * timag + wimag[j - 1] * treal;
            }
            for (m = 2; m <= n; m *= 2)
            {
                int k;
                for (k = 0; k < n; k += m)
                {
                    for (j = 0; j < m / 2; j++)
                    {
                        var index1 = k + j;
                        var index2 = index1 + m / 2;
                        var t = n * j / m;
                        treal = wreal[t] * xreal[index2] - wimag[t] * ximag[index2];
                        timag = wreal[t] * ximag[index2] + wimag[t] * xreal[index2];
                        var ureal = xreal[index1];
                        var uimag = ximag[index1];
                        xreal[index1] = ureal + treal;
                        ximag[index1] = uimag + timag;
                        xreal[index2] = ureal - treal;
                        ximag[index2] = uimag - timag;
                    }
                }
            }
            return n;
        }

        private static int Ifft(double[] xreal, double[] ximag)
        {
            //n值为2的N次方
            var n = 2;
            while (n <= xreal.Length)
            {
                n *= 2;
            }
            n /= 2;
            // 快速傅立叶逆变换
            var wreal = new double[n / 2];
            var wimag = new double[n / 2];
            double treal, timag, ureal, uimag, arg;
            int m, k, j, t, index1, index2;
            Bitrp(xreal, ximag, n);
            // 计算 1 的前 n / 2 个 n 次方根 Wj = wreal [j] + i * wimag [j] , j = 0, 1, ... , n / 2 - 1
            arg = 2 * Math.PI / n;
            treal = Math.Cos(arg);
            timag = Math.Sin(arg);
            wreal[0] = 1.0f;
            wimag[0] = 0.0f;
            for (j = 1; j < n / 2; j++)
            {
                wreal[j] = wreal[j - 1] * treal - wimag[j - 1] * timag;
                wimag[j] = wreal[j - 1] * timag + wimag[j - 1] * treal;
            }
            for (m = 2; m <= n; m *= 2)
            {
                for (k = 0; k < n; k += m)
                {
                    for (j = 0; j < m / 2; j++)
                    {
                        index1 = k + j;
                        index2 = index1 + m / 2;
                        t = n * j / m;    // 旋转因子 w 的实部在 wreal [] 中的下标为 t
                        treal = wreal[t] * xreal[index2] - wimag[t] * ximag[index2];
                        timag = wreal[t] * ximag[index2] + wimag[t] * xreal[index2];
                        ureal = xreal[index1];
                        uimag = ximag[index1];
                        xreal[index1] = ureal + treal;
                        ximag[index1] = uimag + timag;
                        xreal[index2] = ureal - treal;
                        ximag[index2] = uimag - timag;
                    }
                }
            }
            for (j = 0; j < n; j++)
            {
                xreal[j] /= n;
                ximag[j] /= n;
            }
            return n;
        }

        private static int Radix2(int v)
        {
            var res = 1;
            while (res < v)
                res <<= 1;
            return res;
        }

        /// <summary>
        /// 对于时域a[]转换为频率Y(f),参数b为0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int FftData(double[] a, double[] b, double fs, out double[] y, out double[] f)
        {
            var l = a.Length;//采样个数
            var nfft = Radix2(l);//最靠近大于2的幂数NFFT，有点问题
            y = new double[nfft / 2 + 1];
            f = new double[nfft / 2 + 1];
            var ax = new double[nfft];
            var bx = new double[nfft];
            Array.Copy(a, 0, ax, 0, l);
            if (b != null)
            {
                Array.Copy(b, 0, bx, 0, l);
            }
            FFT(ax, bx);

            for (var i = 0; i < nfft / 2 + 1; i++)
            {
                var j = (double)i / (nfft / 2);
                f[i] = fs / 2 * j;
            }

            for (var i = 0; i < nfft / 2 + 1; i++)
            {
                y[i] = 2 * Math.Sqrt(Math.Pow(ax[i], 2) + Math.Pow(bx[i], 2)) / l;
            }
            return nfft / 2 + 1;
        }

        /// <summary>
        /// 对于时域a[]转换为频率Y(f),参数b为0
        /// 数据总量可大于每次FFT计算的nCount，多次FFT计算求平均
        /// </summary>
        public static bool FftDataFragment(double[] a, double fs, int nCount, out double[] y, out double[] f)
        {
            var len = a.Length;
            var totalCnt = 0;
            var index = 0;
            var tmp = new double[nCount];
            y = new double[nCount / 2 + 1];
            while (index < len)
            {
                var n = 0;
                for (var i = index; i < index + nCount; i++)
                {
                    if (i < len)
                    {
                        tmp[n] = a[i];
                    }
                    else
                    {
                        tmp[n] = 0f;
                    }
                    n++;
                }

                var b = new double[nCount];
                FFT(tmp, b);

                //直流分量
                y[0] += Math.Sqrt(Math.Pow(tmp[0], 2) + Math.Pow(b[0], 2)) / nCount;
                for (var i = 1; i < nCount / 2 + 1; i++)
                {
                    y[i] += 2 * Math.Sqrt(Math.Pow(tmp[i], 2) + Math.Pow(b[i], 2)) / nCount;
                }
                index += nCount;
                totalCnt++;
            }

            for (var i = 0; i < nCount / 2 + 1; i++)
            {
                y[i] = y[i] / totalCnt;
            }

            f = new double[nCount / 2 + 1];
            for (var i = 0; i < nCount / 2 + 1; i++)
            {
                var j = (double)i / (nCount / 2);
                f[i] = fs / 2 * j;
            }
            return true;
        }

        /// <summary>
        /// Applies the forward Fast Fourier Transform (FFT) to arbitrary-length sample vectors.
        /// </summary>
        public static void Fft(double[] a, double fs, out double[] y, out double[] f)
        {
            var len = a.Length;
            var samples = new Complex[len];
            for (var i = 0; i < len; i++)
            {
                samples[i] = new Complex(a[i], 0);
            }
            Fourier.Forward(samples, FourierOptions.Matlab);
            var n = len / 2;
            var df = fs / len;
            y = new double[n];
            f = new double[n];
            for (var i = 0; i < n; i++)
            {
                f[i] = i * df;
                y[i] = samples[i].Magnitude * 2 / len;
            }
        }

        /// <summary>
        /// anapts - 分析点数(null:全程分析)
        /// </summary>
        public static void Forward(double[] a, double fs, int? anapts, out double[] y, out double[] f)
        {
            var len = a.Length;
            var pts = anapts ?? Radix2(len);
            var samples = new Complex[pts];
            for (var i = 0; i < Math.Min(len, pts); i++)
            {
                samples[i] = new Complex(a[i], 0);
            }
            Fourier.Forward(samples, FourierOptions.Matlab);
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
            var pts = anapts ?? Radix2(len);
            var samples = new Complex[pts];
            for (var i = 0; i < Math.Min(len, pts); i++)
            {
                samples[i] = new Complex(a[i], 0);
            }
            Fourier.Forward(samples, FourierOptions.Matlab);
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
    }
}
