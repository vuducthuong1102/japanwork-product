using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Manager.WebApp.Models
{
    public class EmailModel
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public List<string> Receivers { get; set; }
        public string SenderPwd { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string MailServer { get; set; }
        public bool UseSSL { get; set; }
        public int Port { get; set; }
        public List<Attachment> Attachments { get; set; }

        public string SenderPwdHash { get; set; }        
    }

    public class BatchEmailModel
    {
        public string Sender { get; set; }
        public List<ManageEmailBatchSendingModel> Receivers { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string MailServer { get; set; }
        public bool UseSSL { get; set; }
        public int Port { get; set; }
        public List<Attachment> Attachments { get; set; }
        public string SenderPwdHash { get; set; }
    }
}