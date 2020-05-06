//using System;
//using Autofac;
//using ApiJobMarket;
//using ApiJobMarket.Caching.Providers;
//using ApiJobMarket.DB.Sql.Entities;
//using ApiJobMarket.DB.Sql.Stores;
//using ApiJobMarket.Helpers;
//using ApiJobMarket.Logging;
//using ApiJobMarket.Settings;

//namespace ApiJobMarket.WebApp.Helpers
//{
//    public class AccountHelper
//    {
//        private static readonly ILog logger = LogProvider.For<AccountHelper>();
//        private static readonly string _profileCacheKey = "PROFILE_";
//        private static readonly string _counterCacheKey = "COUNTER_";
        
//        public static IdentityUser GetUserProfile(int id)
//        {
//            var myKey = string.Format("{0}{1}", _profileCacheKey, id);
//            IdentityUser userInfo = null;
//            try
//            {
//                //Check from cache first
//                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
//                cacheProvider.Get<IdentityUser>(myKey, out userInfo);

//                if(userInfo == null)
//                {
//                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreUser>();
//                    userInfo = myStore.GetUserProfile(id);

//                    if(userInfo != null)
//                    {
//                        userInfo.Avatar = CdnHelper.CoreGetFullImgPath(userInfo.Avatar);

//                        //Storage to cache
//                        cacheProvider.Set(myKey, userInfo, SystemSettings.UserCachingTime);
//                    }                   
//                }         
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Could not GetUserProfile: " + ex.ToString());
//            }

//            return userInfo;
//        }

//        public static IdentityUserData GetCounter(int id)
//        {
//            var myKey = string.Format("{0}{1}", _counterCacheKey, id);
//            IdentityUserData userInfo = null;
//            try
//            {
//                //Check from cache first
//                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
//                cacheProvider.Get<IdentityUserData>(myKey, out userInfo);

//                if (userInfo == null)
//                {
//                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreUser>();
//                    userInfo = myStore.GetCounter(id);

//                    if (userInfo != null)
//                    {
//                        //Storage to cache
//                        cacheProvider.Set(myKey, userInfo, SystemSettings.UserCachingTime);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Could not GetUserProfile: " + ex.ToString());
//            }

//            return userInfo;
//        }

//        public static void ClearUserCache(int id)
//        {
//            var myKey = string.Format("{0}{1}", _profileCacheKey, id);
//            try
//            {
//                //Check from cache first
//                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

//                //Storage to cache
//                cacheProvider.Clear(myKey);
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Could not ClearUserCache of user [{0}] because: {1}", id, ex.ToString()));
//            }
//        }

//        public static void ClearUserCounter(int id)
//        {
//            var myKey = string.Format("{0}{1}", _counterCacheKey, id);
//            try
//            {
//                //Check from cache first
//                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

//                //Storage to cache
//                cacheProvider.Clear(myKey);
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Could not ClearUserCounter of user [{0}] because: {1}", id, ex.ToString()));
//            }
//        }
//    }
//}