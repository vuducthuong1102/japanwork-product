using System;
using Autofac;
using ApiJobMarket.Caching.Providers;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Stores;
using ApiJobMarket.Logging;
using ApiJobMarket.SharedLib.Extensions;
using ApiCompanyMarket.Helpers;
using System.Linq;
using ApiJobMarket.Settings;

namespace ApiJobMarket.Helpers
{
    public class CvHelpers
    {
        private static readonly ILog logger = LogProvider.For<JobHelpers>();

        public static IdentityCv GetBaseInfoCv(int id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.Cv, id);
            IdentityCv baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityCv>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCv>();
                    baseInfo = myStore.GetDetail(id);

                    //Storage to cache
                    cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                }

                if(baseInfo != null)
                {
                    var jkInfo = JobSeekerHelpers.GetBaseInfo(baseInfo.job_seeker_id);
                    if(jkInfo != null)
                    {
                        baseInfo.fullname = jkInfo.fullname;
                        baseInfo.fullname_furigana = jkInfo.fullname_furigana;
                        baseInfo.gender = jkInfo.gender;
                        baseInfo.birthday = jkInfo.birthday;
                        baseInfo.email = jkInfo.email;
                        baseInfo.phone = jkInfo.phone;
                        baseInfo.marriage = jkInfo.marriage == 0 ? false : true;
                        baseInfo.dependent_num = jkInfo.dependent_num;

                        baseInfo.image = jkInfo.image;
                        if (!string.IsNullOrEmpty(baseInfo.image))
                            baseInfo.image_full_path = CdnHelper.SocialGetFullImgPath(baseInfo.image);

                        if (jkInfo.Addresses.HasData())
                        {
                            baseInfo.address = jkInfo.Addresses.Where(x => x.is_contact_address == false).FirstOrDefault();
                            baseInfo.address_contact = jkInfo.Addresses.Where(x => x.is_contact_address == true).FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetBaseInfoCv: " + ex.ToString());
            }

            return baseInfo;
        }
        
    }
}