//using Newtonsoft.Json;
//using MySite.Resources;
//using System.ComponentModel.DataAnnotations;
//using SingleSignOn.DB.Sql.Entities;
//using System.Collections.Generic;
//using System.Net;

//namespace MySite.Models
//{
//    public class ApiAuthLoginModel
//    {        
//        public string UserName { get; set; }

//        /// <summary>
//        /// Password in MD5 encrypted
//        /// </summary>
//        public string Password { get; set; }

//        /// <summary>
//        /// Login time format: yyyyMMddHHmmss
//        /// </summary>
//        public string Time { get; set; }

//        /// <summary>
//        /// Hash value: MD5(<username>|<password>|<time>)
//        /// </summary>
//        public string Hash { get; set; }
//    }

//    public class ResponseApiAuthLoginModel
//    {
//        public int Code { get; set; }
//        public string Msg { get; set; }

//        public string TokenKey { get; set; }
        
//        public ApiAuthUserLoginIdentity Data { get; set; }

//        public ResponseApiAuthLoginModel()
//        {
//            Data = new ApiAuthUserLoginIdentity();
//        }
//    }

//    public class RefreshTokenModel
//    {

//        /// <summary>
//        /// UserId
//        /// </summary>
//        public int UserId { get; set; }


//        /// <summary>
//        /// The time when calling api: yyyyMMddHHmmss
//        /// </summary>
//        public string Time { get; set; }

//        public string Token { get; set; }
//    }

//    public class ResponseApiModel
//    {
//        public int Code { get; set; }
//        public string Msg { get; set; }
//        public string TokenKey { get; set; }
//        public object Data { get; set; }
//    }

//    public class ApiResponseCommonModel
//    {
//        public int status { get; set; }
//        public dynamic value { get; set; }
//        public string message { get; set; }
//        public ApiResponseErrorModel error { get; set; }
//        public int total { get; set; }

//        public ApiResponseCommonModel()
//        {
//            status = (int)HttpStatusCode.OK;
//            error = new ApiResponseErrorModel();
//        }
//    }

//    public class ApiResponseErrorModel
//    {
//        public string error_code { get; set; }
//        public string message { get; set; }
//        public List<ApiResponseErrorFieldModel> field { get; set; }
//    }

//    public class ApiResponseErrorFieldModel
//    {
//        public string name { get; set; }
//        public string message { get; set; }
//    }

//    public class CreateOTPModel
//    {
//        public int UserId { get; set; }

//        public string Action { get; set; }

//        public string Time { get; set; }

//        public string Token { get; set; }
//    }

//    public class VerifyOTPModel
//    {
//        public int UserId { get; set; }

//        public string OTPCode { get; set; }

//        public string Time { get; set; }

//        public string Action { get; set; }

//        public string Token { get; set; }
//    }

//    public class LogoutModel
//    {
//        /// <summary>
//        /// UserId
//        /// </summary>
//        public int UserId { get; set; }


//        /// <summary>
//        /// The time when calling api: yyyyMMddHHmmss
//        /// </summary>
//        public string Time { get; set; }

//        public string Token { get; set; }
//    }

//    public class ApiCheckUserExistsModel
//    {
//        /// <summary>
//        /// UserId
//        /// </summary>
//        public string UserName { get; set; }


//        /// <summary>
//        /// The time when calling api: yyyyMMddHHmmss
//        /// </summary>
//        public string Time { get; set; }

//        public string Token { get; set; }
//    }

//    public class ApiGetUserInfoModel
//    {
//        /// <summary>
//        /// UserId
//        /// </summary>
//        public int UserId { get; set; }


//        /// <summary>
//        /// The time when calling api: yyyyMMddHHmmss
//        /// </summary>
//        public string Time { get; set; }

//        public string Token { get; set; }
//    }

//    public class ApiCheckPwd2IsValidModel
//    {
//        /// <summary>
//        /// UserId
//        /// </summary>
//        public int UserId { get; set; }

//        public string Pwd { get; set; }
//        /// <summary>
//        /// The time when calling api: yyyyMMddHHmmss
//        /// </summary>
//        public string Time { get; set; }

//        public string Token { get; set; }
//    }

//    public class ApiGetUserByEmailModel
//    {
//        /// <summary>
//        /// UserId
//        /// </summary>
//        public string Email { get; set; }
//    }

//    public class ApiGetUserByPhoneNumberModel
//    {
//        /// <summary>
//        /// UserId
//        /// </summary>
//        public string PhoneNumber { get; set; }
//    }

//    public class ApiChangePassword1Model
//    {
//        /// <summary>
//        /// UserId
//        /// </summary>
//        public int UserId { get; set; }

//        /// <summary>
//        /// NewPassword
//        /// </summary>
//        public string NewPassword { get; set; }

//        public string PasswordType { get; set; }
//    }

//    public class ApiChangePassword2Model
//    {
//        /// <summary>
//        /// UserId
//        /// </summary>
//        public int UserId { get; set; }

//        /// <summary>
//        /// PasswordType
//        /// </summary>
//        public string PasswordType { get; set; }
//    }

//    public class ApiRecoverPasswordOTPModel
//    {
//        /// <summary>
//        /// UserId
//        /// </summary>
//        public string PhoneNumber { get; set; }

//        /// <summary>
//        /// NewPassword
//        /// </summary>
//        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_PASSWORD_NULL))]
//        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        public string NewPassword { get; set; }

//        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
//        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
//        [Compare("NewPassword", ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_CONFIRMPWD_INCORRECT))]
//        public string ConfirmPassword { get; set; }

//        public string PasswordLevel { get; set; }
//    }
//}