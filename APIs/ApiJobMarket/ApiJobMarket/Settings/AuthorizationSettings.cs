using ApiJobMarket.ShareLibs;
using System.Configuration;

namespace ApiJobMarket.Settings
{
    public class AuthorizationSingleSignOnSettings
    {
        public static string HeaderKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSingleSignOn:HeaderKey"];
            }
        }

        public static string HeaderValue
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSingleSignOn:HeaderValue"];
            }
        }

        public static string Username
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSingleSignOn:Username"];
            }
        }

        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSingleSignOn:Password"];
            }
        }
    }

    public class AuthorizationSettings
    {
        public static bool Required
        {
            get
            {
                return Utils.ConvertToBoolean(ConfigurationManager.AppSettings["AuthorizeSocial:Required"], false);
            }
        }

        public static string HeaderKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSocial:HeaderKey"];
            }
        }

        public static string HeaderValue
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSocial:HeaderValue"];
            }
        }

        public static string Username
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSocial:Username"];
            }
        }

        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSocial:Password"];
            }
        }
    }
}