using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMLab4
{
    internal class Solver
    {
        private static Func<double, double> func1 = (x) => x + Math.Sin(x);
        private static Func<double, double> func2 = (x) => 5 * Math.Cos(3 * x) / (1 + Math.Pow(x, 2));
        private static double[] bitcoinDynamics = { 44717.80, 43927.43, 42560.46, 42679.30, 42323.69, 
                                                    42247.24, 42102.79, 44182.90, 41084.34, 40114.91,
                                                    40076.94, 38362.96, 38327.56, 37880.40, 37655.01,
                                                    38209.00, 38914.51, 39364.51, 37977.11, 41742.97 };

        private static Func<double, double> func3 = (x) => bitcoinDynamics[(int)x];
        private static Func<double, double>[] funcs = { func1, func2, func3 };

        private static double[][] linear = new double[2][];
        private static double[][] exponential = new double[2][];
        private static double[][] logarithmic = new double[2][];
        private static double[][] polinomical = new double[2][];


        private static double MakeValueNoise(double value)
        {
            Random random = new Random();
            return value + ((double)random.NextDouble()) / 2;
        }

        public static Point[] GetData(int funcNum)
        {
            Point[] data = new Point[20];

            for (int i = 0; i < 20; i++)
            {
                double x;
                double y;
                if (funcNum != 2)
                {
                    x = MakeValueNoise(i);
                    y = funcs[funcNum](x);
                }

                else
                {
                    x = i + 1;
                    y = funcs[funcNum](x - 1);
                }  

                data[i] = new Point(x, y);
            }

            return data;
        }

        public static int FindMaxDeviate(Point[] data, Point[] appData)
        {
            double sqrt = Int32.MinValue;
            int index = 0;

            for (int i = 0; i < data.Length; i++)
            {
                double current = Math.Pow(data[i].y - appData[i].y, 2);
                if (current > sqrt)
                {
                    sqrt = current;
                    index = i;
                }
            }
            return index;
        }

        private static double[] GetDets2(double[][] sysMatrix)
        {
            double mainDet = sysMatrix[0][0] * sysMatrix[1][1] - sysMatrix[0][1] * sysMatrix[1][0];
            double aDet = sysMatrix[0][2] * sysMatrix[1][1] - sysMatrix[0][1] * sysMatrix[1][2];
            double bDet = sysMatrix[0][0] * sysMatrix[1][2] - sysMatrix[0][2] * sysMatrix[1][0];

            double aCoef = aDet / mainDet;
            double bCoef = bDet / mainDet;

            return new double[] { aCoef, bCoef };
        }

        private static double[] GetDets3(double[][] sysMatrix)
        {
            double mainDet = sysMatrix[0][0] * sysMatrix[1][1] * sysMatrix[2][2] +
                             sysMatrix[0][2] * sysMatrix[1][0] * sysMatrix[2][1] +
                             sysMatrix[2][0] * sysMatrix[0][1] * sysMatrix[1][2] -

                             sysMatrix[2][0] * sysMatrix[1][1] * sysMatrix[0][2] -
                             sysMatrix[0][0] * sysMatrix[1][2] * sysMatrix[2][1] -
                             sysMatrix[2][2] * sysMatrix[0][1] * sysMatrix[1][0];

            double aDet =    sysMatrix[0][3] * sysMatrix[1][1] * sysMatrix[2][2] +
                             sysMatrix[0][2] * sysMatrix[1][3] * sysMatrix[2][1] +
                             sysMatrix[2][3] * sysMatrix[0][1] * sysMatrix[1][2] -

                             sysMatrix[2][3] * sysMatrix[1][1] * sysMatrix[0][2] -
                             sysMatrix[0][3] * sysMatrix[1][2] * sysMatrix[2][1] -
                             sysMatrix[2][2] * sysMatrix[0][1] * sysMatrix[1][3];

            double bDet =    sysMatrix[0][0] * sysMatrix[1][3] * sysMatrix[2][2] +
                             sysMatrix[0][2] * sysMatrix[1][0] * sysMatrix[2][3] +
                             sysMatrix[2][0] * sysMatrix[0][3] * sysMatrix[1][2] -

                             sysMatrix[2][0] * sysMatrix[1][3] * sysMatrix[0][2] -
                             sysMatrix[0][0] * sysMatrix[1][2] * sysMatrix[2][3] -
                             sysMatrix[2][2] * sysMatrix[0][3] * sysMatrix[1][0];

            double cDet =    sysMatrix[0][0] * sysMatrix[1][1] * sysMatrix[2][3] +
                             sysMatrix[0][3] * sysMatrix[1][0] * sysMatrix[2][1] +
                             sysMatrix[2][0] * sysMatrix[0][1] * sysMatrix[1][3] -
                                
                             sysMatrix[2][0] * sysMatrix[1][1] * sysMatrix[0][3] -
                             sysMatrix[0][0] * sysMatrix[1][3] * sysMatrix[2][1] -
                             sysMatrix[2][3] * sysMatrix[0][1] * sysMatrix[1][0];

            return new double[] {aDet / mainDet, bDet / mainDet, cDet / mainDet};
        }

        public static Point[] ApproximateLinear(Point[] data, int approxNum)
        {
            Point[] points = new Point[data.Length];

            double xSqrtSum = 0; Array.ForEach(data, (x) => xSqrtSum += Math.Pow(x.x, 2));
            double xSum = 0; Array.ForEach(data, (x) => xSum += x.x);
            double multyXY = 0; Array.ForEach(data, (x) => multyXY += x.x * x.y);
            double ySum = 0; Array.ForEach(data, (x) => ySum += x.y);

            double n = data.Length;

            double[][] sysMatrix = new double[2][];
            sysMatrix[0] = new double[] { xSqrtSum, xSum, multyXY };
            sysMatrix[1] = new double[] { xSum, n, ySum };

            double[] coefs = GetDets2(sysMatrix);

            double aCoef = coefs[0]; double bCoef = coefs[1];

            linear[approxNum] = new double[2];
            linear[approxNum][0] = aCoef; linear[approxNum][1] = bCoef;

            for (int i = 0; i < data.Length; i++)
            {
                double y = coefs[0] * data[i].x + coefs[1];
                points[i] = new Point(data[i].x, y);
            }

            return points;
        }

        public static Point[] ApproximateLogarithmic(Point[] data, int approxNum)
        {
            Point[] points = new Point[data.Length];

            double xSLnSum = 0; Array.ForEach(data, (x) => xSLnSum += Math.Pow(Math.Log(x.x), 2));
            double xLnSum = 0; Array.ForEach(data, (x) => xLnSum += Math.Log(x.x));
            double multyLnXY = 0; Array.ForEach(data, (x) => multyLnXY += x.y * Math.Log(x.x));
            double ySum = 0; Array.ForEach(data, (x) => ySum += x.y);

            double n = data.Length;

            double[][] sysMatrix = new double[2][];
            sysMatrix[0] = new double[] { xSLnSum, xLnSum, multyLnXY };
            sysMatrix[1] = new double[] { xLnSum, n, ySum };

            double[] coefs = GetDets2(sysMatrix);

            double aCoef = coefs[0]; double bCoef = coefs[1];

            logarithmic[approxNum] = new double[2];
            logarithmic[approxNum][0] = aCoef; logarithmic[approxNum][1] = bCoef;

            for (int i = 0; i < data.Length; i++)
            {
                double y = coefs[0] * Math.Log(data[i].x) + coefs[1];
                points[i] = new Point(data[i].x, y);
            }

            return points;
        }

        public static Point[] ApproximateExponential(Point[] data, int approxNum)
        {
            Point[] points = new Point[data.Length];

            double xSqrtSum = 0; Array.ForEach(data, (x) => xSqrtSum += Math.Pow(x.x, 2));
            double xSum = 0; Array.ForEach(data, (x) => xSum += x.x);
            double multyXLnY = 0; Array.ForEach(data, (x) => {
                if (x.y > 0)
                    multyXLnY += x.x * Math.Log(x.y);
            });
            double lnYSum = 0; Array.ForEach(data, (x) => {
                if (x.y > 0)
                    lnYSum += Math.Log(x.y);
            });

            double n = data.Length;

            double[][] sysMatrix = new double[2][];
            sysMatrix[0] = new double[] { xSqrtSum, xSum, multyXLnY };
            sysMatrix[1] = new double[] { xSum, n, lnYSum };

            double[] coefs = GetDets2(sysMatrix);

            double aCoef = coefs[0]; double bCoef = Math.Pow(Math.E, coefs[1]);

            exponential[approxNum] = new double[2];
            exponential[approxNum][0] = aCoef; exponential[approxNum][1] = bCoef;

            for (int i = 0; i < data.Length; i++)
            {
                double y = bCoef * Math.Pow(Math.E, coefs[0] * data[i].x);
                points[i] = new Point(data[i].x, y);
            }

            return points;
        }

        public static Point[] ApproximatePolinomical(Point[] data, int apprixNum)
        {
            Point[] points = new Point[data.Length];

            double x4Sum = 0; Array.ForEach(data, (x) => x4Sum += Math.Pow(x.x, 4));
            double x3Sum = 0; Array.ForEach(data, (x) => x3Sum += Math.Pow(x.x, 3));
            double x2Sum = 0; Array.ForEach(data, (x) => x2Sum += Math.Pow(x.x, 2));
            double xSum = 0; Array.ForEach(data, (x) => xSum += x.x);
            double ySum = 0; Array.ForEach(data, (x) => ySum += x.y);
            double x2ySum = 0; Array.ForEach(data, (x) => x2ySum += Math.Pow(x.x, 2) * x.y);
            double xySum = 0; Array.ForEach(data, (x) => xySum += x.y * x.x);

            double n = data.Length;

            double[][] sysMatrix = new double[3][];
            sysMatrix[0] = new double[] { x4Sum, x3Sum, x2Sum, x2ySum };
            sysMatrix[1] = new double[] { x3Sum, x2Sum, xSum, xySum };
            sysMatrix[2] = new double[] { x2Sum, xSum, n, ySum };

            double[] coefs = GetDets3(sysMatrix);
            polinomical[apprixNum] = new double[3];
            for (int i = 0; i < 3; i++)
                polinomical[apprixNum][i] = coefs[i];

            for (int i = 0; i < data.Length; i++)
            {
                double y = coefs[0] * Math.Pow(data[i].x, 2) + coefs[1] * data[i].x + coefs[2];
                points[i] = new Point(data[i].x, y);
            }

            return points;
        }

        public static double[] GetLinearCoefs(int approxNum)
        {
            return linear[approxNum];
        }

        public static double[] GetLogarithmicFoefs(int approxNum)
        {
            return logarithmic[approxNum];
        }

        public static double[] GetExponentialCoefs(int approxNum)
        {
            return exponential[approxNum];
        }

        public static double[] GetPolinomicalCoefs(int appoxNum)
        {
            return polinomical[appoxNum];
        }
    }
}
