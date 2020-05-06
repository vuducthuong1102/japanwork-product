﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Helpers
{
    public static class UrlExtensions
    {
        public static string ResolveServerUrl(string serverUrl, bool forceHttps)
        {
            if (serverUrl.IndexOf("://") > -1)
                return serverUrl;

            string newUrl = serverUrl;
            Uri originalUri = System.Web.HttpContext.Current.Request.Url;
            newUrl = (forceHttps ? "https" : originalUri.Scheme) +
                "://" + originalUri.Authority + newUrl;
            return newUrl;
        }

        /// <summary>
        /// Converts the provided app-relative path into an absolute Url containing the 
        /// full host name
        /// </summary>
        /// <param name="relativeUrl">App-Relative path</param>
        /// <returns>Provided relativeUrl parameter as fully qualified Url</returns>
        /// <example>~/path/to/foo to http://www.web.com/path/to/foo</example>
        public static string ToAbsoluteUrl(this string relativeUrl)
        {
            
            if (string.IsNullOrEmpty(relativeUrl))
                return relativeUrl;
            else
            {
                if (!relativeUrl.Contains("/"))
                {
                    relativeUrl = "/" + relativeUrl;

                    return relativeUrl;
                }
            }

            if (HttpContext.Current == null)
                return relativeUrl;

            if (relativeUrl.StartsWith("/"))
                return relativeUrl;

            if (relativeUrl.StartsWith("~/"))
                return relativeUrl;

            //var url = HttpContext.Current.Request.Url;
            //var port = url.Port != 80 ? (":" + url.Port) : String.Empty;

            ////return String.Format("{0}://{1}{2}{3}",
            ////    url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));

            //return String.Format("{0}",
            //   VirtualPathUtility.ToAbsolute(relativeUrl));

            var uri = new Uri(relativeUrl);
            if (uri != null)
                relativeUrl = uri.AbsolutePath;

            return relativeUrl;
        }
    }
}