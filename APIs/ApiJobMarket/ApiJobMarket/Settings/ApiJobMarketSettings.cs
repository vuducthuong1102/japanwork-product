using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;

namespace ApiJobMarket.Settings
{
    public class ApiJobMarketSettings
    {
        public static string SSOCommonUserKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOCommonUserKey"];
            }
        }

        public static string AllowedIPAddresses
        {
            get
            {
                return ConfigurationManager.AppSettings["AllowedIPAddresses"];
            }
        }

        public static string AllowedCorsOrigins
        {
            get
            {
                return ConfigurationManager.AppSettings["AllowedCorsOrigins"];
            }
        }

        public static long DefaultCachedTimeout
        {
            get
            {
                return Convert.ToInt64(ConfigurationManager.AppSettings["DefaultCachedTimeout"]);
            }
        }

        public static string ApiJobMarketDB
        {
            get
            {
                return ConfigurationManager.AppSettings["JobMarketDB"];
            }
        }

        public static bool AllowToTraceLoginRequest
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["AllowToTraceLoginRequest"]);
            }
        }

        public static bool AllowToTraceHttpMiddleWare
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["AllowToTraceHttpMiddleWare"]);
            }
        }

        public static bool MDS_EnableTracking
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["MDS_EnableTracking"]);
            }
        }

        public static string MDS_FromAppName
        {
            get
            {
                return ConfigurationManager.AppSettings["MDS_FromAppName"];
            }
        }

        public static string MDS_FromSrvName
        {
            get
            {
                return ConfigurationManager.AppSettings["MDS_FromSrvName"];
            }
        }

        public static string MDS_DestAppName
        {
            get
            {
                return ConfigurationManager.AppSettings["MDS_DestAppName"];
            }
        }

        public static string MDS_DestSrvName
        {
            get
            {
                return ConfigurationManager.AppSettings["MDS_DestSrvName"];
            }
        }

        public static string MDS_ServerHost
        {
            get
            {
                return ConfigurationManager.AppSettings["MDS_ServerHost"];
            }
        }

        public static int MDS_ServerPort
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["MDS_ServerPort"]);
            }
        }

        public static int NumberOfFailedLoginsToShowCaptcha
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfFailedLoginsToShowCaptcha"]);
            }
        }

        public static int ApiLoginTimeValidInHours
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ApiLoginTimeValidInHours"]);
            }
        }

        public static string CharactersOfOTPCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CharactersOfOTPCode"];
            }
        }

        public static int NumberCharactersOfOTPCode
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["NumberCharactersOfOTPCode"]);
            }
        }
    }

    public class ImageSettings
    {
        public static int AvatarWidth
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["Img:AvatarWidth"]);
            }
        }

        public static int AvatarHeight
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["Img:AvatarHeight"]);
            }
        }

        public static string AvatarFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["Img:AvatarFolder"];
            }
        }
    }
}