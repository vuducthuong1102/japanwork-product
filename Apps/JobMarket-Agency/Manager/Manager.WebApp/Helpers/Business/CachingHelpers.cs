using System;
using Autofac;
using Manager.SharedLibs.Caching.Providers;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Settings;

namespace Manager.WebApp.Helpers
{
    public class CachingHelpers
    {
        private static readonly ILog logger = LogProvider.For<CachingHelpers>();

        public static void ClearCacheByKey(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearCacheByKey: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearCacheByPrefix(string prefix = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.ClearAll(prefix);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearCacheByPrefix: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearEmailServerCache(int agencyId, string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("EMAIL_SERVERS");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearUnitCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearUnitCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("UNITS");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearUnitCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearUserCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("USERS");
                else
                    cacheProvider.Clear(string.Format("USER_{0}", cacheKey));
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearUserCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearDeviceCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("DEVICES");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearDeviceCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearProviderCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("PROVIDERS");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearProviderCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearCurrencyCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("CURRENCIES");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearCurrencyCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearEmailSynchronizedIds(int agencyId, int staffId)
        {
            var strError = string.Empty;
            var cacheKey = string.Format(EnumFormatInfoCacheKeys.EmailIdsSynchronized, agencyId, staffId);
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearEmailSynchronizedIds: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearProductCategoryCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("PRODUCTCATEGORIES");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearProductCategoryCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearPropertyCategoryCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("PROPERTYCATEGORIES");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearPropertyCategoryCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearPropertyCache(int parentId = 0)
        {
            var strError = string.Empty;
            var myKey = string.Format("{0}_{1}", "PROPERTIES_", parentId);
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (parentId == 0)
                    cacheProvider.ClearAll("PROPERTIES_");
                else
                    cacheProvider.Clear(myKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearPropertyCache [{0}]: {1}", myKey, ex.ToString());
                logger.Error(strError);
            }
        }
    }
}