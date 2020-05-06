using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using SingleSignOn.ShareLibs;

namespace SingleSignOn.Settings
{
    public class AWSSettings
    {
        #region

        public static bool AWS_SES_Enabled
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["AWS-SES-Enabled"]);
            }
        }

        public static string AWS_SES_EmailDisplayName
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS-SES-EmailDisplayName"].ToString();
            }
        }

        public static string AWS_SES_Email
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS-SES-Email"].ToString();
            }
        }

        public static string AWS_SES_AccessKeyId
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS-SES-AccessKeyId"].ToString();
            }
        }

        public static string AWS_SES_SecrectAccessKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS-SES-SecrectAccessKey"].ToString();
            }
        }

        #endregion
    }

    public class SystemSettings
    {       

        public static string EncryptKey
        {
            get
            {
                return ConfigurationManager.AppSettings["System:EncryptKey"];
            }
        }

        public static string EmailSender
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_Sender"];
            }
        }

        public static string EmailSenderPwd
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_SenderPwd"];
            }
        }

        public static string EmailHost
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_Host"];
            }
        }

        public static string EmailServerPort
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_ServerPort"];
            }
        }

        public static bool EmailIsUseSSL
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["System:Email_IsUseSSL"]);
            }
        }

        public static string Email_ActiveLink
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_ActiveLink"].ToString();
            }
        }

        public static bool IsLogParamater
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["System:IsLogParamater"]);
            }
        }

        public static string SMSServiceUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["System:SMSUrl"];
            }
        }

        public static string SendEmailServiceUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["System:SendEmailServiceUrl"];
            }
        }

        public static double ExternalServiceTimeout
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["System:ExternalServiceTimeout"]);
            }
        }

        public static string CommonCacheKeyPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["System:CommonCacheKeyPrefix"];
            }
        }

        public static string MediaFileFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["System:MediaFileFolder"];
            }
        }

        public static string FrontendURL
        {
            get
            {
                return ConfigurationManager.AppSettings["System:FrontendURL"];
            }
        }
    }

    public class AuthorizationSettings
    {
        public static bool Required
        {
            get
            {
                return Utils.ConvertToBoolean(ConfigurationManager.AppSettings["AuthorizeCore:Required"], false);
            }
        }

        public static string HeaderKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeCore:HeaderKey"];
            }
        }

        public static string HeaderValue
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeCore:HeaderValue"];
            }
        }

        public static string Username
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeCore:Username"];
            }
        }

        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeCore:Password"];
            }
        }
    }
}