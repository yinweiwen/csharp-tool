using System.Collections.Generic;

namespace AiCableForce.calc
{
    public class Smooth
    {
        #region Linear
        public static IList<double> LinearSmooth3(IList<double> input)
        {
            var n = input.Count;
            var output = new double[n];
            int i;
            if (n < 3)
            {
                for (i = 0; i <= n - 1; i++)
                {
                    output[i] = input[i];
                }
            }
            else
            {
                output[0] = (5.0 * input[0] + 2.0 * input[1] - input[2]) / 6.0;

                for (i = 1; i <= n - 2; i++)
                {
                    output[i] = (input[i - 1] + input[i] + input[i + 1]) / 3.0;
                }

                output[n - 1] = (5.0 * input[n - 1] + 2.0 * input[n - 2] - input[n - 3]) / 6.0;
            }
            return output;
        }

        public static IList<double> LinearSmooth5(IList<double> input)
        {
            var n = input.Count;
            var output = new double[n];
            int i;
            if (n < 5)
            {
                for (i = 0; i <= n - 1; i++)
                {
                    output[i] = input[i];
                }
            }
            else
            {
                output[0] = (3.0 * input[0] + 2.0 * input[1] + input[2] - input[4]) / 5.0;
                output[1] = (4.0 * input[0] + 3.0 * input[1] + 2 * input[2] + input[3]) / 10.0;
                for (i = 2; i <= n - 3; i++)
                {
                    output[i] = (input[i - 2] + input[i - 1] + input[i] + input[i + 1] + input[i + 2]) / 5.0;
                }
                output[n - 2] = (4.0 * input[n - 1] + 3.0 * input[n - 2] + 2 * input[n - 3] + input[n - 4]) / 10.0;
                output[n - 1] = (3.0 * input[n - 1] + 2.0 * input[n - 2] + input[n - 3] - input[n - 5]) / 5.0;
            }
            return output;
        }

        public static IList<double> LinearSmooth7(IList<double> input)
        {
            var n = input.Count;
            var output = new double[n];
            int i;
            if (n < 7)
            {
                for (i = 0; i <= n - 1; i++)
                {
                    output[i] = input[i];
                }
            }
            else
            {
                output[0] = (13.0 * input[0] + 10.0 * input[1] + 7.0 * input[2] + 4.0 * input[3] +
                          input[4] - 2.0 * input[5] - 5.0 * input[6]) / 28.0;

                output[1] = (5.0 * input[0] + 4.0 * input[1] + 3 * input[2] + 2 * input[3] +
                          input[4] - input[6]) / 14.0;

                output[2] = (7.0 * input[0] + 6.0 * input[1] + 5.0 * input[2] + 4.0 * input[3] +
                          3.0 * input[4] + 2.0 * input[5] + input[6]) / 28.0;

                for (i = 3; i <= n - 4; i++)
                {
                    output[i] = (input[i - 3] + input[i - 2] + input[i - 1] + input[i] + input[i + 1] + input[i + 2] + input[i + 3]) / 7.0;
                }

                output[n - 3] = (7.0 * input[n - 1] + 6.0 * input[n - 2] + 5.0 * input[n - 3] +
                              4.0 * input[n - 4] + 3.0 * input[n - 5] + 2.0 * input[n - 6] + input[n - 7]) / 28.0;

                output[n - 2] = (5.0 * input[n - 1] + 4.0 * input[n - 2] + 3.0 * input[n - 3] +
                              2.0 * input[n - 4] + input[n - 5] - input[n - 7]) / 14.0;

                output[n - 1] = (13.0 * input[n - 1] + 10.0 * input[n - 2] + 7.0 * input[n - 3] +
                              4 * input[n - 4] + input[n - 5] - 2 * input[n - 6] - 5 * input[n - 7]) / 28.0;
            }
            return output;
        }
        #endregion

        #region Quadratic
        public static IList<double> QuadraticSmooth5(IList<double> input)
        {
            var n = input.Count;
            var output = new double[n];
            int i;
            if (n < 5)
            {
                for (i = 0; i <= n - 1; i++)
                {
                    output[i] = input[i];
                }
            }
            else
            {
                output[0] = (31.0 * input[0] + 9.0 * input[1] - 3.0 * input[2] - 5.0 * input[3] + 3.0 * input[4]) / 35.0;
                output[1] = (9.0 * input[0] + 13.0 * input[1] + 12 * input[2] + 6.0 * input[3] - 5.0 * input[4]) / 35.0;
                for (i = 2; i <= n - 3; i++)
                {
                    output[i] = (-3.0 * (input[i - 2] + input[i + 2]) +
                              12.0 * (input[i - 1] + input[i + 1]) + 17 * input[i]) / 35.0;
                }
                output[n - 2] = (9.0 * input[n - 1] + 13.0 * input[n - 2] + 12.0 * input[n - 3] + 6.0 * input[n - 4] - 5.0 * input[n - 5]) / 35.0;
                output[n - 1] = (31.0 * input[n - 1] + 9.0 * input[n - 2] - 3.0 * input[n - 3] - 5.0 * input[n - 4] + 3.0 * input[n - 5]) / 35.0;
            }
            return output;
        }


        public static IList<double> QuadraticSmooth7(IList<double> input)
        {
            var n = input.Count;
            var output = new double[n];
            int i;
            if (n < 7)
            {
                for (i = 0; i <= n - 1; i++)
                {
                    output[i] = input[i];
                }
            }
            else
            {
                output[0] = (32.0 * input[0] + 15.0 * input[1] + 3.0 * input[2] - 4.0 * input[3] -
                          6.0 * input[4] - 3.0 * input[5] + 5.0 * input[6]) / 42.0;

                output[1] = (5.0 * input[0] + 4.0 * input[1] + 3.0 * input[2] + 2.0 * input[3] +
                          input[4] - input[6]) / 14.0;

                output[2] = (1.0 * input[0] + 3.0 * input[1] + 4.0 * input[2] + 4.0 * input[3] +
                          3.0 * input[4] + 1.0 * input[5] - 2.0 * input[6]) / 14.0;
                for (i = 3; i <= n - 4; i++)
                {
                    output[i] = (-2.0 * (input[i - 3] + input[i + 3]) +
                               3.0 * (input[i - 2] + input[i + 2]) +
                              6.0 * (input[i - 1] + input[i + 1]) + 7.0 * input[i]) / 21.0;
                }
                output[n - 3] = (1.0 * input[n - 1] + 3.0 * input[n - 2] + 4.0 * input[n - 3] +
                              4.0 * input[n - 4] + 3.0 * input[n - 5] + 1.0 * input[n - 6] - 2.0 * input[n - 7]) / 14.0;

                output[n - 2] = (5.0 * input[n - 1] + 4.0 * input[n - 2] + 3.0 * input[n - 3] +
                              2.0 * input[n - 4] + input[n - 5] - input[n - 7]) / 14.0;

                output[n - 1] = (32.0 * input[n - 1] + 15.0 * input[n - 2] + 3.0 * input[n - 3] -
                              4.0 * input[n - 4] - 6.0 * input[n - 5] - 3.0 * input[n - 6] + 5.0 * input[n - 7]) / 42.0;
            }
            return output;
        }
        #endregion

        #region Cubic
        /**
         * 五点三次平滑
         *
         */
        public static IList<double> CubicSmooth5(IList<double> input)
        {
            var n = input.Count;
            var output = new double[n];
            int i;
            if (n < 5)
            {
                for (i = 0; i <= n - 1; i++)
                    output[i] = input[i];
            }

            else
            {
                output[0] = (69.0 * input[0] + 4.0 * input[1] - 6.0 * input[2] + 4.0 * input[3] - input[4]) / 70.0;
                output[1] = (2.0 * input[0] + 27.0 * input[1] + 12.0 * input[2] - 8.0 * input[3] + 2.0 * input[4]) / 35.0;
                for (i = 2; i <= n - 3; i++)
                {
                    output[i] = (-3.0 * (input[i - 2] + input[i + 2]) + 12.0 * (input[i - 1] + input[i + 1]) + 17.0 * input[i]) / 35.0;
                }
                output[n - 2] = (2.0 * input[n - 5] - 8.0 * input[n - 4] + 12.0 * input[n - 3] + 27.0 * input[n - 2] + 2.0 * input[n - 1]) / 35.0;
                output[n - 1] = (-input[n - 5] + 4.0 * input[n - 4] - 6.0 * input[n - 3] + 4.0 * input[n - 2] + 69.0 * input[n - 1]) / 70.0;
            }
            return output;
        }

        public static IList<double> CubicSmooth7(IList<double> input)
        {
            var n = input.Count;
            var output = new double[n];
            int i;
            if (n < 7)
            {
                for (i = 0; i <= n - 1; i++)
                {
                    output[i] = input[i];
                }
            }
            else
            {
                output[0] = (39.0 * input[0] + 8.0 * input[1] - 4.0 * input[2] - 4.0 * input[3] +
                          1.0 * input[4] + 4.0 * input[5] - 2.0 * input[6]) / 42.0;
                output[1] = (8.0 * input[0] + 19.0 * input[1] + 16.0 * input[2] + 6.0 * input[3] -
                          4.0 * input[4] - 7.0 * input[5] + 4.0 * input[6]) / 42.0;
                output[2] = (-4.0 * input[0] + 16.0 * input[1] + 19.0 * input[2] + 12.0 * input[3] +
                          2.0 * input[4] - 4.0 * input[5] + 1.0 * input[6]) / 42.0;
                for (i = 3; i <= n - 4; i++)
                {
                    output[i] = (-2.0 * (input[i - 3] + input[i + 3]) +
                               3.0 * (input[i - 2] + input[i + 2]) +
                              6.0 * (input[i - 1] + input[i + 1]) + 7.0 * input[i]) / 21.0;
                }
                output[n - 3] = (-4.0 * input[n - 1] + 16.0 * input[n - 2] + 19.0 * input[n - 3] +
                              12.0 * input[n - 4] + 2.0 * input[n - 5] - 4.0 * input[n - 6] + 1.0 * input[n - 7]) / 42.0;
                output[n - 2] = (8.0 * input[n - 1] + 19.0 * input[n - 2] + 16.0 * input[n - 3] +
                              6.0 * input[n - 4] - 4.0 * input[n - 5] - 7.0 * input[n - 6] + 4.0 * input[n - 7]) / 42.0;
                output[n - 1] = (39.0 * input[n - 1] + 8.0 * input[n - 2] - 4.0 * input[n - 3] -
                              4.0 * input[n - 4] + 1.0 * input[n - 5] + 4.0 * input[n - 6] - 2.0 * input[n - 7]) / 42.0;
            }
            return output;
        }
        #endregion
    }
}
