using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using SingleSignOn;
using SingleSignOn.Caching.Providers;
using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.DB.Sql.Stores;
using SingleSignOn.Logging;
using SingleSignOn.Settings;
using StackExchange.Redis.Extensions.Core;

namespace Manager.WebApp.Helpers
{
    public class CommonHelpers
    {
        private static readonly ILog logger = LogProvider.For<CommonHelpers>();
        
        public static List<IdentityLocation> GetListLocation(string keyword)
        {
            var myKey = string.Format("{0}{1}", SystemSettings.CommonCacheKeyPrefix, "LOCATIONS");
            List<IdentityLocation> myList = null;
            List<IdentityLocation> returnList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityLocation>>(myKey, out myList);

                if(myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreLocation>();
                    myList = myStore.GetList(string.Empty);

                    //Storage to cache
                    cacheProvider.Set(myKey, myList);
                }

                if(myList != null && myList.Count > 0)
                {
                    returnList = myList.Where(x => x.Name.ToLower().Contains(keyword.ToLower())).ToList();
                }                
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListLocation: " + ex.ToString());
            }

            return returnList;
        }
    }
}