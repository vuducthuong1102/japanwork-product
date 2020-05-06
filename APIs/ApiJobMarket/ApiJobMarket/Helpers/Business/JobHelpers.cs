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
    public class JobHelpers
    {
        private static readonly ILog logger = LogProvider.For<JobHelpers>();

        public static IdentityJob GetBaseInfoJob(int id, string language_code)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.JobInfoByLang, id, language_code);
            IdentityJob baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityJob>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreJob>();
                    baseInfo = myStore.GetBaseInfo(id, language_code);

                    if (baseInfo != null)
                    {
                        if (baseInfo.Job_translations.HasData())
                        {
                            var tranInfo = baseInfo.Job_translations[0];
                            if (tranInfo.language_code != language_code)
                                myKey = string.Format(EnumFormatInfoCacheKeys.JobInfoByLang, id, tranInfo.language_code);
                            else
                                //Storage to cache
                                cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                        }
                    }
                }

                if (baseInfo != null)
                {
                    if (!baseInfo.view_company)
                    {
                        baseInfo.company_info = new IdentityCompanyShort();

                        var agencyInfo = AgencyHelpers.GetBaseInfoAgency(baseInfo.company_info.agency_id);
                        if (agencyInfo != null)
                        {
                            baseInfo.company_info.company_name = agencyInfo.company_name;
                            baseInfo.company_info.logo_full_path = agencyInfo.logo_full_path;
                        }
                    }
                    else
                    {
                        if (baseInfo.company_id > 0)
                        baseInfo.company_info = CompanyHelpers.GetBaseInfoCompanyShort(baseInfo.company_id, language_code);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetBaseInfoJob: " + ex.ToString());
            }

            return baseInfo;
        }

        public static IdentityJob Agency_GetBaseInfoJob(int id, string language_code)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.JobInfoByLang, id, language_code);
            IdentityJob baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityJob>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreJob>();
                    baseInfo = myStore.GetBaseInfo(id, language_code);

                    if (baseInfo != null)
                    {
                        if (baseInfo.Job_translations.HasData())
                        {
                            var tranInfo = baseInfo.Job_translations[0];
                            if (tranInfo.language_code != language_code)
                                myKey = string.Format(EnumFormatInfoCacheKeys.JobInfoByLang, id, tranInfo.language_code);
                            else
                                //Storage to cache
                                cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                        }
                    }
                }

                if (baseInfo != null)
                {
                    if (baseInfo.company_id > 0)
                        baseInfo.company_info = CompanyHelpers.GetBaseInfoCompanyShort(baseInfo.company_id, language_code);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetBaseInfoJob: " + ex.ToString());
            }

            return baseInfo;
        }

        public static List<IdentityJob> GetListJobs(List<int> listIds, string language_code)
        {
            List<IdentityJob> myList = new List<IdentityJob>();
            try
            {
                if (listIds.HasData())
                {
                    foreach (var id in listIds)
                    {
                        var info = GetBaseInfoJob(id, language_code);
                        if (info != null)
                            myList.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListJobs: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityApplication> GetListHots(int job_seeker_id, int page_size)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.JobHots + "_" + job_seeker_id);
            List<IdentityApplication> listHots = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityApplication>>(myKey, out listHots);

                if (listHots == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreJob>();
                    listHots = myStore.GetListHot(job_seeker_id, page_size);

                    if (listHots != null)
                    {
                        //Storage to cache
                        cacheProvider.Set(myKey, listHots);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListHots: " + ex.ToString());
            }

            return listHots;
        }
    }
}