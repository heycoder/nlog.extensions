using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace HeyCoder.NLog.Extensions.Utils
{
    internal class MailUtil
    {
        public static void SendEmail(SendMail sendMail)
        {
            var mailMessage = new MailMessage { Subject = sendMail.Subject, Body = sendMail.Body, IsBodyHtml = sendMail.IsBodyHtml };
            sendMail.ToMailAddressList.ForEach(m => mailMessage.To.Add(m));
            sendMail.AttachmentList?.ForEach(attachment => mailMessage.Attachments.Add(attachment));
            mailMessage.From = new MailAddress(sendMail.FromMailAddress, sendMail.DisplayName, Encoding.UTF8);
            var client = new SmtpClient
            {
                UseDefaultCredentials = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(sendMail.UserName, sendMail.Password),
                Host = sendMail.Host
            };
            client.Send(mailMessage);
        }

       
    }

    internal class SendMail
    {
        public string Host { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string FromMailAddress { get; set; }

        public string DisplayName { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsBodyHtml { get; set; }

        public List<Attachment> AttachmentList { get; set; }
        public List<string> ToMailAddressList { get; set; }

    }
}
