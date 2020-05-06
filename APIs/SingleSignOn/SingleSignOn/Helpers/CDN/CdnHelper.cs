using SingleSignOn.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SingleSignOn.Helpers
{
    public class CdnHelper
    {
        public static string GetLinkContent()
        {
            try
            {
                return string.Format("{0}/{1}", ImageSettings.ImageContainerServer, ImageSettings.CdnReadContentLink);
            }
            catch
            {
                return ImageSettings.ImageContainerServer;
            }
        }

        public static string GetFullImgPath(string url)
        {
            //var baseUrl = GetLinkContent();

            //try
            //{
            //    if (!string.IsNullOrEmpty(url))
            //        url = url.Replace(baseUrl, string.Empty);

            //    return string.Format("{0}{1}", baseUrl, url);
            //}
            //catch
            //{
            //    return url;
            //}

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
}