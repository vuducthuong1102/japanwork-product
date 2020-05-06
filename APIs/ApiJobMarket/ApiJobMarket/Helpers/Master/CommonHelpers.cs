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
using Newtonsoft.Json;
using System.Linq;

namespace ApiJobMarket.Helpers
{
    public class CommonHelpers
    {
        private static readonly ILog logger = LogProvider.For<CommonHelpers>();

        public static List<DateTime> GetListDaysInCurrentWeek()
        {
            List<DateTime> dates = new List<DateTime>();
            try
            {
                DateTime today = DateTime.Today;
                int currentDayOfWeek = (int)today.DayOfWeek;
                DateTime sunday = today.AddDays(-currentDayOfWeek);
                DateTime monday = sunday.AddDays(1);

                // If we started on Sunday, we should actually have gone *back*
                // 6 days instead of forward 1...
                if (currentDayOfWeek == 0)
                {
                    monday = monday.AddDays(-7);
                }

                dates = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when get list days in current week because: {0}", ex.ToString()));
            }

            return dates;
        }

        public static IdentityStation GetBaseInfoStation(int id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.Station, id);
            IdentityStation baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityStation>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreStation>();
                    baseInfo = myStore.GetById(id);

                    if (baseInfo != null)
                    {
                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not BaseInfoStation: " + ex.ToString());
            }

            return baseInfo;
        }

        public static IdentityRegion GetBaseInfoRegion(int id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.Region, id);
            IdentityRegion baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityRegion>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreRegion>();
                    baseInfo = myStore.GetById(id);

                    if (baseInfo != null)
                    {
                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetBaseInfoRegion: " + ex.ToString());
            }

            return baseInfo;
        }
        public static List<IdentitySalaryFilter> GetListSalaryFilters()
        {
            var myKey = EnumListCacheKeys.SalaryFilters;
            List<IdentitySalaryFilter> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentitySalaryFilter>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreSalaryFilter>();
                    myList = myStore.GetAll();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetList SalaryFilter: " + ex.ToString());
            }
            return myList;
        }

        public static List<IdentityProcessStatus> GetListProcessStatus(int agency_id)
        {
            var myKey = EnumListCacheKeys.ProcessStatus + "_" + agency_id;
            List<IdentityProcessStatus> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityProcessStatus>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreProcessStatus>();
                    myList = myStore.GetList(agency_id);

                    myList.Add(new IdentityProcessStatus()
                    {
                        id = 27,
                        status_name = "終了",
                        status = 1,
                        order = 999
                    });

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetList ProcessStatus: " + ex.ToString());
            }
            return myList;
        }
        public static List<IdentityTypeSuggest> GetListTypeSuggests(string language_code)
        {
            var myKey = EnumListCacheKeys.TypeSuggests;
            List<IdentityTypeSuggest> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityTypeSuggest>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreTypeSuggest>();
                    myList = myStore.GetAll();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
                if (myList.HasData())
                {
                    foreach (var item in myList)
                    {
                        if (item.ListLang.HasData())
                        {
                            var langValue = item.ListLang.FirstOrDefault(s => s.language_code == language_code);
                            item.ListLang = new List<IdentityTypeSuggestLang>();
                            item.ListLang.Add(langValue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetList TypeSuggest: " + ex.ToString());
            }
            return myList;
        }
        public static IdentityPrefecture GetBaseInfoPrefecture(int id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.Prefecture, id);
            IdentityPrefecture baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityPrefecture>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePrefecture>();
                    baseInfo = myStore.GetById(id);

                    if (baseInfo != null)
                    {
                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetBaseInfoPrefecture: " + ex.ToString());
            }

            return baseInfo;
        }

        public static IdentityCity GetBaseInfoCity(int id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.City, id);
            IdentityCity baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityCity>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCity>();
                    baseInfo = myStore.GetById(id);

                    if (baseInfo != null)
                    {
                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetBaseInfoCity: " + ex.ToString());
            }

            return baseInfo;
        }

        public static List<IdentityCountry> GetCountries()
        {
            var myKey = EnumListCacheKeys.Countries;
            List<IdentityCountry> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityCountry>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCountry>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListCountries: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityRegion> GetListRegions(int country_id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.RegionByCountry, country_id);
            List<IdentityRegion> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityRegion>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreRegion>();
                    myList = myStore.GetList(country_id);

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListRegions: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityPrefecture> GetListPrefectures()
        {
            var myKey = EnumListCacheKeys.Prefectures;
            List<IdentityPrefecture> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityPrefecture>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePrefecture>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListRegions: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityCity> GetListCities()
        {
            var myKey = EnumListCacheKeys.Cities;
            List<IdentityCity> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityCity>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCity>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                    {
                        //Storage to cache
                        cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListCities: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityStation> GetListStations(List<int> listIds = null)
        {
            var myKey = EnumListCacheKeys.Stations;
            List<IdentityStation> myList = new List<IdentityStation>();
            try
            {
                foreach (var id in listIds)
                {
                    var info = GetBaseInfoStation(id);
                    if (info != null)
                        myList.Add(info);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListStations: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityStation> GetListStations()
        {
            var myKey = EnumListCacheKeys.Stations;
            List<IdentityStation> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityStation>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreStation>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                    {
                        //returnList = new List<ApiResponseStationItemModel>();

                        //foreach (var item in myList)
                        //{
                        //    ApiResponseStationItemModel record = new ApiResponseStationItemModel();
                        //    record.id = item.id;
                        //    record.station = item.station;
                        //    record.furigana = item.furigana;

                        //    returnList.Add(record);
                        //}

                        //Storage to cache
                        cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListStations: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityPrefecture> GetListPrefecturesByRegion(int region_id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.PrefecturesByRegion, region_id);
            List<IdentityPrefecture> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityPrefecture>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePrefecture>();
                    myList = myStore.GetListByRegion(region_id);

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListPrefecturesByRegion: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityCity> GetListCitiesByPrefecture(int prefecture_id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.CitiesByPrefecture, prefecture_id);
            List<IdentityCity> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityCity>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCity>();
                    myList = myStore.GetListByPrefecture(prefecture_id);

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListCitiesByPrefecture: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityTrainLine> GetListTrainLinesByPrefecture(int prefecture_id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.TrainLinesByPrefecture, prefecture_id);
            List<IdentityTrainLine> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityTrainLine>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreTrainLine>();
                    myList = myStore.GetListByPrefecture(prefecture_id);

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListTrainLinesByPrefecture: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityTrainLine> GetListTrainLines()
        {
            var myKey = EnumListCacheKeys.TrainLines;
            List<IdentityTrainLine> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityTrainLine>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreTrainLine>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListTrainLines: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityEmploymentType> GetListEmploymentTypes()
        {
            var myKey = EnumListCacheKeys.EmploymentTypes;
            List<IdentityEmploymentType> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityEmploymentType>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreEmploymentType>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListEmploymentTypes: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentitySalaryType> GetListSalaryTypes()
        {
            var myKey = EnumListCacheKeys.SalaryTypes;
            List<IdentitySalaryType> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentitySalaryType>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreSalaryType>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListSalaryTypes: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityQualification> GetListQualifications()
        {
            var myKey = EnumListCacheKeys.Qualifications;
            List<IdentityQualification> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityQualification>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreQualification>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListQualifications: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityField> GetListFields()
        {
            var myKey = EnumListCacheKeys.Fields;
            List<IdentityField> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityField>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreField>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListFields: " + ex.ToString());
            }

            return myList;
        }
        public static List<IdentitySubField> GetListSubFields()
        {
            var myKey = EnumListCacheKeys.SubFields;
            List<IdentitySubField> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentitySubField>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreSubField>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListSubFields: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityField> F_GetListCount()
        {
            var myKey = EnumListCacheKeys.FieldCounts;
            List<IdentityField> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityField>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreField>();
                    myList = myStore.F_GetListCount();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not F_GetListCounts: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityIndustry> GetListIndustries()
        {
            var myKey = EnumListCacheKeys.Industries;
            List<IdentityIndustry> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityIndustry>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreIndustry>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListIndustries: " + ex.ToString());
            }

            return myList;
        }
        public static List<IdentitySubIndustry> GetListSubIndustries()
        {
            var myKey = EnumListCacheKeys.SubIndustries;
            List<IdentitySubIndustry> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentitySubIndustry>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreIndustry>();
                    myList = myStore.GetListSub();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListSub: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentitySubIndustry> GetListSubByIndustry(int industry_id)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.SubsByIndustry, industry_id);
            List<IdentitySubIndustry> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentitySubIndustry>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreIndustry>();
                    myList = myStore.GetListSubByIndustry(industry_id);

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListSubIndustrysByRegion: " + ex.ToString());
            }

            return myList;
        }

        public static List<ApiResponseAgencyItemModel> GetListAgencies()
        {
            var myKey = EnumListCacheKeys.Agencies;
            List<IdentityAgency> myList = null;
            List<ApiResponseAgencyItemModel> returnList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<ApiResponseAgencyItemModel>>(myKey, out returnList);

                if (returnList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreAgency>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                    {
                        returnList = new List<ApiResponseAgencyItemModel>();

                        foreach (var item in myList)
                        {
                            ApiResponseAgencyItemModel record = new ApiResponseAgencyItemModel();
                            record.id = item.id;
                            record.agency_id = item.agency_id;
                            record.constract_id = item.constract_id;

                            returnList.Add(record);
                        }

                        //Storage to cache
                        cacheProvider.Set(myKey, returnList);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListAgencies: " + ex.ToString());
            }

            return returnList;
        }

        public static List<IdentityNavigation> GetListNavigations()
        {
            var myKey = EnumListCacheKeys.Navigations;
            List<IdentityNavigation> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityNavigation>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();
                    myList = myStore.F_GetList();

                    if (myList.HasData())
                    {
                        //Storage to cache
                        cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListNavigations: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityJapaneseLevel> GetListJapaneseLevels()
        {
            var myKey = EnumListCacheKeys.JapaneseLevels;
            List<IdentityJapaneseLevel> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityJapaneseLevel>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreJapaneseLevel>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListJapaneseLevels: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityCompanySize> GetListCompanySizes()
        {
            var myKey = EnumListCacheKeys.CompanySizes;
            List<IdentityCompanySize> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityCompanySize>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCompanySize>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListCompanySizes: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityMajor> GetListMajors()
        {
            var myKey = EnumListCacheKeys.Majors;
            List<IdentityMajor> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityMajor>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreMajor>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListMajors: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityVisa> GetListVisas()
        {
            var myKey = EnumListCacheKeys.Visas;
            List<IdentityVisa> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityVisa>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreVisa>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListVisas: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentitySuggest> GetListSuggests(IdentitySuggest filter)
        {
            var jsonFilter = JsonConvert.SerializeObject(filter);
            var surfix = Utility.Md5HashingData(jsonFilter);
            var myKey = EnumListCacheKeys.Suggests;
            myKey = myKey + "_" + surfix;

            List<IdentitySuggest> myList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentitySuggest>>(myKey, out myList);

                if (!myList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreSuggest>();
                    myList = myStore.GetList(filter);

                    //Storage to cache
                    cacheProvider.Set(myKey, myList, SystemSettings.DefaultCachingTimeInMinutes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListSuggests: " + ex.ToString());
            }

            return myList;
        }

        public static IdentityFooter GetFooterByLang(string langCode)
        {
            var myKey = string.Format(EnumFormatInfoCacheKeys.Footer, langCode);
            IdentityFooter baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityFooter>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreFooter>();
                    baseInfo = myStore.GetByLangCode(langCode);

                    if (baseInfo != null)
                    {
                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetFooterByLang: " + ex.ToString());
            }

            return baseInfo;
        }
    }
}