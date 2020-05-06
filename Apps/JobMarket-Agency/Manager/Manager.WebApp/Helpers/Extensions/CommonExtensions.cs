using Manager.SharedLibs.Logging;
using Manager.WebApp.Models;
using Newtonsoft.Json;

namespace Manager.WebApp.Helpers
{
    public static class ApiExtensions
    {
        public static bool HasData(this ApiResponseCommonModel res)
        {
            if (res != null && res.value != null)
                return true;

            return false;
        }
        
        public static T ConvertData<T>(this ApiResponseCommonModel res)
        {
            try
            {
                if (res.HasData())
                {
                    var myObj = JsonConvert.DeserializeObject<T>(res.value.ToString());
                    return myObj;
                }                
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return default(T);
        }

        //public static T GetUserCookie<T>(string key)
        //{
        //    string value = string.Empty;
        //    HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

        //    if (cookie != null)
        //    {
        //        // For security purpose, we need to encrypt the value.
        //        //HttpCookie decodedCookie = HttpSecureCookie.Decode(cookie);               
        //        try
        //        {
        //            //var decryptedStr = Base64Decode(cookie.Value);
        //            //var myObj = JsonConvert.DeserializeObject<T>(decryptedStr);
        //            //return myObj;

        //            var myObj = DecodeDataFromToken<T>(cookie.Value);
        //            return myObj;
        //        }
        //        catch
        //        {
        //            ClearCookie(key);
        //            return default(T);
        //        }
        //    }

        //    return default(T);
        //}
    }
}