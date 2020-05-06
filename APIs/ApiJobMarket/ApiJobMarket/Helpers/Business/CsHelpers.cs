using System;
using Autofac;
using ApiJobMarket.Caching.Providers;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Stores;
using ApiJobMarket.Logging;
using ApiJobMarket.SharedLib.Extensions;
using ApiCompanyMarket.Helpers;
using ApiJobMarket.Settings;

namespace ApiJobMarket.Helpers
{
    public class CsHelpers
    {
        private static readonly ILog logger = LogProvider.For<JobHelpers>();

        public static IdentityCs GetBaseInfoCs(int id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.Cs, id);
            IdentityCs baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityCs>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCs>();
                    baseInfo = myStore.GetDetail(id);
                    
                    if (baseInfo != null)
                    {
                        baseInfo.image_full_path = CdnHelper.SocialGetFullImgPath(baseInfo.image);

                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetBaseInfoCs: " + ex.ToString());
            }

            return baseInfo;
        }
        
    }
}