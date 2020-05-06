
using Manager.SharedLibs;
using System;
using System.Configuration;

namespace Manager.WebApp.Settings
{
    public class SystemSettings
    {
        public static string EncryptKey
        {
            get
            {
                return ConfigurationManager.AppSettings["System:EncryptKey"];
            }
        }

        public static string EmailStorageFolder 
        {
            get
            {
                return ConfigurationManager.AppSettings["System:EmailStorageFolder"];
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

        public static string GenerateTokenSecretKey
        {
            get
            {
                return ConfigurationManager.AppSettings["System:GenerateTokenSecretKey"];
            }
        }

        public static bool RestrictedCopyURL
        {
            get
            {
                return Utils.ConvertToBoolean(ConfigurationManager.AppSettings["System:RestrictedCopyURL"]);
            }
        }

        public static bool MasterCategoryCachingEnable
        {
            get
            {
                return Utils.ConvertToBoolean(ConfigurationManager.AppSettings["System:MasterCategoryCachingEnable"]);
            }
        }

        public static int DefaultCachingTimeInMinutes
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["System:DefaultCachingTimeInMinutes"]);
            }
        }

        public static string CustomerSingleSignOnApi
        {
            get
            {
                return ConfigurationManager.AppSettings["System:CustomerSingleSignOnApi"];
            }
        }

        public static string MainApi
        {
            get
            {
                return ConfigurationManager.AppSettings["System:MainApi"];
            }
        }

        public static string AdminServer
        {
            get
            {
                return ConfigurationManager.AppSettings["System:AdminServer"];
            }
        }

        public static bool ReadFileDirectly
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["ReadFileDirectly"]);
            }
        }

        public static string MediaFileUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["MediaFileUrl"];
            }
        }

        public static string FileManagerServer
        {
            get
            {
                return ConfigurationManager.AppSettings["System:FileManagerServer"];
            }
        }

        public static string CultureKey
        {
            get
            {
                return ConfigurationManager.AppSettings["System:CultureKey"];
            }
        }
       

        public static string FrontendUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["System:FrontendUrl"];
            }
        }

        public static int DefaultPageSize
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:DefaultPageSize"]);
            }
        }

        public static int ExternalServiceTimeout
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:ExternalServiceTimeout"]);
            }
        }

        public static string VietNamDateTimeFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["System:VietNamDateTimeFormat"].ToString();
            }
        }

        public static int ExtenalSeviceTimeOutInSeconds
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:ExtenalSeviceTimeOutInSeconds"]);
            }
        }

        public static string MyCloudServer
        {
            get
            {
                return ConfigurationManager.AppSettings["System:MyCloudServer"].ToString();
            }
        }

        public static string MessengerCloud
        {
            get
            {
                return ConfigurationManager.AppSettings["System:MessengerCloud"].ToString();
            }
        }

        public static string EmailFromAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailFromAddress"].ToString();
            }
        }

        public static string MailAuthUser
        {
            get
            {
                return ConfigurationManager.AppSettings["MailAuthUser"].ToString();
            }
        }

        public static string EmailConfirmSubject
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailConfirmSubject"].ToString();
            }
        }

        public static int CacheExpireDataInDashBoard
        {
            get {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:CacheExpireDataInDashBoard"]);
            }
        }

        public static string CommonCacheKeyPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["System:CommonCacheKeyPrefix"];
            }
        }

        public static string CoreContainerServer
        {
            get
            {
                return ConfigurationManager.AppSettings["CoreCdn:ContainerServer"];
            }
        }

        public static string CoreCdnReadContentLink
        {
            get
            {
                return ConfigurationManager.AppSettings["CoreCdn:CdnReadContentLink"];
            }
        }

        public static string SocialContainerServer
        {
            get
            {
                return ConfigurationManager.AppSettings["CDN:SocialFileServer"];
            }
        }

        public static string SocialCdnReadContentLink
        {
            get
            {
                return ConfigurationManager.AppSettings["CDN:SocialMediaUrl"];
            }
        }
    }

    public class ImageSettings
    {        
        public static int AvatarWidth
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["Img:AvatarWidth"]);
            }
        }

        public static int AvatarHeight
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["Img:AvatarHeight"]);
            }
        }

        public static string AvatarFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["Img:AvatarFolder"];
            }
        }

        public static int MaxFileSizeUploadLength
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["Img:MaxFileSizeUploadLength"]);
            }
        }
    }

    public class MapSettings
    {
        public static string DefaultLatitude
        {
            get
            {
                return ConfigurationManager.AppSettings["Map:DefaultLatitude"];
            }
        }

        public static string DefaultLongitude
        {
            get
            {
                return ConfigurationManager.AppSettings["Map:DefaultLongitude"];
            }
        }


        public static int DefaultZoomSize
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["Map:DefaultZoomSize"]);
            }
        }
    }
}