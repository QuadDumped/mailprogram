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
using System.Windows.Forms;

namespace MailProgram
{
 
    public partial class WriteMail : Window
    {
        public string input;
        public string file;
        public bool hasClosed;


        public WriteMail()
        {
            InitializeComponent();
            
        }

        public void replyAdress(string adress) //Skriver in svarsadress i fältet
        {
            to.Text = adress;
        }



        private void Done_Click(object sender, RoutedEventArgs e)
        {
            hasClosed = false;
            Hide();        
            
        }

        private void Attachfilebutton_Click(object sender, RoutedEventArgs e) //Input anges och användaren får ange vägen för filen
        {
            input = "attachfile";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            file = dialog.FileName;
            filelabel.Content = file; //Skriver ut vägen i en label vid sidan av knappen
          
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
   
            Hide();
            hasClosed = true;
        }
    }
}
