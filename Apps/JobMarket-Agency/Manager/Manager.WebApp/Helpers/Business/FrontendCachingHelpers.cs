using System;
using Autofac;
using Manager.SharedLibs.Caching.Providers;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Settings;
using Manager.WebApp.Caching;

namespace Manager.WebApp.Helpers
{
    public class FrontendCachingHelpers
    {
        private static readonly ILog logger = LogProvider.For<FrontendCachingHelpers>();

        public static void ClearProjectCategoryCache()
        {
            var myKey = "PROJECTCATEGORIES";
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Clear(myKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearProjectCategoryCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearNavigationCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("NAVIGATIONS");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearNavigationCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearProjectCateById(int id)
        {
            var myKey = string.Format("{0}_{1}", "PROJECT", id);
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Clear(myKey);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not ClearProjectCateById [{id}] because: {1}", id, ex.ToString()));
            }
        }

        public static void ClearPostCacheById(int id)
        {
            var myKey = string.Format("{0}_{1}", "POST", id);
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Clear(myKey);                
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not ClearPostCacheById [{id}] because: {1}", id, ex.ToString()));
            }
        }

        public static void ClearPostCacheByCategoryId(int catId)
        {
            var myKey = string.Format("GROUP_ARTICLES_{0}", catId);
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.ClearAll(myKey);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not ClearPostCacheByCategoryId [{id}] because: {1}", catId, ex.ToString()));
            }
        }

        public static void ClearFooterCached()
        {
            var myKey = "FOOTERS";
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Clear(myKey);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not ClearFooterCached because: {0}", ex.ToString()));
            }
        }
    }
}