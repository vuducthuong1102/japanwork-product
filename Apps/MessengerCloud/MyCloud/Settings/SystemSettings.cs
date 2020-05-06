using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MyCloud.Settings
{
    public class SystemSettings
    {
        public static string MainApi
        {
            get
            {
                return ConfigurationManager.AppSettings["System:MainApi"];
            }
        }

        public static string SingleSignOnApi
        {
            get
            {
                return ConfigurationManager.AppSettings["System:SingleSignOnApi"];
            }
        }

        public static double ExternalServiceTimeout
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["System:ExternalServiceTimeout"]);
            }
        }

        public static int DefaultCachedTimeout
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:DefaultCachedTimeout"]);
            }
        }

        public static string DefaultCacheKeyPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["System:DefaultCacheKeyPrefix"].ToString();
            }
        }
    }
}