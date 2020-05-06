using System.Configuration;

namespace MySite.Settings
{
    public class AuthorizationCoreSettings
    {
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

    public class AuthorizationSocialSettings
    {
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

    public class AuthorizationMessengerSettings
    {
        public static string HeaderKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeMessenger:HeaderKey"];
            }
        }

        public static string HeaderValue
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeMessenger:HeaderValue"];
            }
        }

        public static string Username
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeMessenger:Username"];
            }
        }

        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeMessenger:Password"];
            }
        }
    }
}