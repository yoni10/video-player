using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VideoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string videoCode;
        private string customerCode;
        private RegistryKey regKey;
        private bool isReg = false;
        private string currentRegVideoPath;

        public MainWindow()
        {
            InitializeComponent();
            Init();
            
        }

        private void Init() {
            State.VideoCode = System.IO.File.ReadAllText("files/videoCode.txt");
            State.CustomerCode = System.IO.File.ReadAllText("files/customerCode.txt");
            currentRegVideoPath = Globals.PURCHASE_PATH + State.CustomerCode + "\\" + State.VideoCode + '\\';
            //regKey = Registry.CurrentUser.OpenSubKey(currentRegVideoPath, true);

            if (NeedToPurchaseVideo())
            {
                PurchaseWindow w = new PurchaseWindow();
 
                w.Show();
                this.Close();
            }
            else 
            {
                //TODO implement go to video page
            }
        }


        private bool NeedToPurchaseVideo() {
            if (regKey == null)
                return true;

            try
            {
                object paid = regKey.GetValue("Paid");

                if (paid == null || paid.ToString() != "true")
                    return true;                

                State.IsPaid = true;
                State.PinCode = regKey.GetValue("randomPinCode").ToString();

            }
            catch(Exception e)
            {
                return true;
            }

            return false;
        }
    }
}
