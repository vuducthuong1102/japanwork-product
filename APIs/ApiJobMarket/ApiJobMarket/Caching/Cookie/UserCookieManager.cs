using System;
using System.Web;
using ApiJobMarket.Logging;
using ApiJobMarket.Helpers;
using Newtonsoft.Json;
using System.Text;
using System.Web.Security;
using ApiJobMarket.Settings;

namespace ApiJobMarket.Caching
{
    public class UserCookieManager
    {
        private static readonly ILog logger = LogProvider.For<UserCookieManager>();
        public static bool IsAuthenticated(string cookieKey = "")
        {
            if (string.IsNullOrEmpty(cookieKey)) cookieKey = ApiJobMarketSettings.SSOCommonUserKey;
            var myObjStr = GetCookie<object>(cookieKey);
            if (myObjStr != null)
            {
                return true;
            }

            return false;
        }

        public static void SetCookie(string key, object data, int expireInMinutes = 30)
        {
            string myObjectJson = JsonConvert.SerializeObject(data);
            var encryptedStr = Base64Encode(myObjectJson);

            HttpCookie encodedCookie = new HttpCookie(key, encryptedStr);

            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[key];
                cookieOld.Expires = DateTime.Now.AddMinutes(expireInMinutes);
                cookieOld.Value = encodedCookie.Value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                encodedCookie.Expires = DateTime.Now.AddMinutes(expireInMinutes);
                HttpContext.Current.Response.Cookies.Add(encodedCookie);
            }

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        //public static string GetCookie(string key)
        //{
        //    string value = string.Empty;
        //    HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

        //    if (cookie != null)
        //    {
        //        // For security purpose, we need to encrypt the value.
        //        //HttpCookie decodedCookie = HttpSecureCookie.Decode(cookie);
        //        var decryptedStr = Base64Decode(cookie.Value);
        //        return decryptedStr;
        //    }
        //    return null;
        //}

        public static T GetCookie<T>(string key)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

            if (cookie != null)
            {
                // For security purpose, we need to encrypt the value.
                //HttpCookie decodedCookie = HttpSecureCookie.Decode(cookie);               
                try
                {
                    var decryptedStr = Base64Decode(cookie.Value);
                    var myObj = JsonConvert.DeserializeObject<T>(decryptedStr);
                    return myObj;
                }
                catch
                {
                    ClearCookie(key);
                    return default(T);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Deletes a cookie with specified name
        /// </summary>
        /// <param name="controller">extends the controller</param>
        /// <param name="cookieName">cookie name</param>
        public static bool ClearCookie(string cookieName)
        {
            var c = new HttpCookie(cookieName)
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            HttpContext.Current.Response.Cookies.Add(c);

            return true;
        }
    }
}