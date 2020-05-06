using Newtonsoft.Json;
using SingleSignOn.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace SingleSignOn.Models
{
    public class ApiRegisterModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERNAME_NULL))]
        [MinLength(6, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        public string UserName { get; set; }

        /// <summary>
        /// Password in MD5 encrypted
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_PASSWORD_NULL))]
        public string Password { get; set; }

        //[Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_EMAIL_NULL))]
        //[MinLength(6, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        //[MaxLength(50, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        //[EmailAddress]
        public string Email { get; set; }

        public string Birthday { get; set; }

        public string Sex { get; set; }

        public string Address { get; set; }

        public string Full_Name { get; set; }

        public string Display_Name { get; set; }

        public string CMTND { get; set; }

        //[Phone(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_PHONE_INVALID))]
        //[Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_PHONENUMBER_NULL))]
        public string Phone { get; set; }

        [MaxLength(1000, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_OVER_1000_CHARACTERS))]
        public string Note { get; set; }

        /// <summary>
        /// Calling API time format: yyyyMMddHHmmss
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }

        public bool IsEmail { get; set; }

        public bool IsPhoneNumber { get; set; }
    }

    public class ApiChangePasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }

        /// <summary>
        /// Password in MD5 encrypted
        /// </summary>
        public string OldPwd1 { get; set; }

        /// <summary>
        /// Password in MD5 encrypted
        /// </summary>
        public string OldPwd2 { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_PWDTYPE_NULL))]
        public string PwdType { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_PASSWORD_NULL))]
        public string NewPwd { get; set; }

        /// <summary>
        /// Calling API time format: yyyyMMddHHmmss
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Token { get; set; }
    }

    public class ApiUpdateProfileModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }

        /// <summary>
        /// Calling API time format: yyyyMMddHHmmss
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Token { get; set; }

        public string FieldName { get; set; }

        public string FieldValue { get; set; }

        //public string Full_Name { get; set; }

        //public string Display_Name { get; set; }

        //public string Phone { get; set; }

        //public string Birthday { get; set; }

        //public string Sex { get; set; }

        //public string Address { get; set; }

        //[MaxLength(1000, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_OVER_1000_CHARACTERS))]
        //public string Note { get; set; }

    }

    public class ApiChangeAuthMethodModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Token { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_OTPTYPE_NULL))]
        public string AuthMethod { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }
    }

    public class ApiRecoverPasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_PASSWORD_NULL))]
        [MinLength(6, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        public string Password { get; set; }

        [Compare("Password", ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_CONFIRMPWD_INCORRECT))]
        [MinLength(6, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        public string RePassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_PWDTYPE_NULL))]
        public string PwdType { get; set; }

        //EMAIL, OTPSMS, ODPSMS, OTPAPP, ODPAPP or ID of security question
        public string QuestionId { get; set; }

        //EMAIL, OTPSMS, ODPSMS, OTPAPP, ODPAPP or ID of security question
        public string Answer { get; set; }

        /// <summary>
        /// Hash value: MD5(<MD5(password)>|<MD5(repassword)>|<time>|< pwdType>|<questionID>)
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_HASH_NULL))]
        public string Hash { get; set; }

        //[Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }
    }

    public class ApiTopTravellerModel
    {
        //string UserId, string Tokenkey, int NumberTop
        public int NumberTop { get; set; }
        public int UserId { get; set; }
        public string Tokenkey { get; set; }
    }

    public class ApiUserActionModel
    {
        public int UserId { get; set; }

        public int UserActionId { get; set; }

        public string TokenKey { get; set; }

        public string ActionType { get; set; }

        public int Status { get; set; } 
    }

    public class ApiActiveAccountModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERNAME_NULL))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string ActiveMethod { get; set; }

        public string OTPCode { get; set; }

        public string HashingData { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }
    }
}