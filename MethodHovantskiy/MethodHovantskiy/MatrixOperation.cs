using System;

namespace MethodHovantskiy
{
    public static class MatrixOperation
    {
        public static double NormaMax(int n, int m, double[,] A, double[,] B)
        {
            double max = Math.Abs(A[0, 0] - B[0, 0]);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (Math.Abs(A[i, j] - B[i, j]) > max)
                    {
                        max = Math.Abs(A[i, j] - B[i, j]);
                    }
                }
            }
            return max;
        }
        public static void MultiplicationMatr(int n, int m, int q, double[,] A, double[,] B, out double[,] T)
        {
            T = new double[m, q];
            double temp = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < q; j++)
                {
                    for (int r = 0; r < n; r++)
                    {
                        temp += A[i, r] * B[r, j];
                    }
                    T[i, j] = temp;
                    temp = 0;
                }
            }
        }
        public static void PowMatr(int n, int pow, double[,] A, out double[,] T)
        {
            T = new double[n, n];
            double[,] G = new double[n, n];
            double temp = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    G[i, j] = A[i, j];
                }
            }
            for (int p = 0; p < pow; p++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        for (int r = 0; r < n; r++)
                        {
                            temp += A[i, r] * G[r, j];
                        }
                        T[i, j] = temp;
                        temp = 0;
                    }
                }
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        G[i, j] = T[i, j];
                    }
                }
            }
        }
        public static void DifferenceMatr(int n, int m, double[,] A, double[,] B, out double[,] T)
        {
            T = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    T[i, j] = A[i, j] - B[i, j];
                }
            }
        }
        public static void AddMatr(int n, double[,] A, double[,] B, out double[,] T)
        {
            T = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    T[i, j] = A[i, j] + B[i, j];
                }
            }
        }
        public static void SerchObernen(int n, double[,] A, out double[,] ObernA)
        {
            int i, j, k, p;
            double sum;
            double[,] LU = new double[n, n];
            ObernA = new double[n, n];

            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n; j++)
                {
                    sum = 0;

                    if (i <= j)
                    {
                        for (k = 0; k < i; k++)
                            sum += LU[i, k] * LU[k, j];
                        LU[i, j] = A[i, j] - sum;
                    }
                    else
                    {
                        for (k = 0; k < j; k++)
                            sum += LU[i, k] * LU[k, j];
                        if (LU[j, j] == 0)
                            break;
                        LU[i, j] = (A[i, j] - sum) / LU[j, j];
                    }
                }
            }

            for (i = n - 1; i >= 0; i--)
            {
                for (j = n - 1; j >= 0; j--)
                {
                    sum = 0;
                    if (i == j)
                    {
                        for (p = j + 1; p < n; p++)
                            sum += LU[j, p] * ObernA[p, j];
                        ObernA[j, j] = (1 - sum) / LU[j, j];
                    }
                    else if (i < j)
                    {
                        for (p = i + 1; p < n; p++)
                            sum += LU[i, p] * ObernA[p, j];
                        ObernA[i, j] = -sum / LU[i, i];
                    }
                    else
                    {
                        for (p = j + 1; p < n; p++)
                            sum += ObernA[i, p] * LU[p, j];
                        ObernA[i, j] = -sum;
                    }
                }
            }
        }
    }
}

