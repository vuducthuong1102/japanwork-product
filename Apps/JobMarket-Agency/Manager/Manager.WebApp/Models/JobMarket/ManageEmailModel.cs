using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;
using OpenPop.Mime.Header;
using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using Message = OpenPop.Mime.Message;

namespace Manager.WebApp.Models
{
    public class ManageEmailModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityEmailMessage> SearchResults { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }
    }

    public class ManageEmailViewModel
    {
        public int Id { get; set; }        
        public string IdHash { get; set; }
        public string MessageId { get; set; }
        public string Subject { get; set; }
        public bool IsRead { get; set; }
        public string CreatedDateStr { get; set; }
        public string ShortMessage { get; set; }
        public string Message { get; set; }

        public IdentityEmailAddress Sender { get; set; }
        public List<IdentityEmailAddress> Receiver { get; set; }
        public List<IdentityEmailAddress> Cc { get; set; }
        public List<IdentityEmailAddress> Bcc { get; set; }
        public List<IdentityEmailMessage> MessageParts { get; set; }
        public List<IdentityEmailAttachment> AttachFiles { get; set; }

        public int TotalAttachments { get; set; }
        public int TotalChilds { get; set; }
        public string DetailTk { get; set; }
        public string DetailPartTk { get; set; }
    }

    public class ManageEmailSendingModel
    {
        public int job_seeker_id { get; set; }
        public int company_id { get; set; }
        public string controller_name { get; set; }

        public string action_name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_RECEIVER))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string receiver { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TITLE))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string subject { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CONTENT))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [AllowHtml]
        public string body { get; set; }

        public int is_online { get; set; }

        //Token protection
        public string tk { get; set; }

        public List<HttpPostedFileBase> attachments { get; set; }

        public string ReceiversJsonList { get; set; }

        public int target_type { get; set; }
    }

    public class ManageEmailBatchSendingModel : ManageEmailSendingModel
    {
        public List<ManageEmailObjectInfoModel> SelectedObjects { get; set; }
    }

    public class ManageEmailObjectInfoModel
    {
        public int object_id { get; set; }
        public string email { get; set; }
        public string display_name { get; set; }
    }

    public class ManageEmailBusinessModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<ManageEmailBusinessViewModel> SearchResults { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        public int job_seeker_id { get; set; }

        public int company_id { get; set; }

        public int is_online { get; set; }
    }

    public class ManageEmailBusinessViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string CreatedDateStr { get; set; }
        public string ShortMessage { get; set; }
        public string Message { get; set; }

        public string Sender { get; set; }
        public string Receiver { get; set; }
        public List<IdentityEmailAddress> Cc { get; set; }
        public List<IdentityEmailAddress> Bcc { get; set; }
        public List<IdentityEmailAttachment> AttachFiles { get; set; }

        public int TotalAttachments { get; set; }
        public string DetailTk { get; set; }
        public string DetailPartTk { get; set; }
    }
}