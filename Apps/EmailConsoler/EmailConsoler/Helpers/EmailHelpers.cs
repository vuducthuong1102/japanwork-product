using EmailConsoler.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace EmailConsoler.Helpers
{
    public class EmailHelpers
    {
        public static string SendEmail(EmailModel model)
        {
            var result = string.Empty;
            try
            {
                ServicePointManager.CertificatePolicy = new MyPolicy();
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(SystemSettings.EmailHost);
                
                mail.From = new MailAddress(model.Sender);

                if (!string.IsNullOrEmpty(model.Receiver))
                    mail.To.Add(model.Receiver);

                if (model.Receivers != null && model.Receivers.Count > 0)
                {
                    foreach (var item in model.Receivers)
                    {
                        mail.To.Add(item);
                    }
                }

                if(model.AttachmentLinks != null && model.AttachmentLinks.Count > 0)
                {
                    foreach (var item in model.AttachmentLinks)
                    {
                        var fileName = FileHelper.GetFileNameFromUrl(item);

                        if (string.IsNullOrEmpty(fileName))
                            continue;

                        var fileType = MimeMapping.GetMimeMapping(fileName);

                        var client = new WebClient();

                        // Download the PDF file from external site (pdfUrl) 
                        // to your local file system (pdfLocalFileName)
                        client.DownloadFile(item, fileName);

                        var att = new Attachment(fileName, fileType);
                        mail.Attachments.Add(att);

                        //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(item);
                        //using (HttpWebResponse HttpWResp = (HttpWebResponse)req.GetResponse())
                        //using (Stream responseStream = HttpWResp.GetResponseStream())
                        //using (MemoryStream ms = new MemoryStream())
                        //{
                        //    responseStream.CopyTo(ms);
                        //    ms.Seek(0, SeekOrigin.Begin);
                        //    Attachment attachment = new Attachment(ms, fileName, fileType);
                        //    mail.Attachments.Add(attachment);
                        //}
                    }
                }

                mail.Subject = model.Subject;
                mail.Body = model.Body;
                mail.SubjectEncoding = System.Text.Encoding.GetEncoding("utf-8");

                //Attachment attachment;
                //attachment = new Attachment(@"E:\NHS\N_Document\N_SVN\VID\document\send_email_example.txt");
                //mail.Attachments.Add(attachment);
                mail.IsBodyHtml = true;
                SmtpServer.Port = Convert.ToInt32(SystemSettings.EmailServerPort);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(model.Sender, model.SenderPwd);
                SmtpServer.EnableSsl = SystemSettings.EmailIsUseSSL;

                mail.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
                //mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(model.Body, new ContentType("text/html")));
                //mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(model.Body, new ContentType("text/plain")));

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                result = string.Format("Failed when sending email due to {0}", ex.ToString());
            }

            return result;
        }

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
