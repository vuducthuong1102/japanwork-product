using Newtonsoft.Json;
using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SingleSignOn.Models
{
    public class ApiAuthLoginModel
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

        /// <summary>
        /// Login time format: yyyyMMddHHmmss
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }

        /// <summary>
        /// Hash value: MD5(<username>|<password>|<time>)
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_HASH_NULL))]
        public string Hash { get; set; }


        /// <summary>
        /// Social is string
        /// </summary>
        public string SocialProvider { get; set; }
    }

    public class ApiAuthLoginWithModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERNAME_NULL))]
        [MinLength(6, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        public string UserName { get; set; }
        /// <summary>
        /// Social is string
        /// </summary>
        /// 
        public string Email { get; set; }

        public string SocialProvider { get; set; }

        public string DisplayName { get; set; }

        public string AppCode { get; set; }
    }
    public class ResponseApiAuthLoginModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }

        public string TokenKey { get; set; }
        
        public ApiAuthUserLoginIdentity Data { get; set; }

        public ResponseApiAuthLoginModel()
        {
            Data = new ApiAuthUserLoginIdentity();
        }
    }

    public class RefreshTokenModel
    {

        /// <summary>
        /// UserId
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }


        /// <summary>
        /// The time when calling api: yyyyMMddHHmmss
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Token { get; set; }
    }

    public class ResponseApiModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string TokenKey { get; set; }
        public dynamic Data { get; set; }
    }

    public class CreateOTPModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Action { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }
    }

    public class CreateEmailModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Action { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }
    }

    public class VerifyOTPModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_OTPCODE_NULL))]
        public string OTPCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_OTPACTION_NULL))]
        public string Action { get; set; }

        //[Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Token { get; set; }
    }

    public class LogoutModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }


        /// <summary>
        /// The time when calling api: yyyyMMddHHmmss
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Token { get; set; }
    }

    public class ApiCheckUserExistsModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERNAME_NULL))]
        public string UserName { get; set; }


        /// <summary>
        /// The time when calling api: yyyyMMddHHmmss
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Token { get; set; }
    }

    public class ApiCheckUserTokenModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }

        /// <summary>
        /// The time when calling api: yyyyMMddHHmmss
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Token { get; set; }
    }

    public class ApiGetUserInfoModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }


        /// <summary>
        /// The time when calling api: yyyyMMddHHmmss
        /// </summary>
       // [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }

       // [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Token { get; set; }
    }

    public class ApiGetUserByEmailModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_EMAIL_NULL))]
        public string Email { get; set; }
    }

    public class ApiListUserInfoModel
    {
        /// <summary>
        /// ListUserId
        /// </summary>
        public List<int> ListUserId { get; set; }
    }

    public class ApiChangePassword1Model
    {
        /// <summary>
        /// UserId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// NewPassword
        /// </summary>
        public string NewPassword { get; set; }

        public string PasswordType { get; set; }
    }

    public class ApiChangePassword2Model
    {
        /// <summary>
        /// UserId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// PasswordType
        /// </summary>
        public string PasswordType { get; set; }
    }

    public class ApiRecoverPasswordOTPModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// NewPassword
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_PASSWORD_NULL))]
        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        public string NewPassword { get; set; }

        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_CONFIRMPWD_INCORRECT))]
        public string ConfirmPassword { get; set; }

        public string PasswordLevel { get; set; }
    }

    public class ApiCheckPwd2IsValidModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_PASSWORD_NULL))]
        public string Pwd { get; set; }
        /// <summary>
        /// The time when calling api: yyyyMMddHHmmss
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_TIME_NULL))]
        public string Time { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string Token { get; set; }
    }
}