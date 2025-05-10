using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEO_Memo.Helpers
{
    using System.Net;
    using System.Net.Mail;

    public class EmailHelper
    {
        public static void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress("baohuy23425@gmail.com", "CEO Memo System");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "amlv xrzu yefp amso"; 

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }

}