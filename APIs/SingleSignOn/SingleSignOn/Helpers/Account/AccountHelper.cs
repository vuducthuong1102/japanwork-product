using System;
using System.Collections.Generic;
using Autofac;
using SingleSignOn;
using SingleSignOn.Caching.Providers;
using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.DB.Sql.Stores;
using SingleSignOn.Logging;

namespace SingleSignOn.Helpers
{
    public class AccountHelper
    {
        private static readonly ILog logger = LogProvider.For<AccountHelper>();
        private static readonly string _profileCacheKey = "PROFILE_";
        private static readonly string _tokenCacheKey = "TOKEN_";
        
        public static IdentityUser GetUserInfoFromCache(int id)
        {
            var myKey = string.Format("{0}{1}", _profileCacheKey, id);
            IdentityUser userInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityUser>(myKey, out userInfo);                
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetUserInfoFromCache: " + ex.ToString());
            }

            return userInfo;
        }

        public static IdentityUser GetUserProfile(int id)
        {
            var myKey = string.Format("{0}{1}", _profileCacheKey, id);
            IdentityUser userInfo = null;
            try
            {
                //Check from cache first
                //userInfo = GetUserInfoFromCache(id);

                //if(userInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreUser>();
                    userInfo = myStore.GetUserProfile(id);

                    if(userInfo != null)
                    {
                        userInfo.Avatar = CdnHelper.GetFullImgPath(userInfo.Avatar);

                        //Storage to cache
                        //var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                        //cacheProvider.Set(myKey, userInfo);
                    }                   
                }         
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetUserProfile: " + ex.ToString());
            }

            return userInfo;
        }

        public static List<IdentityUser> GetListUserProfile(string listUserId)
        {
            List<IdentityUser> listUserInfo = new List<IdentityUser>();
            try
            {
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreUser>();
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

                listUserInfo = myStore.GetListUserInfo(listUserId);

                if (listUserInfo != null && listUserInfo.Count > 0)
                {
                    foreach (var item in listUserInfo)
                    {
                        item.Avatar = CdnHelper.GetFullImgPath(item.Avatar);
                        var myKey = string.Format("{0}{1}", _profileCacheKey, item.Id);
                        //Storage to cache
                        cacheProvider.Set(myKey, item);
                    }
                }
             
            }
            catch (Exception ex)
            {
                logger.Error("Could not AccountHelper.GetListUserProfile: " + ex.ToString());
            }

            return listUserInfo;
        }

        public static void ClearUserCache(int id)
        {
            var myKey = string.Format("{0}{1}", _profileCacheKey, id);
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

                //Storage to cache
                cacheProvider.Clear(myKey);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not ClearUserCache of user [{0}] because: {1}", id, ex.ToString()));
            }
        }

        public static UserTokenIdentity GetUserToken(int userId)
        {
            var myKey = string.Format("{0}{1}", _tokenCacheKey, userId);
            UserTokenIdentity userInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<UserTokenIdentity>(myKey, out userInfo);

                if (userInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreUser>();
                    userInfo = myStore.GetCurrentTokenKeyByUser(userId);

                    if(userInfo != null)
                    {
                        TimeSpan span = userInfo.ExpiredDate.Value.Subtract(userInfo.CreatedDate.Value);

                        //Storage to cache
                        cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                        cacheProvider.Set(myKey, userInfo, 5);
                    }                   
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetUserToken: " + ex.ToString());
            }

            return userInfo;
        }
    }
}