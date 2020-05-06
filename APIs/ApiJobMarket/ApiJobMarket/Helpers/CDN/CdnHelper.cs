﻿using ApiJobMarket.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiJobMarket.Helpers
{
    public class CdnHelper
    {
        //public static string CoreGetLinkContent()
        //{
        //    try
        //    {
        //        return string.Format("{0}/{1}", CDNSettings.CoreFileReadingServer, CDNSettings.CoreImgReadingUrl);
        //    }
        //    catch
        //    {
        //        return CDNSettings.CoreFileReadingServer;
        //    }
        //}

        //public static string CoreGetFullImgPath(string url)
        //{
        //    var baseUrl = CoreGetLinkContent();

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(url))
        //            url = url.Replace(baseUrl, string.Empty);

        //        return string.Format("{0}{1}", baseUrl, url);
        //    }
        //    catch
        //    {
        //        return url;
        //    }
        //}

        public static string SocialGetLinkContent()
        {
            try
            {
                return string.Format("{0}/{1}", CDNSettings.SocialFileReadingServer, CDNSettings.SocialImgReadingUrl);
            }
            catch
            {
                return CDNSettings.CoreFileReadingServer;
            }
        }

        public static string SocialGetFullImgPath(string url)
        {
            var baseUrl = SocialGetLinkContent();
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    if (url.Contains("http://"))
                    {
                        return url;
                    }
                }

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