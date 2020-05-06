//using MySite.Helpers;
//using MySite.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace MySite.Caching
//{
//    public class UserSessionManager1
//    {
//        private static readonly ILog logger = LogProvider.For<UserSessionManager1>();
//        public static bool IsAuthenticated(string sessionKey = "")
//        {
//            if (string.IsNullOrEmpty(sessionKey)) sessionKey = Constant.WEB_USER_LOGIN_SESSION_KEY;

//            if (HttpContext.Current.Session[Constant.WEB_USER_LOGIN_SESSION_KEY] != null)
//            {
//                return true;
//            }

//            return false;
//        }

//        public static object GetSessionData(string sessionKey = "")
//        {
//            if (string.IsNullOrEmpty(sessionKey)) sessionKey = Constant.WEB_USER_LOGIN_SESSION_KEY;
            
//            try
//            {
//                var myObj = HttpContext.Current.Session[sessionKey];
//                return myObj;
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Could not ParsingUserData from session due to: {0}", ex.Message);
//                logger.Error(strError);

//                return null;
//            }            
//        }

//        public static void ClearSessionData(string sessionKey = "")
//        {
//            if (string.IsNullOrEmpty(sessionKey)) sessionKey = Constant.WEB_USER_LOGIN_SESSION_KEY;

//            try
//            {
//                HttpContext.Current.Session.Clear();
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Could not Clear session data due to: {0}", ex.Message);
//                logger.Error(strError);
//            }
//        }
//    }
//}