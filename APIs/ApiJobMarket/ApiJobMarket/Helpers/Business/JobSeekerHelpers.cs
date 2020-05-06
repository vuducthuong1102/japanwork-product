using System;
using System.Collections.Generic;
using Autofac;
using ApiJobMarket.Caching.Providers;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Stores;
using ApiJobMarket.Logging;
using ApiJobMarket.Settings;
using ApiJobMarket.SharedLib.Extensions;
using ApiJobMarket.Models;

namespace ApiJobMarket.Helpers
{
    public class JobSeekerHelpers
    {
        private static readonly ILog logger = LogProvider.For<JobSeekerHelpers>();

        public static IdentityJobSeeker GetBaseInfo(int id, int agency_id = 0)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, id);
            IdentityJobSeeker baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityJobSeeker>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();
                    baseInfo = myStore.GetBaseInfo(id, agency_id);

                    if (baseInfo != null)
                    {
                        if (!string.IsNullOrEmpty(baseInfo.image))
                            baseInfo.Extensions.image_full = CdnHelper.SocialGetFullImgPath(baseInfo.image);

                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetBaseInfo: " + ex.ToString());
            }

            return baseInfo;
        }

        public static IdentityJobSeekerConfig GetConfig(int id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.JobSeekerConfig, id);
            IdentityJobSeekerConfig baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityJobSeekerConfig>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();
                    baseInfo = myStore.GetConfig(id);

                    if (baseInfo != null)
                    {
                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetConfig: " + ex.ToString());
            }

            return baseInfo;
        }

        public static IdentityJobSeeker A_GetBaseInfo(int id, int agency_id = 0)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.JobSeekerAgencyInfo, id);
            IdentityJobSeeker baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityJobSeeker>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();
                    baseInfo = myStore.A_GetBaseInfo(id, agency_id);

                    if (baseInfo != null)
                    {
                        baseInfo.Extensions.image_full = CdnHelper.SocialGetFullImgPath(baseInfo.image);

                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not A_GetBaseInfo: " + ex.ToString());
            }

            return baseInfo;
        }
    }
}