using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alx.Models
{
    public class SendEmail
    {
        public int SendEmailPrivateGmail(string strTo, string strSubject, string strMessage)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            //mail.From = new System.Net.Mail.MailAddress("platformaadautototal@gmail.com");
            mail.From = new System.Net.Mail.MailAddress("AlxAnunturi@gmail.com");
            mail.Subject = strSubject;
            string Body = strMessage;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            mail.To.Add(strTo);

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.PickupDirectoryLocation = "C:\\Inetpub\\mailroot\\Queue";
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            //smtp.Credentials = new System.Net.NetworkCredential("platformaadautototal@gmail.com", "250037941");
            smtp.Credentials = new System.Net.NetworkCredential("AlxAnunturi@gmail.com", "ParolaAlx1");
            smtp.EnableSsl = true;

            try
            {
                smtp.Send(mail);
                smtp = null;
                mail.Dispose();
                mail = null;
            }
            catch
            {
                return 0;
            }

            return 1;
        }

        public int SendEmailPrivateGmail(string strTo, string strFrom, string strSubject, string strMessage)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            //mail.From = new System.Net.Mail.MailAddress("platformaadautototal@gmail.com");
            mail.From = new System.Net.Mail.MailAddress("AlxAnunturi@gmail.com");
            mail.Subject = strSubject;
            string Body = @"<br />Ati primit un mesaj din partea utilizatorului cu email-ul:<br />" +
                            @"<i><b>" + strFrom + @"</b></i><br />" +
                            @"<br /><br /><b>Mesaj</b>:<br />" + strMessage;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            mail.To.Add(strTo);

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.PickupDirectoryLocation = "C:\\Inetpub\\mailroot\\Queue";
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            //smtp.Credentials = new System.Net.NetworkCredential("platformaadautototal@gmail.com", "250037941");
            smtp.Credentials = new System.Net.NetworkCredential("AlxAnunturi@gmail.com", "ParolaAlx1");
            smtp.EnableSsl = true;

            try
            {
                smtp.Send(mail);
                smtp = null;
                mail.Dispose();
                mail = null;
            }
            catch
            {
                return 0;
            }

            return 1;
        }
    }
}