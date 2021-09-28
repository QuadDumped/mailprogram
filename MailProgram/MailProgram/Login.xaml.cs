using System;
using System.Collections.Generic;
using System.IO;
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

namespace MailProgram
{
   
    public partial class Login : Window
    {
        bool hasInitialized = true;
        bool hasInitialized2 = true;
        public bool loginsave = false; //Om man har checkat checkbox för att spara användarinformationen eller inte.
       

        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void Textbox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    

        private void Textbox2_Initialized(object sender, EventArgs e)
        {
            if (MainWindow.hasLoggedin == false) //Om man inte har loggat in sedan tidigare och sparat användarinformationen så får textfälten en placeholder.
            {
                textbox2.Password = "password12345678";
                textbox2.Foreground = Brushes.Gray;
            }
        }

        private void Textbox1_Initialized(object sender, EventArgs e)
        {
            if (MainWindow.hasLoggedin == false)
            {
                textbox1.Text = "username";
                textbox1.Foreground = Brushes.Gray;
            }
        }

        private void Textbox1_MouseEnter(object sender, MouseEventArgs e)
        {
            if (hasInitialized == true && MainWindow.hasLoggedin == false) //Placeholdertexten tas bort när man går över med muspekaren, men endast en gång
            {
                textbox1.Text = "";
                textbox1.Foreground = Brushes.Black;
                hasInitialized = false;
            }
        }

        private void Textbox2_MouseEnter(object sender, MouseEventArgs e)
        {
            if (hasInitialized2 == true && MainWindow.hasLoggedin == false)
            {
                textbox2.Password = "";
                textbox2.Foreground = Brushes.Black;
                hasInitialized2 = false;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            loginsave = true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
