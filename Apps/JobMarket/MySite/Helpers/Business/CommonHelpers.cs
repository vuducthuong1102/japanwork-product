using ApiJobMarket.DB.Sql.Entities;
using Autofac;
using MySite.Caching;
using MySite.Caching.Providers;
using MySite.Logging;
using MySite.Models;
using MySite.Resources;
using MySite.Services;
using MySite.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MySite.Helpers
{
    public class CommonHelpers
    {
        private static readonly ILog logger = LogProvider.For<CommonHelpers>();

        public static string GetCurrentLangageCode()
        {
            var currentLang = "vi-VN";
            try
            {
                currentLang = CultureInfo.CurrentCulture.ToString();
            }
            catch
            {

            }

            return currentLang;
        }

        public static List<IdentityEmploymentType> GetListEmploymentTypes()
        {
            var currentAction = "GetListEmploymentTypes";
            List<IdentityEmploymentType> returnList = null;
            var myKey = EnumListCacheKeys.EmploymentTypes;
            var needToCallApi = false;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (!SystemSettings.MasterCategoryCachingEnable)
                {
                    needToCallApi = true;
                }
                else
                {
                    cacheProvider.Get<List<IdentityEmploymentType>>(myKey, out returnList);
                    if (!returnList.HasData())
                    {
                        needToCallApi = true;
                    }
                    else
                    {
                        return returnList;
                    }
                }

                if (needToCallApi)
                {
                    var apiReturned = EmploymentTypeServices.GetListAsync().Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            returnList = JsonConvert.DeserializeObject<List<IdentityEmploymentType>>(apiReturned.value.ToString());

                            if (SystemSettings.MasterCategoryCachingEnable)
                            {
                                if (returnList.HasData())
                                    cacheProvider.Set(myKey, returnList, SystemSettings.DefaultCachingTimeInMinutes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityQualification> GetListQualifications()
        {
            var currentAction = "GetListQualifications";
            List<IdentityQualification> returnList = null;
            var myKey = EnumListCacheKeys.Qualifications;
            var needToCallApi = false;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (!SystemSettings.MasterCategoryCachingEnable)
                {
                    needToCallApi = true;
                }
                else
                {
                    cacheProvider.Get<List<IdentityQualification>>(myKey, out returnList);
                    if (!returnList.HasData())
                    {
                        needToCallApi = true;
                    }
                    else
                    {
                        return returnList;
                    }
                }

                if (needToCallApi)
                {
                    var apiReturned = QualificationServices.GetListAsync().Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            returnList = JsonConvert.DeserializeObject<List<IdentityQualification>>(apiReturned.value.ToString());

                            if (SystemSettings.MasterCategoryCachingEnable)
                            {
                                if (returnList.HasData())
                                    cacheProvider.Set(myKey, returnList, SystemSettings.DefaultCachingTimeInMinutes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityMajor> GetListMajors()
        {
            var currentAction = "GetListMajors";
            List<IdentityMajor> returnList = null;
            var myKey = EnumListCacheKeys.Majors;
            var needToCallApi = false;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (!SystemSettings.MasterCategoryCachingEnable)
                {
                    needToCallApi = true;
                }
                else
                {
                    cacheProvider.Get<List<IdentityMajor>>(myKey, out returnList);
                    if (!returnList.HasData())
                    {
                        needToCallApi = true;
                    }
                    else
                    {
                        return returnList;
                    }
                }

                if (needToCallApi)
                {
                    var apiReturned = MajorServices.GetListAsync().Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            returnList = JsonConvert.DeserializeObject<List<IdentityMajor>>(apiReturned.value.ToString());

                            if (SystemSettings.MasterCategoryCachingEnable)
                            {
                                if (returnList.HasData())
                                    cacheProvider.Set(myKey, returnList, SystemSettings.DefaultCachingTimeInMinutes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityIndustry> GetListIndustries()
        {
            var currentAction = "GetListIndustries";
            List<IdentityIndustry> returnList = null;
            var myKey = EnumListCacheKeys.Industries;
            var needToCallApi = false;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (!SystemSettings.MasterCategoryCachingEnable)
                {
                    needToCallApi = true;
                }
                else
                {
                    cacheProvider.Get<List<IdentityIndustry>>(myKey, out returnList);
                    if (!returnList.HasData())
                    {
                        needToCallApi = true;
                    }
                    else
                    {
                        return returnList;
                    }
                }

                if (needToCallApi)
                {
                    var apiReturned = IndustryServices.GetListAsync().Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            returnList = JsonConvert.DeserializeObject<List<IdentityIndustry>>(apiReturned.value.ToString());

                            if (SystemSettings.MasterCategoryCachingEnable)
                            {
                                if (returnList.HasData())
                                    cacheProvider.Set(myKey, returnList, SystemSettings.DefaultCachingTimeInMinutes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityField> GetListFields()
        {
            var currentAction = "GetListFields";
            List<IdentityField> returnList = null;
            var myKey = EnumListCacheKeys.Fields;
            var needToCallApi = false;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (!SystemSettings.MasterCategoryCachingEnable)
                {
                    needToCallApi = true;
                }
                else
                {
                    cacheProvider.Get<List<IdentityField>>(myKey, out returnList);
                    if (!returnList.HasData())
                    {
                        needToCallApi = true;
                    }
                    else
                    {
                        return returnList;
                    }
                }

                if (needToCallApi)
                {
                    var apiReturned = FieldServices.GetListAsync().Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            returnList = JsonConvert.DeserializeObject<List<IdentityField>>(apiReturned.value.ToString());

                            if (SystemSettings.MasterCategoryCachingEnable)
                            {
                                if (returnList.HasData())
                                    cacheProvider.Set(myKey, returnList, SystemSettings.DefaultCachingTimeInMinutes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }
        public static List<IdentityField> GetListFieldCounts()
        {
            var currentAction = "GetListFieldCounts";
            List<IdentityField> returnList = null;
            var myKey = EnumListCacheKeys.FieldCounts;
            var needToCallApi = false;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (!SystemSettings.MasterCategoryCachingEnable)
                {
                    needToCallApi = true;
                }
                else
                {
                    cacheProvider.Get<List<IdentityField>>(myKey, out returnList);
                    if (!returnList.HasData())
                    {
                        needToCallApi = true;
                    }
                    else
                    {
                        return returnList;
                    }
                }

                if (needToCallApi)
                {
                    var apiReturned = FieldServices.GetListCountAsync().Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            returnList = JsonConvert.DeserializeObject<List<IdentityField>>(apiReturned.value.ToString());

                            if (SystemSettings.MasterCategoryCachingEnable)
                            {
                                if (returnList.HasData())
                                    cacheProvider.Set(myKey, returnList, SystemSettings.DefaultCachingTimeInMinutes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }
        public static List<IdentityJapaneseLevel> GetListJapaneseLevels()
        {
            var currentAction = "GetListJapaneseLevels";
            List<IdentityJapaneseLevel> returnList = null;
            var myKey = EnumListCacheKeys.JapaneseLevels;
            var needToCallApi = false;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (!SystemSettings.MasterCategoryCachingEnable)
                {
                    needToCallApi = true;
                }
                else
                {
                    cacheProvider.Get<List<IdentityJapaneseLevel>>(myKey, out returnList);
                    if (!returnList.HasData())
                    {
                        needToCallApi = true;
                    }
                    else
                    {
                        return returnList;
                    }
                }

                if (needToCallApi)
                {
                    var apiReturned = JapaneseLevelServices.GetListAsync().Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            returnList = JsonConvert.DeserializeObject<List<IdentityJapaneseLevel>>(apiReturned.value.ToString());

                            if (SystemSettings.MasterCategoryCachingEnable)
                            {
                                if (returnList.HasData())
                                    cacheProvider.Set(myKey, returnList, SystemSettings.DefaultCachingTimeInMinutes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityCountry> GetListCountries()
        {
            var currentAction = "GetListCountries";
            List<IdentityCountry> returnList = null;
            try
            {
                var apiReturned = CountryServices.GetListAsync().Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityCountry>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityVisa> GetListVisas()
        {
            var currentAction = "GetListVisas";
            List<IdentityVisa> returnList = null;
            try
            {
                var apiReturned = VisaServices.GetListAsync().Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityVisa>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityApplication> GetListHots(int job_seeker_id,int page_size)
        {
            var currentAction = "GetListHots";
            List<IdentityApplication> returnList = null;
            try
            {
                var apiReturned = JobServices.GetListHotsAsync(job_seeker_id, page_size).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityApplication>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityRegion> GetListRegions(List<int> listIds)
        {
            var currentAction = "GetListRegionsFromListIds";
            List<IdentityRegion> returnList = null;
            try
            {
                var apiInputModel = new ApiGetListByIdsModel();
                apiInputModel.ListIds = listIds;

                var apiReturned = RegionServices.GetListAsync(apiInputModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityRegion> GetListRegions()
        {
            var currentAction = "GetListRegions";
            List<IdentityRegion> returnList = null;
            try
            {
                var apiReturned = RegionServices.GetListAsync().Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityPrefecture> GetListPrefectures(List<int> listIds)
        {
            var currentAction = "GetListPrefectures";
            List<IdentityPrefecture> returnList = null;
            try
            {
                var apiInputModel = new ApiGetListByIdsModel();
                apiInputModel.ListIds = listIds;

                var apiReturned = PrefectureServices.GetListAsync(apiInputModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityPrefecture>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityCity> GetListCities(List<int> listIds)
        {
            var currentAction = "GetListCities";
            List<IdentityCity> returnList = null;
            try
            {
                var apiInputModel = new ApiGetListByIdsModel();
                apiInputModel.ListIds = listIds;

                var apiReturned = CityServices.GetListAsync(apiInputModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityCity>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityStation> GetListStations(List<int> listIds)
        {
            var currentAction = "GetListStations";
            List<IdentityStation> returnList = null;
            try
            {
                var apiInputModel = new ApiGetListByIdsModel();
                apiInputModel.ListIds = listIds;

                var apiReturned = StationServices.GetListAsync(apiInputModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityStation>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }
    }
}