using System;
using System.Linq;
using ApiJobMarket;
using ApiJobMarket.Caching.Providers;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Stores;
using ApiJobMarket.Helpers;
using ApiJobMarket.Logging;
using ApiJobMarket.Settings;
using ApiJobMarket.SharedLib.Extensions;
using Autofac;

namespace ApiCompanyMarket.Helpers
{
    public class CompanyHelpers
    {
        private static readonly ILog logger = LogProvider.For<CompanyHelpers>();

        public static IdentityCompany GetBaseInfoCompany(int id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.Company, id);
            IdentityCompany baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityCompany>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCompany>();
                    baseInfo = myStore.GetById(id);

                    if (baseInfo != null)
                    {
                        if (baseInfo != null)
                            baseInfo.logo_full_path = CdnHelper.SocialGetFullImgPath(baseInfo.logo_path);

                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetBaseInfoCompany: " + ex.ToString());
            }

            return baseInfo;
        }

        public static IdentityCompanyShort GetBaseInfoCompanyShort(int id, string language_code = "")
        {
            IdentityCompanyShort baseInfo = null;
            try
            {
                var info = GetBaseInfoCompany(id);
                if (info != null)
                {
                    baseInfo = new IdentityCompanyShort();
                    baseInfo.id = info.id;
                    baseInfo.company_name = info.company_name;
                    baseInfo.logo_full_path = info.logo_full_path;
                    baseInfo.agency_id = info.agency_id;
                    baseInfo.company_code = info.company_code;

                    if (info.LangList.HasData())
                    {
                        var companyName = info.LangList.Where(x => x.language_code == language_code).Select(x => x.company_name).FirstOrDefault();
                        if (string.IsNullOrEmpty(companyName))
                            companyName = info.LangList[0].company_name;

                        if (!string.IsNullOrEmpty(companyName))
                            baseInfo.company_name = companyName;

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetBaseInfoCompanyShort: " + ex.ToString());
            }

            return baseInfo;
        }

        public static void ClearCache(int id)
        {
            CachingHelpers.ClearCacheByPrefix(string.Format(string.Format(EnumFormatInfoCacheKeys.Company, id)));
        }
    }
}