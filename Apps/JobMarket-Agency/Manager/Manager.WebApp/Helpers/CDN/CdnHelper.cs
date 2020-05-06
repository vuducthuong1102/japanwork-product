using Manager.WebApp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Helpers
{
    public class CoreCdnHelper
    {
        public static string GetLinkContent()
        {
            try
            {
                return string.Format("{0}/{1}", SystemSettings.CoreContainerServer, SystemSettings.CoreCdnReadContentLink);
            }
            catch
            {
                return SystemSettings.CoreContainerServer;
            }
        }

        public static string GetFullImgPath(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.Contains("http:") || url.Contains("https:"))
                {
                    return url;
                }
            }

            var baseUrl = GetLinkContent();

            try
            {
                if (!string.IsNullOrEmpty(url))
                    url = url.Replace(baseUrl, string.Empty);

                return string.Format("{0}{1}", baseUrl, url);
            }
            catch
            {
                return url;
            }
        }
    }

    public class SocialCdnHelper
    {
        public static string GetLinkContent()
        {
            try
            {
                return string.Format("{0}/{1}", SystemSettings.SocialContainerServer, SystemSettings.SocialCdnReadContentLink);
            }
            catch
            {
                return SystemSettings.CoreContainerServer;
            }
        }

        public static string GetFullImgPath(string url)
        {          
            if (!string.IsNullOrEmpty(url))
            {
                if (url.Contains("http:") || url.Contains("https:"))
                {
                    return url;
                }
            }

            var baseUrl = GetLinkContent();

            if (baseUrl == "/")
                return "/" + url;
            try
            {
                if (!string.IsNullOrEmpty(url))
                    url = url.Replace(baseUrl, string.Empty);

                return string.Format("{0}{1}", baseUrl, url);
            }
            catch
            {
                return url;
            }
        }

        public static string GetLocalLink(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.Contains("http:") || url.Contains("https:"))
                {
                    return url;
                }
            }

            var baseUrl = GetBaseUrl();
            try
            {
                if (!string.IsNullOrEmpty(url))
                    url = url.Replace(baseUrl, string.Empty);

                return string.Format("{0}{1}", baseUrl, url);
            }
            catch
            {
                return url;
            }
        }

        public static string GetAvatarOrDefault(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return GetLocalLink("Content/Avatars/messenger-avatar.png");
            }

            if (url.Contains("http:") || url.Contains("https:"))
            {
                return url;
            }

            var baseUrl = GetBaseUrl();
            try
            {
                if (!string.IsNullOrEmpty(url))
                    url = url.Replace(baseUrl, string.Empty);

                return string.Format("{0}{1}", baseUrl, url);
            }
            catch
            {
                return url;
            }
        }
        public static string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }
    }
}