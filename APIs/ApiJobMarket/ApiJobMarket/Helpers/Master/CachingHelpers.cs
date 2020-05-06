using System;
using System.Collections.Generic;
using Autofac;
using ApiJobMarket.Caching.Providers;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Stores;
using ApiJobMarket.Logging;

namespace ApiJobMarket.Helpers
{
    public class CachingHelpers
    {
        private static readonly ILog logger = LogProvider.For<CachingHelpers>();

        public static void ClearCacheByKey(string key)
        {
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Clear(key);               
            }
            catch (Exception ex)
            {
                logger.Error("Could not ClearCacheByKey: " + ex.ToString());
            }
        }

        public static void ClearCacheByPrefix(string prefix)
        {
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.ClearAll(prefix);
            }
            catch (Exception ex)
            {
                logger.Error("Could not ClearCacheByPrefix: " + ex.ToString());
            }
        }
    }
}