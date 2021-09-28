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

namespace MailProgram
{
    public partial class Read : Window
    {
        public string sender; //Vem som skickade mailet man läser
        public string input;
       



        public Read()
        {
            InitializeComponent();                 
        }

        public void show(string body, string subject, string sender, string date) 
        {

            string msg = body.Insert(0, "From: " + sender + "\nSubject: " + subject + "\nReceived: " + date + "\n\n");
            LabelData.Content = msg;
            this.sender = sender;
      
        }

        /*public void show(string body) //Visar mailet i fönstret
        {
            mailbodybox.Text = body;
        }*/


        private void Replybutton_Click_1(object sender, RoutedEventArgs e)
        {
            input = "reply";
            Hide();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            input = ""; //Om man bara trycker på done så anges ingen input
            Hide();          
        }

        private void Forwardbutton_Click(object sender, RoutedEventArgs e)
        {
            input = "forward";
            Hide();
        }

        private void Downloadbutton_Click(object sender, RoutedEventArgs e)
        {
            input = "download";
            Hide();
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
            input = "";
            Hide();
        }
    }
}
