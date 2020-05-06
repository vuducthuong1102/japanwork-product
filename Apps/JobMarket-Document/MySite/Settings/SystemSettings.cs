using MySite.ShareLibs;
using System;

using System.Configuration;

namespace MySite.Settings
{
    public class SystemSettings
    {
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

        public static string MySiteSingleSignOnApi
        {
            get
            {
                return ConfigurationManager.AppSettings["System:MySiteSingleSignOnApi"];
            }
        }

        public static string MySiteApi
        {
            get
            {
                return ConfigurationManager.AppSettings["System:MySiteApi"];
            }
        }

        public static string MySiteMessengerApi
        {
            get
            {
                return ConfigurationManager.AppSettings["System:MySiteMessengerApi"];
            }
        }

        public static int DefaultPageSize
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["System:DefaultPageSize"]);
            }
        }

        public static string CultureKey
        {
            get
            {
                return ConfigurationManager.AppSettings["System:CultureKey"];
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

        public static string SMSServiceUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["System:SMSUrl"];
            }
        }

        public static double ExternalServiceTimeout
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["System:ExternalServiceTimeout"]);
            }
        }

        public static int NotifPageSize
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["NotifPageSize"]);
            }
        }

        public static string KeyMap
        {
            get
            {
                return ConfigurationManager.AppSettings["Map:KeyMap"];
            }
        }        
    }

    public class SocialSettings
    {
        public static string FBAppId
        {
            get
            {
                return ConfigurationManager.AppSettings["Social:FBAppId"];
            }
        }

        public static string GoogleAppId
        {
            get
            {
                return ConfigurationManager.AppSettings["Social:GoogleAppId"];
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

    public class ImageSettings
    {
        public static int MaxFileSizeUploadLength
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["Img:MaxFileSizeUploadLength"]);
            }
        }
    }

    public class OpenWeatherSettings
    {
        public static string ServiceUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["OpenWeather:ServiceUrl"];
            }
        }

        public static string Id
        {
            get
            {
                return ConfigurationManager.AppSettings["OpenWeather:Id"];
            }
        }

        public static string AppId
        {
            get
            {
                return ConfigurationManager.AppSettings["OpenWeather:AppId"];
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
    }
}