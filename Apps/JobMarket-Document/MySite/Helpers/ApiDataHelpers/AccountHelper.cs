//using Autofac;
//using MySite.Caching;
//using MySite.Caching.Providers;
//using MySite.Logging;
//using MySite.Models;
//using MySite.Settings;
//using MySite.ShareLibs;
//using System;
//using MySite.Resources;
//using SingleSignOn.DB.Sql.Entities;
//using System.Web;
//using Newtonsoft.Json;

//namespace MySite.Helpers
//{
//    public class AccountHelper
//    {
//        private static readonly ILog logger = LogProvider.For<AccountHelper>();

//        public static ApiAuthUserLoginIdentity GetCurrentUser_Old()
//        {
//            try
//            {
//                var userLogin = UserCookieManager.GetUserCookie<UserCookieModel>(MySiteSettings.SSOCommonUserKey);
//                if (userLogin != null)
//                {
//                    var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
//                    var cacheKey = string.Format("{0}_{1}", "USER", userLogin.UserId);

//                    ApiAuthUserLoginIdentity userInfo = null;
//                    cacheProvider.Get<ApiAuthUserLoginIdentity>(cacheKey, out userInfo);

//                    if (userInfo != null)
//                        return userInfo;
//                    else
//                        UserCookieManager.ClearCookie(MySiteSettings.SSOCommonUserKey);
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Failed when GetCurrentUser due to {0}", ex.ToString()));
//            }

//            return null;
//        }

//        public static IdentityUser GetCurrentUser()
//        {
//            try
//            {
//                if (HttpContext.Current.User.Identity.IsAuthenticated)
//                {
//                    var currentUser = JsonConvert.DeserializeObject<IdentityUser>(HttpContext.Current.User.Identity.Name);

//                    return currentUser;
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Failed when GetCurrentUser due to {0}", ex.ToString()));
//            }

//            return null;
//        }

//        public static string ShowDisplayName(IdentityUser user)
//        {
//            var displayName = "Guest";
//            if(user != null)
//            {
//                if (!string.IsNullOrEmpty(user.FullName))
//                    displayName = user.FullName;
//                else
//                {
//                    if (!string.IsNullOrEmpty(user.DisplayName))
//                        displayName = user.DisplayName;
//                    else
//                        displayName = user.UserName;
//                }
//            }

//            return displayName;
//        }

//        public static void UpdateUserCachedData(string fieldName, string fieldValue)
//        {
//            try
//            {
//                var userInfo = GetCurrentUser();
//                if (userInfo != null)
//                {
//                    var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
//                    var cacheKey = string.Format("{0}_{1}", "USER", userInfo.Id);

//                    var currentProperty = fieldName.ToLower();
//                    if (currentProperty == nameof(userInfo.Email).ToLower())
//                    {
//                        userInfo.Email = fieldValue;
//                    }
//                    else if (currentProperty == nameof(userInfo.DisplayName).ToLower())
//                    {
//                        userInfo.DisplayName = fieldValue;
//                    }
//                    else if (currentProperty == nameof(userInfo.PhoneNumber).ToLower())
//                    {
//                        userInfo.PhoneNumber = fieldValue;
//                    }
//                    else if (currentProperty == nameof(userInfo.Birthday).ToLower())
//                    {
//                        var birthday = ParseDateTimeWithFormat(Constant.DATE_FORMAT_ddMMyyyy, fieldValue);
//                        userInfo.Birthday = birthday;
//                    }
//                    else if (currentProperty == nameof(userInfo.Avatar).ToLower())
//                    {
//                        userInfo.Avatar = fieldValue;
//                    }

//                    //var overMinutes = Utils.ConvertToInt32((DateTime.Now.Subtract(userInfo.LoginDate).TotalMinutes));
//                    //var expiredTimeRemains = userInfo.LoginDurations - overMinutes;
//                    cacheProvider.Set(cacheKey, userInfo, userInfo.LoginDurations);

//                    //Clear old data
//                    cacheProvider.Clear(string.Format("PROFILE_{0}", userInfo.Id));
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Failed when UpdateUserCachedData due to {0}", ex.ToString()));
//            }

//        }

//        public static DateTime ParseDateTimeWithFormat(string format, string dtStr)
//        {
//            var dt = DateTime.Now;
//            try
//            {
//                dt = DateTime.ParseExact(dtStr, format, null);
//            }
//            catch
//            {

//            }

//            return dt;
//        }

//        public static void ClearLoginDataCached()
//        {
//            var id = GetCurrentUserId();
//            if (id != 0)
//            {
//                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
//                var cacheKey = string.Format("{0}_{1}", "USER", id);
//                cacheProvider.Clear(cacheKey);
//            }
//        }

//        public static bool HasLoggedIn()
//        {
//            var userInfo = GetCurrentUser();
//            if (userInfo != null)
//                return true;

//            return false;
//        }

//        public static int GetCurrentUserId()
//        {
//            var userInfo = GetCurrentUser();
//            if (userInfo != null)
//                return userInfo.Id;

//            return 0;
//        }

//        public static string GetCurrentUserAvatar()
//        {
//            var userInfo = GetCurrentUser();
//            if (userInfo != null)
//                return userInfo.Avatar;

//            return string.Empty;
//        }

//        public static string GetCurrentUserName()
//        {
//            var userInfo = GetCurrentUser();
//            if (userInfo != null)
//                return userInfo.UserName;

//            return string.Empty;
//        }

//        public static string GetUserProfileUrl(IdentityUser userInfo)
//        {
//            if (userInfo != null && userInfo.Id > 0)
//                return string.Format(UrlFriendly.getUrlByUserProfile(userInfo.DisplayName, userInfo.Id));

//            return "javascript:;";
//        }

//        //public static string GetUserProfileUrl(IdentityFriendRequest requestInfo)
//        //{
//        //    if (requestInfo != null && requestInfo.Id > 0)
//        //        return string.Format(UrlFriendly.getUrlByUserProfile(requestInfo.DisplayName, requestInfo.UserSendId));

//        //    return "javascript:;";
//        //}

//        public static string GetUserProfileUrl(ApiAuthUserLoginIdentity userInfo)
//        {
//            if (userInfo != null && userInfo.Id > 0)
//                return string.Format(UrlFriendly.getUrlByUserProfile(userInfo.DisplayName, userInfo.Id));

//            return "javascript:;";
//        }

//        public static string GetUserProfileUrl(IdentityUserInfo userInfo)
//        {
//            if (userInfo != null && userInfo.UserId > 0)
//                return string.Format(UrlFriendly.getUrlByUserProfile(userInfo.DisplayName, userInfo.UserId));

//            return "javascript:;";
//        }

//        public static string GetUserProfileUrl(IdentityCommentReply replyInfo)
//        {
//            if (replyInfo != null && replyInfo.UserId > 0)
//                return string.Format(UrlFriendly.getUrlByUserProfile(replyInfo.DisplayName, replyInfo.UserId));

//            return "javascript:;";
//        }

//        public static string GetUserProfileUrl(IdentityComment commentInfo)
//        {
//            if (commentInfo != null && commentInfo.UserId > 0)
//                return string.Format(UrlFriendly.getUrlByUserProfile(commentInfo.DisplayName, commentInfo.UserId));

//            return "javascript:;";
//        }

//        public static string GetUserProfileUrl(IdentityPost postInfo)
//        {
//            if (postInfo != null && postInfo.UserId > 0)
//                return string.Format(UrlFriendly.getUrlByUserProfile(postInfo.DisplayName, postInfo.UserId));

//            return "javascript:;";
//        }

//        //public static string GetUserProfileUrl(IdentityNotification notif)
//        //{
//        //    if (notif != null && notif.ActorId > 0)
//        //        return string.Format(UrlFriendly.getUrlByUserProfile(notif.ActorDisplayName, notif.ActorId));

//        //    return "javascript:;";
//        //}

//        public static string GetShortDisplayName(string input, int length = 25)
//        {
//            if (!string.IsNullOrEmpty(input))
//            {
//                if (input.Length > length)
//                    return input.Substring(0, length) + "...";
//                else
//                    return input;
//            }

//            return string.Empty;
//        }

//        public static string ParseUserDisplayName(IdentityUser user, int length = 25)
//        {
//            try
//            {
//                var result = string.Empty;
//                if (user != null)
//                {
//                    if (!string.IsNullOrEmpty(user.DisplayName))
//                    {
//                        result = user.DisplayName;
//                        return GetShortDisplayName(result, length);
//                    }

//                    if (!string.IsNullOrEmpty(user.FullName))
//                    {
//                        result = user.FullName;
//                        return GetShortDisplayName(result, length);
//                    }

//                    if (!string.IsNullOrEmpty(user.UserName))
//                    {
//                        result = user.UserName;
//                        return GetShortDisplayName(result, length);
//                    }
//                }

//                if (user == null)
//                {
//                    logger.Error(string.Format("Failed when ParseUserDisplayName. The User data is null"));
//                    return string.Empty;
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Failed when ParseUserDisplayName [{0}] due to {1}", user.Id, ex.ToString()));
//            }

//            return string.Empty;
//        }


//        public static string OnClickEventWithLogin()
//        {
//            var userInfo = AccountHelper.GetCurrentUser();
//            if (userInfo == null)
//            {
//                var onclickStr = String.Format("ConfirmFirst(NeedToLogin, '{0}')", UserWebResource.COMMON_ERROR_NO_LOGIN);
//                return onclickStr;
//            }

//            return string.Empty;
//        }
//    }
//}