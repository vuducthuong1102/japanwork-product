using System;
using System.Web;
using SingleSignOn.Logging;
using SingleSignOn.Helpers;
using Newtonsoft.Json;
using System.Text;
using System.Web.Security;
using SingleSignOn.Settings;
using SingleSignOn.DB.Sql.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace SingleSignOn.Caching
{
    public class UserCookieManager
    {
        private static readonly ILog logger = LogProvider.For<UserCookieManager>();
        public static bool IsAuthenticated(string cookieKey = "")
        {
            if (string.IsNullOrEmpty(cookieKey)) cookieKey = SingleSignOnSettings.SSOCommonUserKey;
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
            //var encryptedStr = Base64Encode(myObjectJson);
            var encryptedStr = GenerateTokenFromData(data);

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
                    //var decryptedStr = Base64Decode(cookie.Value);
                    //var myObj = JsonConvert.DeserializeObject<T>(decryptedStr);
                    //return myObj;

                    var myObj = DecodeDataFromToken<T>(cookie.Value);
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

        public static string GenerateTokenFromData(object myData)
        {
            var tokenString = string.Empty;
            try
            {
                // Define const Key this should be private secret key  stored in some safe place
                string key = SystemSettings.EncryptKey;

                // Create Security key  using private key above:
                // not that latest version of JWT using Microsoft namespace instead of System
                var securityKey = new Microsoft
                   .IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                // Also note that securityKey length should be >256b
                // so you have to make sure that your private key has a proper length
                //
                var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
                                  (securityKey, SecurityAlgorithms.HmacSha256Signature);

                //  Finally create a Token
                var header = new JwtHeader(credentials);

                //Some PayLoad that contain information about the  customer
                var payload = new JwtPayload
                {
                    { "Data", JsonConvert.SerializeObject(myData) }
                };

                //
                var secToken = new JwtSecurityToken(header, payload);
                //var secToken = new JwtSecurityToken(header, userInfo);
                var handler = new JwtSecurityTokenHandler();

                // Token to String so you can use it in your client
                tokenString = handler.WriteToken(secToken);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not GenerateTokenFromData because: {0}", ex.ToString()));
            }

            return tokenString;
        }

        public static T DecodeDataFromToken<T>(string tokenString)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(tokenString);
               
                if (token != null)
                {
                     var jsonData = token.Payload.First().Value.ToString();
                    var myObj = JsonConvert.DeserializeObject<T>(jsonData);
                    return myObj;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not DecodeDataFromToken because: {0}", ex.ToString()));
                return default(T);
            }

            return default(T);
        }
    }
}