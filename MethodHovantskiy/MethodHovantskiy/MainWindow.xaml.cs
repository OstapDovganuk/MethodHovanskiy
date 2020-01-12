using System.Windows;

namespace MethodHovantskiy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Method_hovanskiy_rikati_Click(object sender, RoutedEventArgs e)
        {
            MethodHovanskiy mh = new MethodHovanskiy();
            mh.ShowDialog();
        }

        private void Method_hovanskiy_Click(object sender, RoutedEventArgs e)
        {
            MethodHovanskiyRikati mhr = new MethodHovanskiyRikati();
            mhr.ShowDialog();
        }
    }
}
