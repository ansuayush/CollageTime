using System;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;

namespace ExecViewHrk.WebUI.Helpers
{
    /// <summary>
    /// Shared SMTP sender using Web.config system.net/mailSettings (same as Self Onboarding).
    /// </summary>
    public static class MailSettingsEmailHelper
    {
        /// <summary>
        /// Sends an HTML email using system.net/mailSettings/smtp from Web.config.
        /// </summary>
        public static void Send(string to, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient address is required.", "to");

            var section = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
            if (section == null || section.Network == null)
                throw new InvalidOperationException("system.net/mailSettings/smtp is missing in Web.config.");

            string host = section.Network.Host;
            int port = section.Network.Port > 0 ? section.Network.Port : 587;
            string userName = section.Network.UserName;
            string password = section.Network.Password;
            bool enableSsl = section.Network.EnableSsl;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("SMTP host, userName, and password must be set in Web.config mailSettings.");

            string from = !string.IsNullOrWhiteSpace(userName)
                ? userName.Trim()
                : (ConfigurationManager.AppSettings["FromEmailAddressTraining"]
                   ?? ConfigurationManager.AppSettings["FromEmailAddress"]
                   ?? userName);

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(from.Trim());
                mail.To.Add(to.Trim());
                mail.Subject = subject ?? "";
                mail.Body = htmlBody ?? "";
                mail.IsBodyHtml = true;

                using (var smtp = new SmtpClient(host, port))
                {
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = enableSsl || port == 587 || port == 465;
                    // Must be false or Gmail returns 5.7.0 Authentication Required
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(userName.Trim(), password);
                    smtp.Timeout = 60000;
                    smtp.Send(mail);
                }
            }
        }

        /// <summary>
        /// Same as Send but never throws — returns false on failure.
        /// </summary>
        public static bool TrySend(string to, string subject, string htmlBody)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(to)) return false;
                Send(to, subject, htmlBody);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
