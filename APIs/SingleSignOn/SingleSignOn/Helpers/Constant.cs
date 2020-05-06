using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SingleSignOn.Helpers
{
    public class Constant
    {
        public static string DATEFORMAT_yyyyMMddHHmmss = "yyyyMMddHHmmss";
        public static string DATEFORMAT_ddMMyyyyHHmmss = "ddMMyyyyHHmmss";
        public static string DATE_FORMAT_ddMMyyyy = "dd/MM/yyyy";

        public const string PREFIX_WEB_LOGIN_MODEL_REDIS_KEY = "WEB_LOGIN_MODEL";
        public const string PREFIX_API_LOGIN_MODEL_REDIS_KEY = "API_LOGIN_MODEL";

    }

    public class ActionMethod
    {
        public static string Api = "API";
        public static string Web = "WEB";
    }

    public class ActionType
    {
        public static string Login = "login";
        public static string Register = "register";
        public static string RefreshToken = "refresh_token";
        public static string CreateOTP = "create_otp";
        public static string VerifyOTP = "verify_otp";
        public static string ChangePassword1 = "changepwd1";
        public static string ChangePassword2 = "changepwd2";
        public static string UpdateProfile = "update_profile";
        public static string ChangeAuthMethod = "changeauthmethod";
        public static string RecoverPassword1 = "recover_password1";
        public static string RecoverPassword2 = "recover_password2";
        public static string ActiveAccount = "active_account";
        public static string ResendEmail = "resendemail";
    }

    public class OTPType
    {
        public static string OTPAPP = "OTPAPP";        
        public static string OTPSMS = "OTPSMS";

        public static string ODPAPP = "ODPAPP";
        public static string ODPSMS = "ODPSMS";

        public static string PASSWORD2 = "PASSWORD2";
    }

    public class UserPasswordLevel
    {
        public static int Level1 = 1;
        public static int Level2 = 2;
    }

    public class PasswordLevelType
    {
        public static string Level1 = "level1";
        public static string Level2 = "level2";
    }

    public class UserSex
    {
        public static int Male = 1;
        public static int Female = 0;
    }

    public class RecoverPasswordMethod {
        //EMAIL, OTPSMS, ODPSMS, OTPAPP, ODPAPP or ID of security question
        public static string EMAIL = "EMAIL";
        public static string OTPSMS = "OTPSMS";
        public static string ODPSMS = "ODPSMS";
        public static string OTPAPP = "OTPAPP";
        public static string ODPAPP = "ODPAPP";
        public static string QUESTIONID = "ID";
    }

    public class ActiveAccountMethod
    {
        public static string Email = "email";
        public static string Phone = "phone";
    }
}