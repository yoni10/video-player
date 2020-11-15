using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;


namespace VideoPlayer
{
    /// <summary>
    /// Interaction logic for PurchaseWindow.xaml
    /// </summary>
    public partial class PurchaseWindow : Window
    {

        private string paymentLink = "https://secure.cardcom.solutions/e/xOJZ";
        private RegistryKey regKey;
        public bool isRegistryCodeCreated;
        private string pinCodeStart;
        private string pinCodeEnd;
        private Excel.Application excelApp;
        private Excel.Workbook codesWorkbook;
        private Excel.Worksheet worksheet;
        private const int codesCount = 80985;
        private const string codesFile = "files/codes.xlsx";

        public PurchaseWindow()
        {
            paymentLink = System.IO.File.ReadAllText("files/paymentLink.txt");
            InitializeComponent();

            var currentVideoPath = Globals.PURCHASE_PATH + State.CustomerCode + "\\" + State.VideoCode + '\\';

            regKey = Registry.CurrentUser.OpenSubKey(currentVideoPath, true);

            if (regKey == null)
                regKey = Registry.CurrentUser.CreateSubKey(currentVideoPath, true);               

            GetRegistryCode();
        }

        private void GetRegistryCode()
        {

            try
            {
                object randomPinCode = regKey.GetValue("randomPinCodeStart");

                if (randomPinCode != null)
                {
                    State.PinCode = randomPinCode.ToString();
                    pinCodeStart = State.PinCode.Substring(0, 5);
                    pinCodeEnd = State.PinCode.Substring(5, 5);
                }                
                else
                {
                    GenerateNewPinCode();
                }

                codeLabel.Text = pinCodeStart;
                codeTextbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(" אנו מתנצלים, אך לא ניתן לטעון את מסך התשלום , אנא פנה לתמיכה הטכנית בטלפון :02-5344559" + ex.ToString());
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                Application.Current.Shutdown();
            }
        }

        private void GenerateNewPinCode() {
            try
            {
                excelApp = new Excel.Application();
                codesWorkbook = excelApp.Workbooks.Open(System.IO.Path.Combine(Directory.GetCurrentDirectory(), codesFile), ReadOnly: true, Password: "5516");
                worksheet = codesWorkbook.ActiveSheet;
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;

                Random random = new Random();
                int codeIndex = 1 + random.Next(0, codesCount);

                pinCodeStart = GetCellValue(worksheet, 1, codeIndex);
                pinCodeEnd = GetCellValue(worksheet, 2, codeIndex);

                State.PinCode = pinCodeStart + pinCodeEnd;
                
                codesWorkbook.Close(SaveChanges: false);
                regKey.SetValue("randomPinCodeStart", pinCodeStart);
            }
            catch (Exception ex)
            {
                //ExceptionDetails.WriteException(ex);

                //throw;

                MessageBox.Show(" אנו מתנצלים, אך לא ניתן לטעון את מסך התשלום , אנא פנה לתמיכה הטכנית בטלפון :02-5344559" + ex.ToString());
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                Application.Current.Shutdown();
            }
        }

        private string GetCellValue(Excel.Worksheet worksheet, int col, int row)
        {
            if (worksheet.Cells[row, col].Value == null)
            {
                return "";
            }

            return worksheet.Cells[row, col].Value.ToString();
        }

        private void closePageClick(object sender, RoutedEventArgs e)
        {

            ExitScreen w = new ExitScreen();

            w.InitializeComponent();
            w.ShowDialog();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (codeTextbox.Text.Length != codeTextbox.MaxLength)
            {
                okButton.IsEnabled = false;
            }
            else
            {
                okButton.IsEnabled = true;
            }
        }

        private void leaveX(object sender, MouseEventArgs e)
        {
            XButton.ImageSource = normalX.Source;

        }
        private void enterX(object sender, MouseEventArgs e)
        {
            XButton.ImageSource = changeX.Source;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            StartLink();
            codeTextbox.Focus();
        }

        private void StartLink()
        {
            Process.Start(paymentLink /*+ codeStart)*/);
        }
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            string userInputCode = codeTextbox.Text;

            if (!userInputCode.Equals(pinCodeEnd))
            {
                codeTextbox.Text = "";

                ErrorMessageCode w = new ErrorMessageCode();

                w.InitializeComponent();
                w.ShowDialog();
            }
            else
            {

                SaveRegistryKeys();

                //TODO navigate to video page
                //ErrorMessageCode w = new ErrorMessageCode();
                //w.Show();
                this.Close();
            }
        }

        private void SaveRegistryKeys() {
            regKey.SetValue("Paid", "true");
            State.IsPaid = true;

            regKey.Close();
        }

        private void leaveContinue(object sender, MouseEventArgs e)
        {
            imgButton2.ImageSource = normal.Source;
        }
        //***********************************************************************************************************
        private void enterContinue(object sender, MouseEventArgs e)
        {

            imgButton2.ImageSource = change.Source;

        }
    }
}
