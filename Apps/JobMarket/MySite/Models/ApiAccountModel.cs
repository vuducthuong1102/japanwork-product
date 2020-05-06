using Newtonsoft.Json;
using MySite.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web;

namespace MySite.Models
{
    public class ApiRegisterModel
    {
        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_EMAILORPHONE_NULL))]
        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_EMAILORPHONE))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_PASSWORD_NULL))]
        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_PASSWORD))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_CONFIRM_PASSWORD_NULL))]
        [Compare("Password", ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_PASSWORD_NOT_MATCH))]
        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_CONFIRM_PASSWORD))]
        [JsonIgnore]
        public string ConfirmPassword { get; set; }

        //[Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_NULL))]
        //[MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        //[MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        //[EmailAddress(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_EMAIL_INVALID))]
        //[Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_EMAIL))]
        public string Email { get; set; }

        public string Birthday { get; set; }

        public string Sex { get; set; }

        public string Address { get; set; }


        public string Full_Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_NICKNAME_NULL))]
        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_NICKNAME))]
        public string Display_Name { get; set; }

        public string CMTND { get; set; }

        //[Phone(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_PHONE_INVALID))]
        //[Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_PHONENUMBER_NULL))]
        //[Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.IP_PHONE))]
        public string Phone { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Calling API time format: yyyyMMddHHmmss
        /// </summary>
        public string Time { get; set; }

        public bool IsPhoneNumber { get; set; }

        public bool IsEmail { get; set; }

        public string AppCode { get; set; }
    }

    public class ApiChangePasswordModel
    {
        public int UserId { get; set; }

        /// <summary>
        /// Password in MD5 encrypted
        /// </summary>
        public string OldPwd1 { get; set; }

        /// <summary>
        /// Password in MD5 encrypted
        /// </summary>
        public string OldPwd2 { get; set; }

        public string PwdType { get; set; }

        public string NewPwd { get; set; }

        /// <summary>
        /// Calling API time format: yyyyMMddHHmmss
        /// </summary>
        public string Time { get; set; }

        public string Token { get; set; }
    }

    public class ApiUpdateProfileModel
    {
        public int UserId { get; set; }

        /// <summary>
        /// Calling API time format: yyyyMMddHHmmss
        /// </summary>
        public string Time { get; set; }

        public string Token { get; set; }

        public string FieldName { get; set; }

        public string FieldValue { get; set; }
    }

    public class ApiUploadAvatarModel
    {
        public int UserId { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }

        public ApiUploadAvatarModel()
        {
            Files = new List<HttpPostedFileBase>();
        }       
    }

    public class ApiCropAvatarModel
    {
        /// <summary>
        /// Top
        /// </summary>
        public string t { get; set; }

        /// <summary>
        /// Left
        /// </summary>
        public string l { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        public string h { get; set; }

        /// <summary>
        /// Width
        /// </summary>
        public string w { get; set; }

        public string fileName { get; set; }
        public string userId { get; set; }
    }


    public class ApiChangeAuthMethodModel
    {
        public int UserId { get; set; }

        public string Token { get; set; }

        public string AuthMethod { get; set; }

        public string Time { get; set; }
    }

    public class ApiRecoverPasswordModel
    {
        public string Password { get; set; }

        public string RePassword { get; set; }

        public string PwdType { get; set; }

        //EMAIL, OTPSMS, ODPSMS, OTPAPP, ODPAPP or ID of security question
        public string QuestionId { get; set; }

        //EMAIL, OTPSMS, ODPSMS, OTPAPP, ODPAPP or ID of security question
        public string Answer { get; set; }

        /// <summary>
        /// Hash value: MD5(<MD5(password)>|<MD5(repassword)>|<time>|< pwdType>|<questionID>)
        /// </summary>
        public string Hash { get; set; }

        public int UserId { get; set; }

        public string Time { get; set; }
    }

    public class ApiActiveAccountModel
    {
        public string UserName { get; set; }

        public string ActiveMethod { get; set; }

        public string OTPCode { get; set; }

        public string HashingData { get; set; }

        public string Time { get; set; }
    }
}