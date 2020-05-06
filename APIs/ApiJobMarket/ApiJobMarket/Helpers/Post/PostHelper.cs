//using System;
//using Autofac;
//using ApiJobMarket;
//using ApiJobMarket.Caching.Providers;
//using ApiJobMarket.DB.Sql.Entities;
//using ApiJobMarket.DB.Sql.Stores;
//using ApiJobMarket.Logging;
//using ApiJobMarket.Settings;

//namespace ApiJobMarket.WebApp.Helpers
//{
//    public class PostHelper
//    {
//        private static readonly ILog logger = LogProvider.For<PostHelper>();
//        private static readonly string _postBaseInfoCacheKey = "POST_";
        
//        public static IdentityPost GetBaseInfo(int id)
//        {
//            var myKey = string.Format("{0}{1}", _postBaseInfoCacheKey, id);
//            IdentityPost baseInfo = null;
//            try
//            {
//                //Check from cache first
//                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
//                cacheProvider.Get<IdentityPost>(myKey, out baseInfo);

//                if(baseInfo == null)
//                {
//                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePost>();
//                    baseInfo = myStore.GetBaseInfo(id);

//                    if(baseInfo != null)
//                    {
//                        //Storage to cache
//                        cacheProvider.Set(myKey, baseInfo, SystemSettings.UserCachingTime);
//                    }                   
//                }         
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Could not GetPostBaseInfo: " + ex.ToString());
//            }

//            return baseInfo;
//        }

//        public static void ClearPostBaseCache(int id)
//        {
//            var myKey = string.Format("{0}{1}", _postBaseInfoCacheKey, id);
//            try
//            {
//                //Check from cache first
//                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

//                //Storage to cache
//                cacheProvider.Clear(myKey);
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Could not ClearPostBaseCache of user [{0}] because: {1}", id, ex.ToString()));
//            }
//        }
//    }
//}