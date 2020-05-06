using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace FileReading.Models
{
    public class ResponseApiModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string TokenKey { get; set; }
        public dynamic Data { get; set; }
    }

    public class FileUploadResponseModel
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public string Path { get; set; }
        public string CoverPath { get; set; }
        public string CoverFullPath { get; set; }
    }

    public class FilesDeleteModel
    {
        public List<string> FilesPath { get; set; }
    }

    public class EmailSendingModel
    {
        public string email { get; set; }
        public int job_seeker_id { get; set; }
        public int company_id { get; set; }            
        public string subject { get; set; }
        [AllowHtml]
        public string body { get; set; }
        public int is_online { get; set; }
        public List<HttpPostedFileBase> attachments { get; set; }
    }

    public class BatchEmailModel : BaseEmailItemModel
    {
        public List<EmailSendingModel> Receivers { get; set; }       
    }

    public class BaseEmailItemModel
    {
        public string Sender { get; set; }       
        public string Receiver { get; set; }       
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string MailServer { get; set; }
        public bool UseSSL { get; set; }
        public int Port { get; set; }
        public List<Attachment> Attachments { get; set; }
        public string SenderPwdHash { get; set; }
        public int TargetType { get; set; }
    }
}