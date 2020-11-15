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
    /// Interaction logic for ErrorMessageCode.xaml
    /// </summary>
    public partial class ErrorMessageCode : Window
    {
        public ErrorMessageCode()
        {
            InitializeComponent();
        }

        private void closeErrorClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void leaveX1(object sender, MouseEventArgs e)
        {
            XButton1.ImageSource = normalX1.Source;

        }

        private void enterX1(object sender, MouseEventArgs e)
        {
            XButton1.ImageSource = changeX1.Source;
        }
    }
}
