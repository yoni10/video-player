using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VideoPlayer
{
    /// <summary>
    /// Interaction logic for ExitScreen.xaml
    /// </summary>
    public partial class ExitScreen : Window
    {
        public ExitScreen()
        {
            InitializeComponent();
        }

        private void Button_Click_yes(object sender, RoutedEventArgs e)
        {
            NoButton.IsHitTestVisible = false;
            YesButton.IsHitTestVisible = false;

            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.Current.Shutdown();
        }


        //***********************************************************************************************************
        private void Button_Click_no(object sender, RoutedEventArgs e)
        {
            YesButton.IsHitTestVisible = false;
            NoButton.IsHitTestVisible = false;

            this.Close();

        }
    }
}
