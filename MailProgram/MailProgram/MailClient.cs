using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S22.Imap;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Net.Mime;
using System.Windows;


namespace MailProgram
{
    public class MailClient
    {
        ImapClient imapClient;
        SmtpClient smtpClient;
        NetworkCredential credentials;
       
        public List<uint> ids;
        public List<MailMessage> messages = new List<MailMessage>();
        MailMessage mail = new MailMessage();


        public void storeLogin(string username, string password)
        {
            string folder = @"c:\logdata";

            if (!Directory.Exists(folder)) //Om inget directory finns sedan tidigare så skapas en.
            {
                Directory.CreateDirectory(folder);
            }

            string file = "login.txt";
            string path = Path.Combine(folder, file); //
            FileStream fs = File.Create(path);           
            fs.Close();

            StreamWriter sw = new StreamWriter(path, true);
            string key = "8f797g79h697g6787hg876f"; //Nyckeln används för att kryptera och dekryptera användarnamnet och lösenordet
            string encrypt1 = EncryptionFunctions.EncryptString(username, key);
            string encrypt2 = EncryptionFunctions.EncryptString(password, key);
            sw.WriteLine(encrypt1);
            sw.WriteLine(encrypt2);
            sw.Close();
            return;
        }



        public void GetNewMessages()
        {

           int currentYear = DateTime.Now.Year;
           int currentMonth = DateTime.Now.Month;

           ids = imapClient.Search(SearchCondition.SentSince(new DateTime(currentYear, currentMonth, 1))).ToList<uint>();
           foreach (uint id in ids)
           {
                mail = imapClient.GetMessage(id, false, null);
                messages.Add(mail);
           }
            return;
        }

        public void bLogin(string username, string password)
        {
            imapClient = new ImapClient("imap.gmail.com", 993, true);

            try //Undantagshantering
            {
                imapClient.Login(username, password, AuthMethod.Auto);
            }

            catch(Exception e)
            {
                MessageBox.Show("Could not login, have you entered the correct credentials?");
                Environment.Exit(0);             
            }          

            credentials = new NetworkCredential(username, password);
            return;
        }

        public void bWriteMail(string to, string subject, string content )
        {
            smtpConnect();

            try
            {
                MailMessage sendmail = new MailMessage(credentials.UserName, to, subject, content);
                smtpClient.Send(sendmail);
            }

            catch
            {
                MessageBox.Show("Message not sent, you need to enter proper information.", "Error");
                return;
            }
        }

        public void bWriteMail(string to, string subject, string content, string file)
        {
            smtpConnect();

            try
            {
                MailMessage sendmail = new MailMessage(credentials.UserName, to, subject, content);
                Attachment data = new Attachment(file, MediaTypeNames.Application.Octet); //Filen läggs till som en attachment i mailet
                sendmail.Attachments.Add(data);
                smtpClient.Send(sendmail);
            }

            catch
            {
                MessageBox.Show("Message not sent, you need to enter proper information.", "Error");
                return;
            }

          
        }

        public void bForwardMail(string to, string subject, string originalsender, string originaltarget, string originalmessage)
        {
            smtpConnect();
            string msg = originalmessage.Insert(0, "---------- Forwarded message --------- \nFrom: <" + originalsender + ">\nTo: " + "<" + originaltarget + ">\n Subject: " + subject + "\n"); //Sätter in ytterliggare information i början av mailet.
            try
            {
                MailMessage sendmail = new MailMessage(credentials.UserName, to, subject, msg);
                smtpClient.Send(sendmail);
            }

            catch
            {
                MessageBox.Show("Message not sent, you need to enter proper information", "Error");
            }
        }

        public void bDownloadMail(string body, string subject, string from, string to)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog(); //Öppnar en dialog för att ange mapp

                Random r = new Random();
                string selectedpath = dialog.SelectedPath;
                string filename = "Mail" + r.Next(1, 99999) + "(" + from + ")" + ".txt";
                string path = Path.Combine(selectedpath, filename);
                FileStream fs = File.Create(path);
                fs.Close();

                StreamWriter sw = new StreamWriter(path, true); //Skriver in mailinformationen i textdokumentet
                string text = "";
                text += "From: " + from;
                text += "\r\nTo: " + to + "\r\n";
                text += "Subject: " + subject + "\r\n\r\n";
                text += body;
                sw.Write(text);
                sw.Close();
            }          
        }

        public void bWriteMailReply (string to, string subject, string content, MailMessage mail) //Samma som writemail men utformad för att skriva svar
        {

            smtpConnect();

            try
            {         

                MailMessage replymail = new MailMessage(credentials.UserName, to, subject, content);

                //Här läggs det till speciella headers, vilket krävs för att mailet ska räknas som en reply av gmail.

                string ID = mail.Headers["Message-ID"];
                replymail.Headers.Add("In-Reply-To", ID);

                string references = mail.Headers["References"];
                if (!string.IsNullOrEmpty(references))
                    references += ' ';
                replymail.Headers.Add("References", references + ID);

                if (!mail.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
                    replymail.Subject = "Re: " + mail.Subject;
                smtpClient.Send(replymail);
            }

            catch {
                MessageBox.Show("Message not sent, you need to enter proper information.", "Error");
                return;
            }

        }

        public void bWriteMailReply(string to, string subject, string content, MailMessage mail, string file) 
        {

            smtpConnect();
            try
            {
                MailMessage replymail = new MailMessage(credentials.UserName, to, subject, content);
                string ID = mail.Headers["Message-ID"];
                replymail.Headers.Add("In-Reply-To", ID);

                string references = mail.Headers["References"];
                if (!string.IsNullOrEmpty(references))
                    references += ' ';
                replymail.Headers.Add("References", references + ID);

                if (!mail.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
                    replymail.Subject = "Re: " + mail.Subject;

                MailMessage sendmail = new MailMessage(credentials.UserName, to, subject, content);
                Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
                replymail.Attachments.Add(data);
                smtpClient.Send(replymail);
            }

            catch
            {
                MessageBox.Show("Message not sent, have you entered correct information?", "Error");
                file = "";
                return;
            }

        }

        void smtpConnect() //Ansluter till smtp
        {
            smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
        }


















    }
}
