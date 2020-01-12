using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace MethodHovantskiy
{
    public partial class MethodHovanskiy : Window
    {
        private int n;
        private int m;
        private double eps;
        private double L;
        private double K;
        private double[,] X_0;
        private double[,] XK;
        private double[,] LE;
        private double[,] KE;
        private double[][,] An;
        private double[][,] Yn;
        private double[][,] Y0;
        private int k;

        public MethodHovanskiy()
        {
            InitializeComponent();
        }

        private void calc_MH_Click(object sender, RoutedEventArgs e)
        {
            int i;
            int j;
            int t;
            int k;
            string[] arr = new string[1];
            string text;
            try
            {
                n = int.Parse(power.Text);
                m = int.Parse(size.Text);
                eps = double.Parse(eps_value.Text);
                if (eps < 0 || n < 0 || m < 0)
                {
                    throw new ArgumentException();
                }
                L = double.Parse(l_value.Text);
                K = double.Parse(k_value.Text);
                KE = new double[m, m];
                LE = new double[m, m];
                for (i = 0; i < m; i++)
                {
                    KE[i, i] = K;
                    LE[i, i] = L;
                }
                text = new TextRange(matrix_an.Document.ContentStart,matrix_an.Document.ContentEnd).Text;
                string[] stringSeparators = new string[] { "\n", " " };
                arr = text.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                An = new double[n + 1][,];
                t = 0;
                for (k = 0; k < n + 1; k++)
                {
                    An[k] = new double[m, m];
                    for (i = 0; i < m; i++)
                    {
                        for (j = 0; j < m; j++)
                        {
                            An[k][i, j] = double.Parse(arr[t]);
                            t++;
                        }
                    }
                }
                text = new TextRange(matrix_x0.Document.ContentStart,matrix_x0.Document.ContentEnd).Text;
                arr = text.Split(' ', '\n');
                X_0 = new double[m, m];
                t = 0;
                for (i = 0; i < m; i++)
                {
                    for (j = 0; j < m; j++)
                    {
                        X_0[i, j] = double.Parse(arr[t]);
                        t++;
                    }
                }
                result.Document.Blocks.Clear();
                MethoHOV();
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

        private void MethoHOV()
        {
            double[,] T1 = new double[m, m];
            double[,] T2 = new double[m, m];
            double[,] T3 = new double[m, m];
            double[,] T4 = new double[m, m];

            Yn = new double[n - 2][,];
            Y0 = new double[n - 2][,];

            double norma;

            XK = new double[m, m];
            k = 0; 
            
            MatrixOperation.SerchObernen(m, X_0, out T1); /// x_0^-1
            // Y_0_n
            for (int i = 0; i < n - 2; i++)
            {
                Y0[i] = new double[m, m];
                Yn[i] = new double[m, m];
            }
            if (n - 2 != 0)
            {
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        Y0[0][i, j] = T1[i, j];
                    }
                }
            }
            for (int i = 1; i < n - 2; i++)
            {
                Y0[i] = new double[m, m];
                MatrixOperation.PowMatr(m, i, T1, out Y0[i]);
            }
            //          
            do
            {
                //Y_n
                if (n - 2 != 0)
                {
                    T2 = CalcLAX(m, LE, An[n], X_0);
                    MatrixOperation.AddMatr(m, T2, KE, out T2);
                    MatrixOperation.SerchObernen(m, T2, out T2);
                    MatrixOperation.MultiplicationMatr(m, m, m, LE, An[n], out T3);
                    MatrixOperation.MultiplicationMatr(m, m, m, KE, Y0[0], out T4);
                    MatrixOperation.AddMatr(m, T3, T4, out T3);
                    MatrixOperation.MultiplicationMatr(m, m, m, T2, T3, out Yn[0]);
                }
                for (int i = 1; i < n - 2; i++)
                {
                    MatrixOperation.MultiplicationMatr(m, m, m, LE, An[n], out T3);
                    MatrixOperation.MultiplicationMatr(m, m, m, T3, Yn[i - 1], out T3);
                    MatrixOperation.MultiplicationMatr(m, m, m, KE, Y0[i], out T4);
                    MatrixOperation.AddMatr(m, T3, T4, out T3);
                    MatrixOperation.MultiplicationMatr(m, m, m, T2, T3, out Yn[i]);
                }
                // X_n              
                Thread thear1 = new Thread(() => { T1 = CalcLAX(m, LE, An[n], X_0); });
                Thread thear2 = new Thread(() => { T2 = CalcLA(m, LE, An[n - 1]); });
                Thread thear3 = new Thread(() => { T3 = CalcLA(m, LE, An[n - 2]); });
                thear1.Start();
                thear2.Start();
                thear3.Start();
                thear1.Join();
                thear2.Join();
                thear3.Join();

                MatrixOperation.AddMatr(m, T1, KE, out T1);
                MatrixOperation.SerchObernen(m, T1, out T1);

                MatrixOperation.DifferenceMatr(m, m, KE, T2, out T2);
                MatrixOperation.MultiplicationMatr(m, m, m, T2, X_0, out T2);
                MatrixOperation.DifferenceMatr(m, m, T2, T3, out T2);

                for (int i = 0; i < n - 2; i++)
                {
                    T3 = CalcLAX(m, LE, An[n - 3 - i], Yn[i]);
                    MatrixOperation.DifferenceMatr(m, m, T2, T3, out T2);
                }

                MatrixOperation.MultiplicationMatr(m, m, m, T1, T2, out T2);
                norma = MatrixOperation.NormaMax(m, m, T2, X_0);

                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        X_0[i, j] = T2[i, j];
                    }
                }
                for (int r = 0; r < n - 2; r++)
                {
                    for (int i = 0; i < m; i++)
                    {
                        for (int j = 0; j < m; j++)
                        {
                            Y0[r][i, j] = Yn[r][i, j];
                        }
                    }
                }              
                k++;
            }
            while (norma > eps);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    XK[i, j] = T2[i, j];
                }
            }
            string temp = null;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    temp+= XK[i, j].ToString("E5") + "   ";
                }
                result.Document.Blocks.Add(new Paragraph(new Run(temp)));
                temp = null;
            }
            result.Document.Blocks.Add(new Paragraph(new Run("Кількість ітерацій: " + k.ToString())));
        }
        private double[,]  CalcLAX(int m, double[,] L, double[,] A, double[,] X)
        {
            double[,] T = new double[m, m];
            double[,] G = new double[m, m];
            MatrixOperation.MultiplicationMatr(m, m, m, L, A, out G);
            MatrixOperation.MultiplicationMatr(m, m, m, G, X, out T);
            return T;
        }
        private double[,] CalcLA(int m, double[,] L, double[,] A)
        {
            double[,] G = new double[m, m];
            MatrixOperation.MultiplicationMatr(m, m, m, L, A, out G);
            return G;
        }
    }
}
