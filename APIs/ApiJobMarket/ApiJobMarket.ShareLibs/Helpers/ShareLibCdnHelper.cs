using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJobMarket.ShareLibs
{
    public class ShareLibCdnHelper
    {
        public static string GetLinkContent()
        {
            var containerServer = ConfigurationManager.AppSettings["Img:SocialContainerServer"];
            var readContentLink = ConfigurationManager.AppSettings["Img:SocialCdnReadContentLink"];
            try
            {
                return string.Format("{0}/{1}", containerServer, readContentLink);
            }
            catch
            {
                return containerServer;
            }
        }

        public static string GetFullImgPath(string url)
        {
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
