using System.Net.Mail;

namespace TestWebApplication
{
    public class SendEmailer
    {
        private readonly string emailLog = "Sent To: {0}, CC: {1}, Body: {2}";

        public void SendMail(MailMessage message)
        {
            var ccEmailAddress = string.Empty;

            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    if (message != null)
                    {
                        message = ExcludeDuplicateMailIds(message);

                        if (!string.IsNullOrEmpty(ccEmailAddress))
                        {
                            message.CC.Add(ccEmailAddress);
                        }
                    }
                    SmtpClient smtpClient = new SmtpClient();
                    message.IsBodyHtml = true;
                    message.BodyEncoding = System.Text.Encoding.UTF8;

                    //for production
                    smtpClient.Port = 587;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.EnableSsl = true;

                    //for local
                    //smtpClient.Port = 25;
                    //smtpClient.Host = "localhost";
                    //smtpClient.EnableSsl = false;

                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;

                    smtpClient.Credentials = new System.Net.NetworkCredential()
                    {
                        UserName = "portalservice000@gmail.com",
                        Password = "yknytnwgolnnnepg"
                    };

                    smtpClient.Send(message);

                    string emailContents = string.Empty;

                    if (message != null && message.To != null && message.Body != null)
                    {
                        string cc = message.CC != null ? message.CC.ToString() : string.Empty;
                        emailContents = String.Format(emailLog, message.To, cc, message.Body);
                    }

                }
                catch (Exception ex)
                {
                    string body = message != null ? message.Body : string.Empty;
                    string error = "Message: <br/>";
                    if (ex.InnerException != null)
                    {
                        error += ex != null ? ex.InnerException.Message : string.Empty;
                    }
                    error += "<br/><br/> Stack Trace:<br/>";
                    error += ex != null ? ex.StackTrace : string.Empty;
                    error += ex.InnerException != null ? "<br/><br/>Stack Trace(Inner Expection):<br/>" : string.Empty;
                    error += ex.InnerException != null ? ex.InnerException.StackTrace : string.Empty;
                    error += "<br/><br/>TO Email: <br/>";
                    error += message != null ? message.To.ToString() : string.Empty;
                    error += "<br/><br/>CC Email: <br/>";
                    error += message != null ? message.CC.ToString() : string.Empty;
                    error += "<br/><br/>BCC Email: <br/>";
                    error += message != null ? message.Bcc.ToString() : string.Empty;
                    SendFailureStatusEmail(body, error);
                }
            });
        }

        private MailMessage ExcludeDuplicateMailIds(MailMessage message)
        {
            if (message.To.Count > 0)
            {
                var toList = message.To.ToList();
                message.To.Clear();
                toList.Distinct().All(t =>
                {
                    message.To.Add(t); return true;
                });
            }

            if (message.CC.Count > 0)
            {
                var ccList = message.CC.ToList();
                message.CC.Clear();
                ccList.Distinct().Except(message.To).All(t =>
                {
                    message.CC.Add(t); return true;
                });
            }
            return message;
        }

        private void SendFailureStatusEmail(string body, string error)
        {
            try
            {
                MailMessage mail = new MailMessage();

                bool sendEmail = true;

                if (sendEmail)
                {
                    string environment = "Unknown Environment";

                    mail.Subject = "Failed to Send Email - " + environment;
                    mail.Body = body + "<br/>With Exception:<br/>" + error;
                    mail.IsBodyHtml = true;
                    mail.From = new MailAddress("portalservice000@gmail.com", "Customer Portal");
                    mail.To.Add("portalservice000@gmail.com");

                    if (mail.To.Count > 0)
                    {
                        SendMail(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                string test = ex.ToString();
            }
        }
    }
    
}
