//using Newtonsoft.Json;
//using MySite.Resources;
//using System.ComponentModel.DataAnnotations;
//using SingleSignOn.DB.Sql.Entities;

//namespace MySite.Models
//{
//    public class WebAuthLoginViewModel
//    {
//        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_EMAILORPHONE_NULL))]
//        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_LOGIN_ACCOUNT))]
//        public string UserName { get; set; }

//        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_PASSWORD_NULL))]
//        public string Password { get; set; }

//        public string ReturnUrl { get; set; }

//        public int NumberOfFailedLogins { get; set; }

//        public bool RememberMe { get; set; }
//    }

//    public class ResponseWebAuthLoginModel
//    {
//        public int Code { get; set; }
//        public string Msg { get; set; }
//        public string TokenKey { get; set; }
//        public string ReturnUrl { get; set; }

//        public WebAuthUserLoginIdentity Data { get; set; }

//        public ResponseWebAuthLoginModel()
//        {
//            Data = new WebAuthUserLoginIdentity();
//        }
//    }
    
//    public class HashDataModel
//    {
//        [Required]
//        public string UserName { get; set; }

//        [Required]
//        public string PasswordHash { get; set; }

//        [Required]
//        public string Time { get; set; }

//        public string Result { get; set; }
//    }

//    public class WebAccountRegisterModel
//    {
//        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_EMAIL_NULL))]
//        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_EMAILORPHONE))]
//        public string UserName { get; set; }

//        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_PASSWORD_NULL))]
//        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_PASSWORD))]
//        [JsonIgnore]
//        public string Password { get; set; }

//        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_CONFIRM_PASSWORD_NULL))]
//        [Compare("Password", ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_PASSWORD_NOT_MATCH))]
//        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_CONFIRM_PASSWORD))]
//        [JsonIgnore]
//        public string ConfirmPassword { get; set; }

//        //[Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_NULL))]
//        //[MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        //[MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        //[EmailAddress(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_EMAIL_INVALID))]
//        //[Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_EMAIL))]
//        public string Email { get; set; }

//        public string Birthday { get; set; }

//        public string Sex { get; set; }

//        public string Address { get; set; }

     
//        public string Full_Name { get; set; }

//        //[Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_NICKNAME_NULL))]
//        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_NICKNAME))]
//        public string Display_Name { get; set; }

//        public string CMTND { get; set; }

//        //[Phone(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_PHONE_INVALID))]
//        //[Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_PHONENUMBER_NULL))]
//        //[Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_PHONE))]
//        public string Phone { get; set; }

//        public string Note { get; set; }

//        public bool HaveReadTerm { get; set; }
//    }

//    public class WebAccountUpdateProfileModel
//    {
//        public string Full_Name { get; set; }

//        public string Display_Name { get; set; }

//        public string Birthday { get; set; }

//        public int Sex { get; set; }

//        public string Address { get; set; }

//        [MaxLength(1000, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_1000_CHARACTERS))]
//        public string Note { get; set; }

//        public string Avatar { get; set; }

//        public int UserId { get; set; }

//        public string TokenKey { get; set; }
//    }

//    public class WebAccountChangePasswordModel
//    {
//        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_PASSWORD_NULL))]
//        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        [JsonIgnore]
//        public string OldPassword { get; set; }

//        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_PASSWORD_NULL))]
//        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        [JsonIgnore]
//        public string NewPassword { get; set; }

//        [Compare("NewPassword", ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_CONFIRMPWD_INCORRECT))]
//        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        [JsonIgnore]
//        public string ConfirmNewPassword { get; set; }

//        public int UserId { get; set; }
//        public string TokenKey { get; set; }
//    }

//    public class WebVerifyOTPModel
//    {
//        public int UserId { get; set; }

//        public string TokenKey { get; set; }

//        public string OTPCode { get; set; }

//        public string OTPType { get; set; }

//        public string PhoneNumber { get; set; }

//        public string ActionType { get; set; }
//    }

//    public class WebChangeAuthMethodModel
//    {
//        public string AuthMethod { get; set; }
//        public int UserId { get; set; }
//        public string TokenKey { get; set; }
//    }

//    public class UserCookieModel
//    {
//        public int UserId { get; set; }
//        public string TokenKey { get; set; }
//        public string OTPType { get; set; }
//    }

//    public class WebRecoverPasswordModel
//    {
//        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_PASSWORD_NULL))]
//        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        public string NewPassword { get; set; }

//        [Compare("NewPassword", ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_CONFIRMPWD_INCORRECT))]
//        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        public string ConfirmNewPassword { get; set; }

//        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_RECOVER_METHOD_NULL))]
//        public string RecoverMethod { get; set; }

//        public string PasswordType { get; set; }

//        public string Answer { get; set; }
//    }
//}
