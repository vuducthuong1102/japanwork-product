using Manager.SharedLibs;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Models;
using Manager.WebApp.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Manager.WebApp.Helpers
{
    public class EmailHelpers
    {
        private static readonly ILog logger = LogProvider.For<EmailHelpers>();

        public static string GetEmailTemplate(string templateName)
        {
            var content = string.Empty;
            try
            {
                //Read template file from the App_Data folder
                var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/Views/EmailTemplates/{0}.html", templateName));
                using (var sr = new StreamReader(mappedPath))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when GettingEmailTemplate [{0}] due to {1}", templateName, ex.ToString()));
            }

            return content;
        }

        public static string SendEmail(EmailModel model)
        {
            var result = string.Empty;
            try
            {
                System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(SystemSettings.EmailHost);

                mail.From = new MailAddress(model.Sender);
                mail.To.Add(model.Receiver);
                mail.Subject = model.Subject;
                mail.Body = model.Body;
                if (model.Attachments.HasData())
                {
                    foreach (var item in model.Attachments)
                    {
                        mail.Attachments.Add(item);
                    }
                }

                //Attachment attachment;
                //attachment = new Attachment(@"E:\NHS\N_Document\N_SVN\VID\document\send_email_example.txt");
                //mail.Attachments.Add(attachment);
                mail.IsBodyHtml = true;
                SmtpServer.Port = Convert.ToInt32(SystemSettings.EmailServerPort);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(model.Sender, model.SenderPwd);
                SmtpServer.EnableSsl = SystemSettings.EmailIsUseSSL;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                result = string.Format("Failed when sending email due to {0}", ex.ToString());
            }

            return result;
        }
    }

    public class EmailTemplateConst
    {
        public static string Sender = "{SENDER}";
        public static string Receiver = "{RECEIVER}";
        public static string RecoverPwdLink = "{RECOVER_PWD_LINK}";
        public static string ActiveAccountLink = "{ACTIVE_ACCOUNT_LINK}";
    }

    public class MyPolicy : ICertificatePolicy
    {
        public MyPolicy()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }
    }
}