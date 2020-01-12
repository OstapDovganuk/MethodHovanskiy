using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Documents;

namespace MethodHovantskiy
{
    /// <summary>
    /// Interaction logic for MethodHovanskiyRikati.xaml
    /// </summary>
    public partial class MethodHovanskiyRikati : Window
    {
        private int n;
        private int m;
        private double eps;
        private double L;
        private double K;
        private double[,] A;
        private double[,] B;
        private double[,] C;
        private double[,] F;
        private double[,] X_0;
        private double[,] XK;
        private double[,] LE;
        private double[,] KE;
        private int k;
        public MethodHovanskiyRikati()
        {
            InitializeComponent();
        }

        private void Calc_mhr_Click(object sender, RoutedEventArgs e)
        {
            int i;
            int j;
            int t;
            string[] arr = new string[1];
            string text;
            try
            {
                n = int.Parse(n_value.Text);
                m = int.Parse(m_value.Text);
                eps = double.Parse(eps_value.Text);
                if (eps < 0 || n<0 || m<0)
                {
                    throw new ArgumentException();
                }
                L = double.Parse(l_value.Text);
                K = double.Parse(k_value.Text);
                KE = new double[n, n];
                LE = new double[n, n];

                for (i = 0; i < n; i++)
                {
                    KE[i, i] = K;
                    LE[i, i] = L;
                }

                text = new TextRange(a_value.Document.ContentStart, a_value.Document.ContentEnd).Text;
                arr = text.Split(' ', '\n');
                A = new double[n, n];
                t = 0;
                for (i = 0; i < n; i++)
                {
                    for (j = 0; j < n; j++)
                    {
                        A[i, j] = double.Parse(arr[t]);
                        t++;
                    }
                }

                text = new TextRange(b_value.Document.ContentStart, b_value.Document.ContentEnd).Text;
                arr = text.Split(' ', '\n');
                B = new double[m, m];
                t = 0;
                for (i = 0; i < m; i++)
                {
                    for (j = 0; j < m; j++)
                    {
                        B[i, j] = double.Parse(arr[t]);
                        t++;
                    }
                }

                text = text = new TextRange(c_value.Document.ContentStart, c_value.Document.ContentEnd).Text;
                arr = text.Split(' ', '\n');
                C = new double[n, m];
                t = 0;
                for (i = 0; i < n; i++)
                {
                    for (j = 0; j < m; j++)
                    {
                        C[i, j] = double.Parse(arr[t]);
                        t++;
                    }
                }

                text = new TextRange(f_value.Document.ContentStart, f_value.Document.ContentEnd).Text;
                arr = text.Split(' ', '\n');
                F = new double[m, n];
                t = 0;
                for (i = 0; i < m; i++)
                {
                    for (j = 0; j < n; j++)
                    {
                        F[i, j] = double.Parse(arr[t]);
                        t++;
                    }
                }

                text = new TextRange(x0_value.Document.ContentStart, x0_value.Document.ContentEnd).Text;
                arr = text.Split(' ', '\n');
                X_0 = new double[n, m];
                t = 0;
                for (i = 0; i < n; i++)
                {
                    for (j = 0; j < m; j++)
                    {
                        X_0[i, j] = double.Parse(arr[t]);
                        t++;
                    }
                }
                result_value.Document.Blocks.Clear();
                MethoHOVR();
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Дані мають бути більші нуля!!!");
            }
            catch
            {
                MessageBox.Show("Хибні дані!!!");
            }
        }
        private void MethoHOVR()
        {
            double[,] T1 = new double[n, n];
            double[,] T2 = new double[n, n];
            double[,] T3 = new double[n, m];
            double[,] T4 = new double[n, m];
            double[,] T5 = new double[n, m];
            double norma;
            XK = new double[n, m];
            k = 0;

            do
            {

                Thread thear1 = new Thread(() => { T1 = CalcLXF(n, m, LE, X_0, F); });
                Thread thear2 = new Thread(() => { T2 = CalcLA(n, m, LE, A); });
                Thread thear3 = new Thread(() => { T3 = CalcKX(n, m, KE, X_0); });
                Thread thear4 = new Thread(() => { T4 = CalcLXB(n, m, LE, X_0, B); });
                Thread thear5 = new Thread(() => { T5 = CalcLC(n, m, LE, C); });
                thear1.Start();
                thear2.Start();
                thear3.Start();
                thear4.Start();
                thear5.Start();
                thear1.Join();
                thear2.Join();
                thear3.Join();
                thear4.Join();
                thear5.Join();
                MatrixOperation.AddMatr(n, T1, T2, out T1);
                MatrixOperation.AddMatr(n, T1, KE, out T1);
                MatrixOperation.SerchObernen(n, T1, out T1);
                MatrixOperation.DifferenceMatr(n, m, T3, T4, out T3);
                MatrixOperation.DifferenceMatr(n, m, T3, T5, out T3);
                MatrixOperation.MultiplicationMatr(n, n, m, T1, T3, out T3);
                norma = MatrixOperation.NormaMax(n, m, T3, X_0);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        X_0[i, j] = T3[i, j];
                    }
                }                
                k++;
            }
            while (norma > eps);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    XK[i, j] = T1[i, j];
                }
            }           
            string temp = null;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    temp += XK[i, j].ToString("E5") + "   ";
                }
                result_value.Document.Blocks.Add(new Paragraph(new Run(temp)));
                temp = null;
            }
            result_value.Document.Blocks.Add(new Paragraph(new Run("Кількість ітерацій: " + k.ToString())));
        }
        private double[,] CalcLXF(int n, int m, double[,] L, double[,] X, double[,] F)
        {
            double[,] T = new double[n, n];
            double[,] G = new double[n, m];
            MatrixOperation.MultiplicationMatr(n, n, m, L, X, out G);
            MatrixOperation.MultiplicationMatr(m, n, n, G, F, out T);
            return T;
        }
        private double[,] CalcLA(int n, int m, double[,] L, double[,] A)
        {
            double[,] T = new double[n, n];
            MatrixOperation.MultiplicationMatr(n, n, n, L, A, out T);
            return T;
        }
        private double[,] CalcKX(int n, int m, double[,] K, double[,] X)
        {
            double[,] T = new double[n, m];
            MatrixOperation.MultiplicationMatr(n, n, m, K, X, out T);
            return T;
        }
        private double[,] CalcLXB(int n, int m, double[,] L, double[,] X, double[,] B)
        {
            double[,] T = new double[n, m];
            MatrixOperation.MultiplicationMatr(n, n, m, L, X, out T);
            MatrixOperation.MultiplicationMatr(m, n, m, T, B, out T);
            return T;
        }
        private double[,] CalcLC(int n, int m, double[,] L, double[,] C)
        {
            double[,] T = new double[n, m];
            MatrixOperation.MultiplicationMatr(n, n, m, L, C, out T);
            return T;
        }
    }
}
