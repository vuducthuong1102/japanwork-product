using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManageEmailServerModel : CommonPagingModel
    {
        public List<IdentityEmailServer> SearchResults { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }
    }

    public class EmailServerUpdateModel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]        
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int Status { get; set; }

        public EmailServerSMTPModel SendingConfig { get; set; }
        public EmailServerPOPModel ReceivingConfig { get; set; }
        public bool TestingSuccessed { get; set; }
    }

    public class EmailServerSMTPModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_SMTP_SERVER))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string SMTPServer { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PORT_SERVER))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public int Port { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_SSL_REQUIRED))]
        public bool SSLRequired { get; set; }
    }

    public class EmailServerPOPModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_POP_SERVER))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string POPServer { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PORT_SERVER))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public int Port { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_SSL_REQUIRED))]
        public bool SSLRequired { get; set; }
    }

    public class ManageEmailSettingsModel
    {
        public List<IdentityEmailServer> EmailServers { get; set; }
        public ManageEmailSettingItemModel InComing { get; set; }
        public ManageEmailSettingItemModel OutGoing { get; set; }
    }

    public class ManageEmailSettingItemModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_EMAIL))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [EmailAddress(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_EMAIL_INVALID))]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PASSWORD))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string EmailPassword { get; set; }

        public int EmailServerId { get; set; }

        public int EmailType { get; set; }

        public bool TestingSuccessed { get; set; }

        public bool PasswordChanged { get; set; }
    }
}