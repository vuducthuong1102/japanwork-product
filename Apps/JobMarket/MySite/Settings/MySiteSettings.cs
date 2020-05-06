using MySite.ShareLibs;
using System;

using System.Configuration;

namespace MySite.Settings
{
    public class MySiteSettings
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

        public static string MySiteDB
        {
            get
            {
                return ConfigurationManager.AppSettings["MySiteDB"];
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
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["MDS_ServerPort"]);
            }
        }

        public static int NumberOfFailedLoginsToShowCaptcha
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["NumberOfFailedLoginsToShowCaptcha"]);
            }
        }

        public static int ApiLoginTimeValidInHours
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["ApiLoginTimeValidInHours"]);
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
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["NumberCharactersOfOTPCode"]);
            }
        }

        public static int MaxOfRatingScore
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["MaxOfRatingScore"], 5);
            }
        }
    }
}