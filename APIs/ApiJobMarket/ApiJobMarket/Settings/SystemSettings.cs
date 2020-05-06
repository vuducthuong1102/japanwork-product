using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using ApiJobMarket.ShareLibs;

namespace ApiJobMarket.Settings
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
        public static string SingleSignOnApi
        {
            get
            {
                return ConfigurationManager.AppSettings["System:SingleSignOnApi"];
            }
        }

        public static int DefaultPageSize
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["System:DefaultPageSize"]);
            }
        }

        public static int MaximumVideoUploadLengthInMB
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["System:MaximumVideoUploadLengthInMB"]);
            }
        }

        public static int MaxPageSize
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["System:MaxPageSize"]);
            }
        }

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

        public static string Email_AccepFriendInvitationLink
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_AccepFriendInvitationLink"].ToString();
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

        public static int UserCachingTime
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["System:UserCachingTime"]);
            }
        }

        public static string DomainShare
        {
            get
            {
                return ConfigurationManager.AppSettings["System:DomainShare"];
            }
        }

        public static int DefaultCachingTimeInMinutes
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["System:DefaultCachingTimeInMinutes"]);
            }
        }
    }

    public class CDNSettings
    {
        public static string CoreFileReadingServer
        {
            get
            {
                return ConfigurationManager.AppSettings["CDN:CoreFileReadingServer"];
            }
        }

        public static string CoreImgReadingUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CDN:CoreImgReadingUrl"];
            }
        }

        public static string SocialFileReadingServer
        {
            get
            {
                return ConfigurationManager.AppSettings["CDN:SocialFileReadingServer"];
            }
        }

        public static string SocialImgReadingUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CDN:SocialImgReadingUrl"];
            }
        }
    }

    public class MyCloudSettings
    {
        public static string MyCloudServer
        {
            get
            {
                return ConfigurationManager.AppSettings["System:MyCloudServer"];
            }
        }

        public static string CommonHub
        {
            get
            {
                return ConfigurationManager.AppSettings["System:CommonHub"];
            }
        }

        public static string ManagerHub
        {
            get
            {
                return ConfigurationManager.AppSettings["System:ManagerHub"];
            }
        }
    }
}