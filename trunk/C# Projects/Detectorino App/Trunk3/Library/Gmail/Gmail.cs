using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Mail;



namespace Library
{
    public class Gmail
    {
        //variables

        //constructor
        public Gmail()
        { 
        
        }

        //functions
        public void SendEmail(string from, string to, string password, string subject, string body)
        {

            var fromAddress = new MailAddress(from, "From Name");
            var toAddress = new MailAddress(to, "To Name");
            string fromPassword = password;
            string Sub = subject;
            string Bod = body;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
               
            };
            using (var message = new MailMessage(from, to ,Sub ,Bod))
            {

                try
                {
                    smtp.Send(message);
                }
                catch (Exception)
                {
                    throw;
                } 
               
            }



        }
    }
}
