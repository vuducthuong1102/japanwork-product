using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;

namespace Manager.WebApp.Models
{
    public class ManageAgencyModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityAgency> SearchResults { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }
        public List<dynamic> ListUsers { get; set; }
    }

    public class AgencyCommonUpdateModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_COMPANY_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Company { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PERSON_INCHARGE_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_ADDRESS))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Address { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_EMAIL))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [EmailAddress(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_EMAIL_INVALID))]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_WEBSITE))]
        public string Website { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PHONE))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        //[Phone(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_PHONE_INVALID))]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_LOGO))]
        public string LogoPath { get; set; }

        public string LogoFullPath { get; set; }
    }

    public class AgencyCreateModel : AgencyCommonUpdateModel
    {

    }

    public class AgencyEditModel : AgencyCommonUpdateModel
    {
        public HttpPostedFileBase image_file_upload { get; set; }
    }

    public class AgencyRequestSendEmailActiveModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_EMAIL))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [EmailAddress(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_EMAIL_INVALID))]
        public string Email { get; set; }

        public string ActiveLink { get; set; }
    }

    public class AgencyRequestForgotPwdModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_EMAIL))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [EmailAddress(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_EMAIL_INVALID))]
        public string Email { get; set; }

        public string ActiveLink { get; set; }
    }

    public class AgencyRegisterModel : AgencyCommonUpdateModel
    {
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PASSWORD))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CONFIRM_PASSWORD))]
        [Compare("Password", ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_CONFRIM_PASSWORD_NOT_MATCH))]
        public string ConfirmPassword { get; set; }

        //Extensions
        public string ActiveLink { get; set; }
                
        [Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.LB_TERMS_AND_CONDTION_CHECK_FIRST))]
        public bool TermsAndConditions { get; set; }
    }

    public class AgencyNotificationModel
    {
        public int CurrentPage { get; set; }
        public IdentityAgencyNotification NotifInfo { get; set; }
        public IdentityCv CvInfo { get; set; }
    }
}