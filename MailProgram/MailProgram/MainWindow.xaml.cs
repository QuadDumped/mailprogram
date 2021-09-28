using System;
using System.Windows;
using System.Windows.Controls;
using System.Net.Mail;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace MailProgram
{
  

    public partial class MainWindow : Window
    {
        Login login = new Login();
        MailClient mail = new MailClient();
        WriteMail write = new WriteMail();
        Read read = new Read();
        ForwardMail forward = new ForwardMail();


        public static bool hasLoggedin = false; //Om man har loggat in sedan tidigare och sparat informationen en gång.

        public MainWindow()
        {
            autoLogin(); //Sätter automatiskt in sparade uppgifter i fältet om användaren har sparat dem
            login.ShowDialog(); 
            mail.bLogin(login.textbox1.Text, login.textbox2.Password);

            if (login.loginsave == true) //Om man har checkat checkboxen för att spara
            {
                mail.storeLogin(login.textbox1.Text, login.textbox2.Password); //Lagrar uppgifterna i fälten om man har checkat i login
            }

            InitializeComponent();


            //Sätter en timer i förhållande till den nuvarande tiden
            DispatcherTimer timer = new DispatcherTimer(); 
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += dateTime;
            timer.Start();

        }


        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string item = "";
            try
            {
                item = box.SelectedItem.ToString();
            }

            catch
            {
                box.SelectedIndex = -1;
                return;
            }
       
            string selectedSubject = item.Substring(item.LastIndexOf('>') + 2); ; //Delar på strängen och hittar ämnet, sedan öppnas mailet som motsvarar det.

            foreach (MailMessage messages in mail.messages)        
            {
                           
                if (messages.Subject == selectedSubject)
                {
                    string messageFrom = messages.From.Address;
                    string messageDate = messages.Headers["Date"];

                    read.show(messages.Body, messages.Subject, messageFrom, messageDate);
                    read.ShowDialog();




                    switch (read.input) //Beroende av vad användaren trycker på i read så utförs olika operationer.
                    {
                        case "reply":

                            //Nollställer informationen i mailet
                            write.to.Text = "";
                            write.subject.Text = "";
                            write.replyAdress(messageFrom); //Sätter automatiskt in svarsadressen i fältet i write.
                            write.mailtext.Text = "";
                            write.file = "";
                            write.filelabel.Content = "";

                            write.ShowDialog();


                            if (write.hasClosed == false)
                            {
                                if (write.input == "attachfile") //Kör en overload av bWriteMail som även tar emot en fil, samma sak gäller när man skriver nya mail direkt. 
                                {
                                    mail.bWriteMailReply(write.to.Text, write.subject.Text, write.mailtext.Text, messages, write.file);
                                }

                                else
                                {
                                    mail.bWriteMailReply(write.to.Text, write.subject.Text, write.mailtext.Text, messages);
                                }
                            }
                          
                            break;

                        case "forward":
                            forward.ShowDialog();
                            if (forward.hasClosed == false)
                            {
                                mail.bForwardMail(forward.adressbox.ToString(), "Fwd: " + messages.Subject, messageFrom, messages.To.ToString(), messages.Body);
                            }
                            break;

                        case "download":
                            mail.bDownloadMail(messages.Body, messages.Subject, messageFrom, messages.To.ToString());
                            break;

                        default:                           
                            return;
                    }                               
                }
            }

        }



      



        private void Skriv_Click(object sender, RoutedEventArgs e)
        {
            write.to.Text = "";
            write.subject.Text = "";
            write.mailtext.Text = "";
            write.file = "";
            write.filelabel.Content = "";

            write.ShowDialog();


            if (write.hasClosed == false)
            {
                if (write.input == "attachfile")
                {
                    mail.bWriteMail(write.to.Text, write.subject.Text, write.mailtext.Text, write.file);
                    write.file = "";
                }

                else
                {
                    mail.bWriteMail(write.to.Text, write.subject.Text, write.mailtext.Text);
                }
            }
            
        }

        private void Inbox_Click_1(object sender, RoutedEventArgs e)
        {
            sort.Visibility = Visibility.Visible;
            box.Items.Clear(); //Rensar listbox så att nya items inte läggs över gamla
            mail.messages.Clear();
            mail.GetNewMessages();          
            mail.messages.Reverse(); //Vänder om listan med mail så att elementen hamnar i rätt ordning i listbox, sedan läggs sändaren och ämnet till i listbox..
            foreach (MailMessage sample in mail.messages)
            {                
                box.Items.Add("<" + sample.From.Address + "> " + sample.Subject);
            }
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0); //Stänger programmet
        }

        void autoLogin()
        {
            string folder = @"c:\logdata";
            string file = "login.txt";

            if (Directory.Exists(folder)) 
            {
                string path = Path.Combine(folder, file); //Kombinerar foldern och filen för att skapa en väg som streamreader kan läsa ifrån.
                StreamReader sr = new StreamReader(path);
                string key = "8f797g79h697g6787hg876f";
                login.textbox1.Text = EncryptionFunctions.DecryptString(sr.ReadLine(), key);
                login.textbox2.Password = EncryptionFunctions.DecryptString(sr.ReadLine(), key);
                sr.Close();
                hasLoggedin = true; 
            }
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            box.Items.Clear();

            //Lägger till alla adresser i en lista
            List<string> mailsenders = new List<string>(); 
            foreach(MailMessage mail in mail.messages)
            {
                mailsenders.Add(mail.From.Address);
            }

            mailsenders.Sort(); //Sorterar adresserna efter bokstavsordning

            //Hämtar ämnet från respektive mail efter adress, sedan slås adress och ämne ihop och läggs in i mailitems
            List<string> mailitems = new List<string>();
            foreach(string address in mailsenders)
            {
                foreach(MailMessage message in mail.messages)
                {
                    if (message.From.Address == address)
                    {
                        mailitems.Add("<" + message.From.Address + "> " + message.Subject);                      
                    }
                }
            }     

            //Rensar mailitems från kopior och lägger till den förfinade listan i cleanedmailitems, sedan läggs dessa items till i listbox
            List<string> cleanedMailitems = mailitems.Distinct().ToList();
            foreach(string mailitem in cleanedMailitems)
            {
                box.Items.Add(mailitem); 
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) //Gör så att man kan dra fönstret med musen
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized; //Minimerar fönstret
        }

        private void dateTime(object sender, EventArgs e)
        {

            DateTime localtime = DateTime.Now;
            time.Content = localtime.ToString("dddd HH:mm:ss");

        }


    }





}
