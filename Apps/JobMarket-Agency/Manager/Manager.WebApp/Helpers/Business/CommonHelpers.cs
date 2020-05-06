using System;
using System.Collections.Generic;
using Autofac;
using Manager.SharedLibs.Caching.Providers;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using MsSql.AspNet.Identity.Repositories;
using StackExchange.Redis.Extensions.Core;
using System.Linq;
using MsSql.AspNet.Identity.Stores;
using Manager.WebApp.Caching;
using Manager.SharedLibs;
using ApiJobMarket.DB.Sql.Entities;
using Newtonsoft.Json;
using Manager.WebApp.Services;
using Manager.WebApp.Models;
using System.Web.Mvc;
using System.Globalization;

namespace Manager.WebApp.Helpers
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

        public static List<IdentityProjectCategory> GetListProjectCategory()
        {
            var myKey = "PROJECTCATEGORIES";
            List<IdentityProjectCategory> myList = null;
            var langCode = UserCookieManager.GetCurrentLanguageOrDefault();
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityProjectCategory>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreProjectCategory>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }

                if (myList.HasData())
                {
                    foreach (var item in myList)
                    {
                        var matchedLang = item.MyLanguages.Where(x => x.LangCode == langCode).FirstOrDefault();
                        if (matchedLang != null)
                        {
                            item.Name = matchedLang.Name;
                            item.Description = matchedLang.Description;
                            item.UrlFriendly = matchedLang.UrlFriendly;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListProjectCategory: " + ex.ToString());
            }

            return myList;
        }

        public static IdentityProject GetProjectInfo(int id)
        {
            var myKey = string.Format("{0}_{1}", "PROJECT", id);
            IdentityProject baseInfo = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityProject>(myKey, out baseInfo);

                if (baseInfo == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreProject>();
                    baseInfo = myStore.GetDetailById(id);

                    if (baseInfo != null)
                    {
                        //Storage to cache
                        cacheProvider.Set(myKey, baseInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetProjectInfo: " + ex.ToString());
            }

            return baseInfo;
        }

        public static List<IdentityProject> GetProjectFromList(List<int> listIds)
        {
            List<IdentityProject> myList = null;

            try
            {
                if (listIds.HasData())
                {
                    myList = new List<IdentityProject>();

                    foreach (var id in listIds)
                    {
                        var record = GetProjectInfo(id);

                        if (record != null && record.Id > 0)
                            myList.Add(record);
                    }
                }


            }
            catch (Exception ex)
            {
                logger.Error("Could not GetProjectFromList: " + ex.ToString());
            }

            return myList;
        }

        public static IdentityPage GetPageByOperator(string controller, string action)
        {
            var myKey = string.Format("{0}_{1}", controller.ToUpper(), action.ToUpper());
            IdentityPage info = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityPage>(myKey, out info);

                if (info == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePage>();
                    info = myStore.F_Page_GetPageByOperation(controller, action);

                    //cacheProvider.Set(myKey, info);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetPageByOperator: " + ex.ToString());
            }

            return info;
        }

        public static List<IdentityWidget> GetListWidgets()
        {
            var myKey = "WIDGETS";
            List<IdentityWidget> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityWidget>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreWidget>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListWidgets: " + ex.ToString());
            }

            return myList;
        }

        public static List<MsSql.AspNet.Identity.IdentityUser> GetListUser(int parentId)
        {
            var myKey = "USERS_" + parentId;
            List<MsSql.AspNet.Identity.IdentityUser> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<MsSql.AspNet.Identity.IdentityUser>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IIdentityStore>();
                    myList = myStore.GetListUser(parentId);

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListUser: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityProvider> GetListProvider()
        {
            var myKey = "PROVIDERS";
            List<IdentityProvider> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityProvider>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreProvider>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListProvider: " + ex.ToString());
            }

            return myList;
        }

        //public static List<IdentityDevice> GetListDevice()
        //{
        //    var myKey = "DEVICES";
        //    List<IdentityDevice> myList = null;

        //    try
        //    {
        //        //Check from cache first
        //        var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
        //        cacheProvider.Get<List<IdentityDevice>>(myKey, out myList);

        //        if (myList == null)
        //        {
        //            var myStore = GlobalContainer.IocContainer.Resolve<IStoreDevice>();
        //            myList = myStore.GetList();

        //            if (myList.HasData())
        //                //Storage to cache
        //                cacheProvider.Set(myKey, myList, 2);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Could not GetListDevice: " + ex.ToString());
        //    }

        //    return myList;
        //}

        //public static int RegisterNewDevice(IdentityDevice deviceInfo, out bool isNew)
        //{
        //    var newId = 0;
        //    isNew = false;
        //    try
        //    {
        //        var myStore = GlobalContainer.IocContainer.Resolve<IStoreDevice>();

        //        if(deviceInfo != null)
        //        {
        //            newId = myStore.RegisterNewDevice(deviceInfo, out isNew);
        //            if(isNew)
        //            {
        //                //Clear cache
        //                CachingHelpers.ClearDeviceCache();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Could not RegisterNewDevice: " + ex.ToString());
        //    }

        //    return newId;
        //}

        public static List<IdentityProductCategory> GetListProductCategory()
        {
            var myKey = "PRODUCTCATEGORIES";
            List<IdentityProductCategory> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityProductCategory>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreProductCategory>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListProductCategory: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityUnit> GetListUnit()
        {
            var myKey = "UNITS";
            List<IdentityUnit> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityUnit>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreUnit>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListUnit: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityPolicy> GetListPolicy()
        {
            var myKey = string.Format("{0}", "POLICIES");
            List<IdentityPolicy> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityPolicy>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePolicy>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheClient.Add(myKey, myList);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListPolicy: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityCurrency> GetListCurrency()
        {
            var myKey = "CURRENCIES";
            List<IdentityCurrency> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityCurrency>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCurrency>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheClient.Add(myKey, myList);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListCurrency: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityCredit> GetListCredit()
        {
            var myKey = "CREDITS";
            List<IdentityCredit> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityCredit>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCredit>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheClient.Add(myKey, myList);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListCredit: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityPayment> GetListPayment()
        {
            var myKey = string.Format("{0}", "PAYMENTS");
            List<IdentityPayment> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityPayment>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePayment>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheClient.Add(myKey, myList);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListPayment: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityPropertyCategory> GetListPropertyCategory()
        {
            var myKey = string.Format("{0}", "PROPERTYCATEGORIES");
            List<IdentityPropertyCategory> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityPropertyCategory>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePropertyCategory>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not GetListPropertyCategory: {0}", ex.ToString()));
            }

            return myList;
        }

        public static List<IdentityProperty> GetListPropertyByParent(int parentId)
        {
            var myKey = string.Format("{0}_{1}", "PROPERTIES_", parentId);
            List<IdentityProperty> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityProperty>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreProperty>();
                    myList = myStore.GetByCategory(parentId);

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not GetListPropertyByParent [{0}]: {1}", parentId, ex.ToString()));
            }

            return myList;
        }

        public static List<IdentityLanguage> GetListLanguages()
        {
            var myKey = string.Format("{0}", "LANGUAGES");
            List<IdentityLanguage> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityLanguage>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreLanguage>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheClient.Add(myKey, myList);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListLanguages: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityLanguage> GetListLanguageNotExist(List<string> listLangExists)
        {
            var myKey = string.Format("{0}", "LANGUAGES");
            List<IdentityLanguage> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityLanguage>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreLanguage>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheClient.Add(myKey, myList);
                }

                if (myList != null && myList.Count > 0)
                {
                    for (int i = 0; i < myList.Count; i++)
                    {
                        if (listLangExists != null && listLangExists.Count > 0)
                        {
                            var item = myList[i];
                            var itemCheck = listLangExists.Contains(item.LangCode);
                            if (itemCheck)
                            {
                                myList.RemoveAt(i); i--;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListLanguages: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityPropertyCategory> GetSelectedPropertyCategory(List<IdentityPropertyCategory> allCategories, string[] selectedProperties)
        {
            List<IdentityPropertyCategory> returnList = new List<IdentityPropertyCategory>();
            if (allCategories.HasData() && selectedProperties.HasData())
            {
                foreach (var item in allCategories)
                {
                    if (item.Properties.HasData())
                    {
                        var selectedList = new List<IdentityProperty>();
                        foreach (var select in selectedProperties)
                        {
                            var matched = item.Properties.Where(x => x.Id.ToString() == select).FirstOrDefault();
                            if (matched != null)
                            {
                                selectedList.Add(matched);
                            }
                        }

                        if (selectedList.HasData())
                        {
                            var selectedCat = new IdentityPropertyCategory();
                            selectedCat = item;
                            selectedCat.Properties = selectedList;

                            returnList.Add(selectedCat);
                        }
                    }
                }
            }

            return returnList;
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

        public static List<IdentityProcessStatus> GetListProcessStatus(int agency_id)
        {
            var currentAction = "GetListProcessStatus";
            List<IdentityProcessStatus> returnList = null;
            var myKey = EnumListCacheKeys.ProcessStatus + "_" + agency_id;
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
                    cacheProvider.Get<List<IdentityProcessStatus>>(myKey, out returnList);
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
                    var apiReturned = ProcessStatusServices.GetListAsync(agency_id).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            returnList = JsonConvert.DeserializeObject<List<IdentityProcessStatus>>(apiReturned.value.ToString());

                            //if (SystemSettings.MasterCategoryCachingEnable)
                            //{
                            //    if (returnList.HasData())
                            //        cacheProvider.Set(myKey, returnList, SystemSettings.DefaultCachingTimeInMinutes);
                            //}
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

        public static List<IdentityProcessStatus> GetListProcessStatusActive(int agency_id)
        {
            var listResult = GetListProcessStatus(agency_id).Where(s => s.status == 1).ToList();
            return listResult;
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
        public static List<IdentitySubIndustry> GetListSubIndustries()
        {
            var currentAction = "GetListSubIndustries";
            List<IdentitySubIndustry> returnList = null;
            var myKey = EnumListCacheKeys.SubIndustries;
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
                    cacheProvider.Get<List<IdentitySubIndustry>>(myKey, out returnList);
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
                    var apiReturned = IndustryServices.GetListSubAsync().Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            returnList = JsonConvert.DeserializeObject<List<IdentitySubIndustry>>(apiReturned.value.ToString());

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

        public static List<IdentityField> GetListFieldsByEmploymentType(int employment_type)
        {
            var currentAction = "GetListFieldsByEmploymentType";
            List<IdentityField> returnList = null;
            var myKey = String.Concat("{0}-{1}", EnumListCacheKeys.Fields, employment_type.ToString());
            var needToCallApi = true;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                //if (!SystemSettings.MasterCategoryCachingEnable)
                //{
                //    needToCallApi = true;
                //}
                //else
                //{
                //    cacheProvider.Get<List<IdentityField>>(myKey, out returnList);
                //    if (!returnList.HasData())
                //    {
                //        needToCallApi = true;
                //    }
                //    else
                //    {
                //        return returnList;
                //    }
                //}

                if (needToCallApi)
                {
                    var apiReturned = EmploymentTypeServices.GetListFieldsAsync(employment_type).Result;
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

        public static List<IdentitySubField> GetListSubFields()
        {
            var currentAction = "GetListFields";
            List<IdentitySubField> returnList = null;
            var myKey = EnumListCacheKeys.SubFields;
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
                    cacheProvider.Get<List<IdentitySubField>>(myKey, out returnList);
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
                    var apiReturned = SubFieldServices.GetListAsync().Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            returnList = JsonConvert.DeserializeObject<List<IdentitySubField>>(apiReturned.value.ToString());

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

        public static List<IdentityCompanySize> GetListCompanySizes()
        {
            var currentAction = "GetListCompanySizes";
            List<IdentityCompanySize> returnList = null;
            var myKey = EnumListCacheKeys.CompanySizes;
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
                    cacheProvider.Get<List<IdentityCompanySize>>(myKey, out returnList);
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
                    var apiReturned = CompanySizeServices.GetListAsync().Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            returnList = JsonConvert.DeserializeObject<List<IdentityCompanySize>>(apiReturned.value.ToString());

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
            List<IdentityCountry> returnList = new List<IdentityCountry>();
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

        public static List<IdentityRegion> GetListRegions(List<int> listIds)
        {
            var currentAction = "GetListRegionsFromListIds";
            List<IdentityRegion> returnList = new List<IdentityRegion>();
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
            List<IdentityRegion> returnList = new List<IdentityRegion>();
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
            List<IdentityPrefecture> returnList = new List<IdentityPrefecture>();
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
            List<IdentityCity> returnList = new List<IdentityCity>();
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

        public static List<SelectListItem> GetCompanyManagers(int agency_id, string language_code)
        {
            var currentAction = "GetCompanyManagers";
            var model = new ApiCompanyModel
            {
                agency_id = agency_id,
                page_index = 1,
                page_size = 1000
            };
            var listDataCompany = AgencyServices.GetCompaniesAsync(model).Result;
            List<IdentityCompany> listCompany = new List<IdentityCompany>();
            List<SelectListItem> listResult = new List<SelectListItem>();
            try
            {
                if (listDataCompany != null && listDataCompany.value != null)
                {
                    if (!string.IsNullOrEmpty(listDataCompany.value.ToString()))
                    {
                        listCompany = JsonConvert.DeserializeObject<List<IdentityCompany>>(listDataCompany.value.ToString());
                    }
                }

                if (listCompany.HasData())
                {
                    foreach (var record in listCompany)
                    {
                        var item = new SelectListItem();
                        item.Value = record.id.ToString();
                        var company_name = "";
                        if (record.LangList.HasData())
                        {
                            var companyLang = record.LangList.FirstOrDefault(s => s.language_code == language_code);
                            if (companyLang != null) company_name = companyLang.company_name;
                        }
                        if (string.IsNullOrEmpty(company_name))
                        {
                            company_name = record.company_name;
                        }
                        item.Text = company_name;
                        listResult.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return listResult;
        }

        public static List<IdentityStation> GetListStations(List<int> listIds)
        {
            var currentAction = "GetListStations";
            List<IdentityStation> returnList = new List<IdentityStation>();
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
        public static List<string> GetEmailMessageIdSynchronized(int agencyId, int staffId)
        {
            var currentAction = "GetEmailMessageIdSynchronized";
            List<string> returnList = null;
            var myKey = string.Format(EnumFormatInfoCacheKeys.EmailIdsSynchronized, agencyId, staffId);

            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<string>>(myKey, out returnList);
                if (!returnList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreEmailMessage>();
                    returnList = myStore.GetSynchronizedIds(agencyId, staffId);

                    if (returnList.HasData())
                    {
                        cacheProvider.Set(myKey, returnList);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityEmailServer> GetListEmailServers(int agencyId)
        {
            var currentAction = "GetListEmailServers";
            List<IdentityEmailServer> returnList = null;
            var myKey = string.Format(EnumFormatInfoCacheKeys.EmailServers, agencyId);

            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityEmailServer>>(myKey, out returnList);
                if (!returnList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreEmailServer>();
                    returnList = myStore.GetListByAgency(agencyId);

                    if (returnList.HasData())
                    {
                        foreach (var item in returnList)
                        {
                            item.ReceivingConfig = JsonConvert.DeserializeObject<IdentityEmailServerPOPConfig>(item.POPConfig);
                            item.SendingConfig = JsonConvert.DeserializeObject<IdentityEmailServerSMTPConfig>(item.SMTPConfig);
                        }
                        cacheProvider.Set(myKey, returnList, SystemSettings.DefaultCachingTimeInMinutes);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return returnList;
        }

        public static List<IdentityEmailSetting> GetListEmailSettings(int agencyId, int staffId)
        {
            var currentAction = "GetListEmailSettings";
            List<IdentityEmailSetting> returnList = null;
            var myKey = string.Format(EnumFormatInfoCacheKeys.EmailSettings, agencyId, staffId);

            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityEmailSetting>>(myKey, out returnList);

                if (!returnList.HasData())
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreEmailServer>();
                    returnList = myStore.GetEmailSettingsByStaff(agencyId, staffId);

                    if (returnList.HasData())
                        cacheProvider.Set(myKey, returnList, SystemSettings.DefaultCachingTimeInMinutes);
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