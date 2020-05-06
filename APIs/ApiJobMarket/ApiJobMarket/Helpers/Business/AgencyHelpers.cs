using System;
using Autofac;
using ApiJobMarket.Caching.Providers;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Stores;
using ApiJobMarket.Logging;
using ApiJobMarket.SharedLib.Extensions;
using ApiCompanyMarket.Helpers;
using System.Collections.Generic;
using ApiJobMarket.Settings;

namespace ApiJobMarket.Helpers
{
    public class AgencyHelpers
    {
        private static readonly ILog logger = LogProvider.For<AgencyHelpers>();

        public static IdentityAgency GetBaseInfoAgency(int agency_id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.Agency, agency_id);
            IdentityAgency baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityAgency>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreAgency>();
                    baseInfo = myStore.GetBaseInfo(agency_id);

                    if (baseInfo != null)
                    {
                        if(!string.IsNullOrEmpty(baseInfo.logo_path))
                            baseInfo.logo_full_path = CdnHelper.SocialGetFullImgPath(baseInfo.logo_path);

                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetBaseInfoAgency: " + ex.ToString());
            }

            return baseInfo;
        }
    }
}