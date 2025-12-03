using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace ExecViewHrk.Domain.Helper
{
    public class EmailProcessor
    {
        //public string MailToAddress = "drizzo@resnav.com";
        ////public string MailFromAddress = "WebtimeAdmin@resnav.com";
        //public bool UseSsl = true;
        ////public string Username = "WebtimeAdmin@resnav.com";
        ////public string Password = "rn8998999";

        //////public string Username = "ExecViewAdmih@resnav.com";
        //////public string Password = "rn8998999";

        ////public string ServerName = "smtp.gmail.com";
        ////public int ServerPort = 587;

        ////public string MailToAddress = "TechnicalSupport@hrknowledge.com";
        //public string MailFromAddress = "TechnicalSupport@Hrknowledge.com";
        //public string Username = "TechnicalSupport@hrknowledge.com";
        //public string Password = "pteX6!3\"9*P^beNV";
        //public string ServerName = "smtp.office365.com";
        //public int ServerPort = 587;

        public string MailToAddress = "nitesh_singhal@priyanet.com";
        public bool UseSsl = true;
        public string MailFromAddress = "nitesh_singhal@priyanet.com";
        public string Username = "nitesh_singhal@priyanet.com";
        public string Password = "Noida2017";
        public string ServerName = "smtp.1and1.com";
        public int ServerPort = 587;

        public bool WriteAsFile = false;
        public string FileLocation = @"c:\EmployMeMatch_emails";

        // constructor
        public EmailProcessor()
        {
        }

        public void SendTest()
        {
            Send(MailFromAddress, MailToAddress, "Test email", "This is a test message from ExecViewHrk");
        }

        public void Send(string from, string to, string subject, string body)
        {
            using (var smtpClient = new SmtpClient())
            {

                smtpClient.EnableSsl = UseSsl;
                smtpClient.Host = ServerName;
                smtpClient.Port = ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(Username, Password);

                if (WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = FileLocation;
                    smtpClient.EnableSsl = false;
                }

                //MailFromAddress = from;
                MailToAddress = to;

                MailMessage mailMessage = new MailMessage(MailFromAddress, MailToAddress, subject, body);

                if (WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                //smtpClient.Send(mailMessage);
            }


        }
    }
    public class EmailProcessorCommunity
    {
        private string m_strFrom;
        private string m_strTo;
        private string m_strBody = "";
        private string m_strSubject = "";
        private string m_strSMTPServer;

        public string SMTPServer
        {
            get { return m_strSMTPServer; }
            set { m_strSMTPServer = value; }
        }

        public string From
        {
            get { return m_strFrom; }
            set { m_strFrom = value; }
        }

        public string To
        {
            get { return m_strTo; }
            set { m_strTo = value; }
        }

        public string Subject
        {
            get { return m_strSubject; }
            set { m_strSubject = value; }
        }

        public string Body
        {
            get { return m_strBody; }
            set { m_strBody = value; }
        }

        public EmailProcessorCommunity(string strSMTPServer, string strFrom, string strTo, string strSubject, string strBody)
        {
            m_strSMTPServer = strSMTPServer;
            m_strFrom = strFrom;
            m_strTo = strTo;
            m_strSubject = strSubject;
            m_strBody = strBody;
        }

        static public void SendWithAttachment(string strFrom, string strTo, string strToSecond, string strToThree, string strToFour, string strToFive, string strSubject, string strBody, bool bEmailAsHTML, List<string> AllImportedFullPaths)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress(strFrom);
            mail.To.Add(strTo);
            mail.CC.Add(strToSecond);
            mail.CC.Add(strToThree);
            mail.CC.Add(strToFour);
            mail.CC.Add(strToFive);
            mail.Subject = "Encrypt" + " " + strSubject; 
            mail.Body = strBody;
            foreach (string FilePath in AllImportedFullPaths)
            {
                if (!string.IsNullOrEmpty(FilePath))
                {
                    if (System.IO.File.Exists(FilePath))
                    {
                        mail.Attachments.Add(new Attachment(FilePath));
                    }
                }
            }
            if (bEmailAsHTML)
                mail.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
        static public void SendSingleFileWithAttachment(string strFrom, string strTo, string strToSecond, string strToThree, string strSubject, string strBody, bool bEmailAsHTML, string filePath)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress(strFrom);
            mail.To.Add(strTo);
            mail.CC.Add(strToSecond);
            mail.CC.Add(strToThree);
            mail.Subject = "Encrypt" +" "+ strSubject;
            mail.Body = strBody;
            if (!string.IsNullOrEmpty(filePath))
            {
                mail.Attachments.Add(new Attachment(filePath));
            }
            if (bEmailAsHTML)
                mail.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        static public void SendMultipleFileWithAttachment(string strFrom, string strTo, string strToSecond, string strToThree, string strSubject, string strBody, bool bEmailAsHTML, List<string> filePath)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress(strFrom);
            mail.To.Add(strTo);
            mail.CC.Add(strToSecond);
            mail.CC.Add(strToThree);
            mail.Subject = strSubject;
            mail.Body = strBody;
            foreach (string FilePath in filePath)
            {
                if (!string.IsNullOrEmpty(FilePath))
                {
                    if (System.IO.File.Exists(FilePath))
                    {
                        mail.Attachments.Add(new Attachment(FilePath));
                    }
                }
            }           
            if (bEmailAsHTML)
                mail.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
        static public void sendEmailForSFTPBanner(string strFrom, string strTo, string strToSecond, string strToThree, string strSubject, string strBody, bool bEmailAsHTML)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress(strFrom);
            mail.To.Add(strTo);
            mail.CC.Add(strToSecond);
            mail.CC.Add(strToThree);
            mail.Subject = strSubject;
            mail.Body = strBody;
            if (bEmailAsHTML)
                mail.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        static public void Send(string strSMTPServer, string strFrom, string strTo, string strSubject, string strBody, bool bEmailAsHTML)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress(strFrom);
            mail.To.Add(strTo);
            mail.Subject = strSubject;
            mail.Body = strBody;
            if (bEmailAsHTML)
                mail.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        public static void SendEmail(string strSMTPServer, string strFrom, string strTo, string strSubject, string strBody, bool bEmailAsHTML)
        {
            MailMessage mailMessage = new MailMessage(strFrom, strTo)
            {
                Body = strBody,
                Subject = strSubject,
                IsBodyHtml = bEmailAsHTML
            };

            SmtpClient smtpClient = new SmtpClient("smtp.1and1.com", 587);
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "mail@priyanet.com",
                Password = "HYD2017"
            };
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
        }


        /// <summary>
        /// Treaty Limit Exceeded
        /// </summary>
        /// <param name="strFrom"></param>
        /// <param name="strTo"></param>
        /// <param name="strToSecond"></param>
        /// <param name="strSubject"></param>
        /// <param name="strBody"></param>
        /// <param name="bEmailAsHTML"></param>
        /// <param name="ms"></param>
        /// <param name="fileName"></param>
        /// 
        static public void SendEmailTreatyLimitExceeded(string strFrom, string strTo, string strBCC, string strSubject, string strBody, bool bEmailAsHTML)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress(strFrom);
            mail.To.Add(strTo);
            mail.Bcc.Add(strBCC);           
            mail.Subject = strSubject;
            mail.Body = strBody;
            if (bEmailAsHTML)
                mail.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        static public void SendEmailSingleFileAttachment(string strFrom, string strTo, string strToSecond, string strSubject, string strBody, bool bEmailAsHTML, System.IO.MemoryStream ms, string fileName)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress(strFrom);
            mail.To.Add(strTo);
            mail.Bcc.Add(strToSecond);
            mail.Subject = "Encrypt" + " " + strSubject; 
            mail.Body = strBody;
            if (!string.IsNullOrEmpty(fileName))
            {
                mail.Attachments.Add(new Attachment(ms, fileName));
            }            
           
            if (bEmailAsHTML)
                mail.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }



    }
}
