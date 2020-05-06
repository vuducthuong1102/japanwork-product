using System;
using System.Collections.Generic;
using Autofac;
using Manager.SharedLibs.Caching.Providers;
using Manager.SharedLibs.Logging;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using System.Web;
using System.Linq;
using Manager.WebApp.Caching;
using MsSql.AspNet.Identity.Stores;
using Manager.SharedLibs;
using Manager.WebApp.Models;
using ApiJobMarket.DB.Sql.Entities;

namespace Manager.WebApp.Helpers
{
    public class FrontendHelpers
    {
        private static readonly ILog logger = LogProvider.For<FrontendHelpers>();

        public static string GenerateCvDetailAddress(ManageCvPreviewAddressModel model, string currentLang = "vi-VN")
        {
            var addStr = string.Empty;
            var seperatorChars = ", ";
            if (model != null && model.AddressInfo != null)
            {
                addStr = model.country_name;

                if (currentLang == "ja-JP")
                {
                    if (!string.IsNullOrEmpty(addStr))
                    {
                        addStr += seperatorChars;
                    }

                    if (model.AddressInfo.region_info != null && !string.IsNullOrEmpty(model.AddressInfo.region_info.region))
                    {
                        addStr += model.AddressInfo.region_info.region + seperatorChars;
                    }

                    if (model.AddressInfo.prefecture_info != null && !string.IsNullOrEmpty(model.AddressInfo.prefecture_info.prefecture))
                    {
                        addStr += model.AddressInfo.prefecture_info.prefecture + seperatorChars;
                    }

                    if (model.AddressInfo.city_info != null && !string.IsNullOrEmpty(model.AddressInfo.city_info.city))
                    {
                        addStr += model.AddressInfo.city_info.city + seperatorChars;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(addStr))
                    {
                        addStr += seperatorChars;
                    }

                    if (model.AddressInfo.region_info != null && !string.IsNullOrEmpty(model.AddressInfo.region_info.furigana))
                    {
                        addStr += model.AddressInfo.region_info.furigana + seperatorChars;
                    }

                    if (model.AddressInfo.prefecture_info != null && !string.IsNullOrEmpty(model.AddressInfo.prefecture_info.furigana))
                    {
                        addStr += model.AddressInfo.prefecture_info.furigana + seperatorChars;
                    }

                    if (model.AddressInfo.city_info != null && !string.IsNullOrEmpty(model.AddressInfo.city_info.furigana))
                    {
                        addStr += model.AddressInfo.city_info.furigana + seperatorChars;
                    }
                }

            }

            return addStr;
        }

        public static string GenerateCvDetailAddressFull(ManageCvPreviewAddressModel model, string currentLang = "vi-VN")
        {
            var addStr = string.Empty;
            var seperatorChars = ", ";
            if (model != null && model.AddressInfo != null)
            {
                addStr = model.country_name;

                if (currentLang == "ja-JP")
                {
                    if (!string.IsNullOrEmpty(addStr))
                    {
                        addStr += seperatorChars;
                    }

                    if (model.AddressInfo.region_info != null && !string.IsNullOrEmpty(model.AddressInfo.region_info.region))
                    {
                        addStr += model.AddressInfo.region_info.region + seperatorChars;
                    }

                    if (model.AddressInfo.prefecture_info != null && !string.IsNullOrEmpty(model.AddressInfo.prefecture_info.prefecture))
                    {
                        addStr += model.AddressInfo.prefecture_info.prefecture + seperatorChars;
                    }

                    if (model.AddressInfo.city_info != null && !string.IsNullOrEmpty(model.AddressInfo.city_info.city))
                    {
                        addStr += model.AddressInfo.city_info.city + seperatorChars;
                    }

                    if (!string.IsNullOrEmpty(model.AddressInfo.detail))
                    {
                        addStr += model.AddressInfo.detail;

                        if (!string.IsNullOrEmpty(model.AddressInfo.furigana))
                        {
                            addStr += string.Format("({0})", model.AddressInfo.furigana);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.AddressInfo.furigana))
                        {
                            addStr += string.Format("({0})", model.AddressInfo.furigana);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(addStr))
                    {
                        addStr += seperatorChars;
                    }

                    if (model.AddressInfo.region_info != null && !string.IsNullOrEmpty(model.AddressInfo.region_info.furigana))
                    {
                        addStr += model.AddressInfo.region_info.furigana + seperatorChars;
                    }

                    if (model.AddressInfo.prefecture_info != null && !string.IsNullOrEmpty(model.AddressInfo.prefecture_info.furigana))
                    {
                        addStr += model.AddressInfo.prefecture_info.furigana + seperatorChars;
                    }

                    if (model.AddressInfo.city_info != null && !string.IsNullOrEmpty(model.AddressInfo.city_info.furigana))
                    {
                        addStr += model.AddressInfo.city_info.furigana + seperatorChars;
                    }

                    if (!string.IsNullOrEmpty(model.AddressInfo.detail))
                    {
                        addStr += model.AddressInfo.detail;

                        if (!string.IsNullOrEmpty(model.AddressInfo.furigana))
                        {
                            addStr += string.Format("({0})", model.AddressInfo.furigana);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.AddressInfo.furigana))
                        {
                            addStr += string.Format("({0})", model.AddressInfo.furigana);
                        }
                    }
                }

            }

            return addStr;
        }

        public static string SubstringBracket(string value)
        {
            var result1 = "";
            if (!string.IsNullOrEmpty(value))
            {
                var hasBracket = false;
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == '（')
                    {
                        hasBracket = true;
                        result1 = result1.Trim();
                    }
                    if(hasBracket == false)
                    {
                        result1 += value[i];
                    }
                    if (value[i] == '）')
                    {
                        hasBracket = false;
                    }
                }
            }
            var result = "";
            if (!string.IsNullOrEmpty(result1))
            {
                var hasBracket = false;
                for (int i = 0; i < result1.Length; i++)
                {
                    if (result1[i] == '(')
                    {
                        hasBracket = true;
                        result = result.Trim();
                    }
                    if (hasBracket == false)
                    {
                        result += result1[i];
                    }
                    if (result1[i] == ')')
                    {
                        hasBracket = false;
                    }
                }
            }
            if (!string.IsNullOrEmpty(result)) result = result.Trim().TrimEnd(',');
            return result;
        }
        public static string GenerateCompanyAddress(IdentityCompany model, string currentLang = "vi-VN")
        {
            var addStr = string.Empty;
            var seperatorChars = " ";
            try
            {
                if (model != null && model.address_info != null)
                {
                    if (currentLang == "ja-JP")
                    {
                        if (!string.IsNullOrEmpty(addStr))
                        {
                            addStr += seperatorChars;
                        }

                        if (model.address_info.prefecture_info != null && !string.IsNullOrEmpty(model.address_info.prefecture_info.prefecture))
                        {
                            addStr += model.address_info.prefecture_info.prefecture + seperatorChars;
                        }

                        if (model.address_info.city_info != null && !string.IsNullOrEmpty(model.address_info.city_info.city))
                        {
                            addStr += model.address_info.city_info.city + seperatorChars;
                        }

                        if (!string.IsNullOrEmpty(model.address_detail))
                        {
                            addStr += model.address_detail;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(addStr))
                        {
                            addStr += seperatorChars;
                        }

                        if (model.address_info.prefecture_info != null && !string.IsNullOrEmpty(model.address_info.prefecture_info.furigana))
                        {
                            addStr += model.address_info.prefecture_info.furigana + seperatorChars;
                        }

                        if (model.address_info.city_info != null && !string.IsNullOrEmpty(model.address_info.city_info.furigana))
                        {
                            addStr += model.address_info.city_info.furigana + seperatorChars;
                        }

                        if (!string.IsNullOrEmpty(model.address_detail))
                        {
                            addStr += model.address_detail;
                        }
                    }

                }

                if(addStr[addStr.Length] > 2)
                {
                    if (addStr[addStr.Length - 2] == ',')
                    {
                        addStr = addStr.Substring(0, addStr.Length - 2);
                    }
                }                
            }
            catch (Exception ex)
            {
                logger.Error("Could not GenerateCompanyAddress: " + ex.ToString());
            }
            return addStr;
        }
        public static string GenerateCvDetailAddressPrint(ManageCvPreviewAddressModel model)
        {
            var addStr = string.Empty;
            var seperatorChars = " - ";
            if (model != null && model.AddressInfo != null)
            {
                addStr = model.country_name;

                if (!string.IsNullOrEmpty(addStr))
                {
                    addStr += seperatorChars;
                }

                if (model.AddressInfo.region_info != null && !string.IsNullOrEmpty(model.AddressInfo.region_info.region))
                {
                    addStr += model.AddressInfo.region_info.region;
                }

                if (model.AddressInfo.prefecture_info != null && !string.IsNullOrEmpty(model.AddressInfo.prefecture_info.prefecture))
                {
                    addStr += seperatorChars + model.AddressInfo.prefecture_info.prefecture;
                }

                if (model.AddressInfo.city_info != null && !string.IsNullOrEmpty(model.AddressInfo.city_info.city))
                {
                    addStr += seperatorChars + model.AddressInfo.city_info.city;
                }

            }

            return addStr;
        }
        public static string GenerateCvDetailAddressPrintFurigana(ManageCvPreviewAddressModel model)
        {
            var addStr = string.Empty;
            var seperatorChars = " - ";
            if (model != null && model.AddressInfo != null)
            {
                addStr = model.country_name;

                if (!string.IsNullOrEmpty(addStr))
                {
                    addStr += seperatorChars;
                }

                if (model.AddressInfo.region_info != null && !string.IsNullOrEmpty(model.AddressInfo.region_info.furigana))
                {
                    addStr += model.AddressInfo.region_info.furigana;
                }

                if (model.AddressInfo.prefecture_info != null && !string.IsNullOrEmpty(model.AddressInfo.prefecture_info.furigana))
                {
                    addStr += seperatorChars + model.AddressInfo.prefecture_info.furigana;
                }

                if (model.AddressInfo.city_info != null && !string.IsNullOrEmpty(model.AddressInfo.city_info.furigana))
                {
                    addStr += seperatorChars + model.AddressInfo.city_info.furigana;
                }

            }

            return addStr;
        }
        public static string GenerateJobDetailLink(IdentityJob info, string language_code = "vi-VN")
        {
            var friendlyUrlFormat = "/job/detail/{0}/{1}";
            var myUrl = string.Empty;
            var urlFriendly = string.Empty;
            if (info.Job_translations.HasData())
            {
                var matchedLang = info.Job_translations.Where(x => x.language_code == language_code).FirstOrDefault();
                if (matchedLang != null)
                    urlFriendly = matchedLang.friendly_url;
            }

            if (info != null)
            {
                if (!string.IsNullOrEmpty(urlFriendly) && urlFriendly.Contains("http"))
                {
                    return urlFriendly;
                }

                myUrl = string.Format(friendlyUrlFormat, info.id, urlFriendly);
            }

            return myUrl;
        }

        public static string RenderRangeIntegerNumber(int fromNum, int toNum, string seperator = " - ")
        {
            var returnStr = string.Empty;
            if (fromNum > 0)
            {
                if (toNum > 0)
                    returnStr = string.Format("{0}{1}{2}", fromNum.FormatWithComma(), seperator, toNum.FormatWithComma());
                else
                    returnStr = string.Format("{0}", fromNum.FormatWithComma());
            }
            else
            {
                if (toNum > 0)
                    returnStr = string.Format("{0}", toNum.FormatWithComma());
            }

            return returnStr;
        }

        public static string RenderRangeYear(DateTime? fromDate, DateTime? toDate, string seperator = " - ")
        {
            var returnStr = string.Empty;
            if (fromDate != null)
            {
                if (toDate != null)
                    returnStr = string.Format("{0}{1}{2}", fromDate.DateTimeQuestToString("yyyy"), seperator, toDate.DateTimeQuestToString("yyyy"));
                else
                    returnStr = string.Format("{0}", fromDate.DateTimeQuestToString("yyyy"));
            }
            else
            {
                if (toDate != null)
                    returnStr = string.Format("{0}", toDate.DateTimeQuestToString("yyyy"));
            }

            return returnStr;
        }

        public static string RenderJobWorkingAddress(IdentityJobAddress workAddress, bool canShowDetail = true)
        {
            var addressDetail = string.Empty;
            if (workAddress != null)
            {
                //if (workAddress.region_info != null)
                //{
                //    if (string.IsNullOrEmpty(addressDetail))
                //    {
                //        addressDetail = string.Format("{0} ({1})", workAddress.region_info.region, workAddress.region_info.furigana);
                //    }
                //    else
                //    {
                //        addressDetail += ", " + string.Format("{0} ({1})", workAddress.region_info.region, workAddress.region_info.furigana);
                //    }
                //}

                if (workAddress.prefecture_info != null)
                {
                    addressDetail = workAddress.prefecture_info.furigana;
                }

                if (workAddress.city_info != null)
                {
                    if (string.IsNullOrEmpty(addressDetail))
                    {
                        addressDetail = string.Format("{0}", workAddress.city_info.furigana);
                    }
                    else
                    {
                        addressDetail += " - " + string.Format("{0}", workAddress.city_info.furigana);
                    }
                }

                if (canShowDetail)
                {
                    if (string.IsNullOrEmpty(addressDetail))
                    {
                        if (!string.IsNullOrEmpty(workAddress.detail))
                        {
                            if (!string.IsNullOrEmpty(workAddress.furigana))
                            {
                                addressDetail = string.Format("{0} ({1})", workAddress.detail, workAddress.furigana);
                            }
                            else
                            {
                                addressDetail = string.Format("{0}", workAddress.detail);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(workAddress.furigana))
                            {
                                addressDetail = string.Format("{0}", workAddress.furigana);
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(workAddress.detail))
                        {
                            if (!string.IsNullOrEmpty(workAddress.furigana))
                            {
                                addressDetail += " - " + string.Format("{0} ({1})", workAddress.detail, workAddress.furigana);
                            }
                            else
                            {
                                addressDetail += " - " + string.Format("{0}", workAddress.detail);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(workAddress.furigana))
                            {
                                addressDetail = " - " + string.Format("{0}", workAddress.furigana);
                            }
                        }
                    }
                }
            }

            return addressDetail;
        }

        public static string RenderJobCompanyAddress(IdentityJob jobInfo, IdentityCompany companyInfo)
        {
            var companyAddress = string.Empty;

            if (companyInfo.address_info != null)
            {
                if (companyInfo.address_info.prefecture_info != null)
                {
                    companyAddress = companyInfo.address_info.prefecture_info.furigana;
                }

                if (companyInfo.address_info.city_info != null)
                {
                    if (string.IsNullOrEmpty(companyAddress))
                    {
                        companyAddress = string.Format("{0}", companyInfo.address_info.city_info.furigana);
                    }
                    else
                    {
                        companyAddress += " - " + string.Format("{0}", companyInfo.address_info.city_info.furigana);
                    }
                }
            }

            if (jobInfo.view_company && companyInfo.address_info != null)
            {
                var companyAddressInfo = companyInfo.address_info;
                if (string.IsNullOrEmpty(companyAddress))
                {
                    if (!string.IsNullOrEmpty(companyInfo.address_detail))
                    {
                        if (!string.IsNullOrEmpty(companyInfo.address_furigana))
                        {
                            companyAddress = string.Format("{0} ({1})", companyInfo.address_detail, companyInfo.address_furigana);
                        }
                        else
                        {
                            companyAddress = string.Format("{0}", companyInfo.address_detail);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(companyInfo.address_furigana))
                        {
                            companyAddress = string.Format("{0}", companyInfo.address_furigana);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(companyInfo.address_detail))
                    {
                        if (!string.IsNullOrEmpty(companyInfo.address_furigana))
                        {
                            companyAddress += " - " + string.Format("{0} ({1})", companyInfo.address_detail, companyInfo.address_furigana);
                        }
                        else
                        {
                            companyAddress += " - " + string.Format("{0}", companyInfo.address_detail);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(companyInfo.address_furigana))
                        {
                            companyAddress = " - " + string.Format("{0}", companyInfo.address_furigana);
                        }
                    }
                }

            }

            return companyAddress;
        }

        public static List<IdentityNavigation> GetAllNavigations()
        {
            var myKey = "NAVIGATIONS";
            List<IdentityNavigation> returnList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityNavigation>>(myKey, out returnList);

                //if (returnList == null)
                //{
                //    var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();
                //    var myList = myStore.F_GetList();

                //    if (myList.HasData())
                //    {
                //        returnList = myList.Where(x => x.ParentId <= 0).ToList();

                //        if (returnList.HasData())
                //        {
                //            foreach (var item in returnList)
                //            {
                //                FindNavigationItemChilds(item, myList);
                //            }
                //        }
                //        //Storage to cache
                //        cacheProvider.Set(myKey, returnList);
                //    }                       
                //}
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetAllNavigations: " + ex.ToString());
            }

            return returnList;
        }

        public static IdentityPost GetPostDetailById(int id)
        {
            var myKey = string.Format("{0}_{1}", "POST", id);
            IdentityPost info = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<IdentityPost>(myKey, out info);

                if (info == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePost>();
                    info = myStore.F_GetDetailById(id);

                    if (info != null && info.Id > 0)
                        //Storage to cache
                        cacheProvider.Set(myKey, info);
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not GetPostDetailById [{0}] because: {1}", id, ex.ToString()));
            }

            return info;
        }

        public static string GeneratePostDetailLink(IdentityPost info, string langCode = "en-US")
        {
            var friendlyUrlFormat = "/article/detail/{0}/{1}";
            var myUrl = string.Empty;
            var urlFriendly = info.UrlFriendly;
            if (info.MyLanguages.HasData())
            {
                var matchedLang = info.MyLanguages.Where(x => x.LangCode == langCode).FirstOrDefault();
                if (matchedLang != null)
                    urlFriendly = matchedLang.UrlFriendly;
            }

            if (info != null)
            {
                if (!string.IsNullOrEmpty(urlFriendly) && urlFriendly.Contains("http"))
                {
                    return urlFriendly;
                }

                myUrl = string.Format(friendlyUrlFormat, info.Id, urlFriendly);
            }

            return myUrl;
        }

        public static string GenerateProjectDetailLink(IdentityProject info, string langCode = "en-US")
        {
            var friendlyUrlFormat = "/ourproject/detail/{0}/{1}";
            var myUrl = string.Empty;

            var urlFriendly = info.UrlFriendly;
            if (info.MyLanguages.HasData())
            {
                var matchedLang = info.MyLanguages.Where(x => x.LangCode == langCode).FirstOrDefault();
                if (matchedLang != null)
                    urlFriendly = matchedLang.UrlFriendly;
            }

            if (info != null)
            {
                if (!string.IsNullOrEmpty(urlFriendly) && urlFriendly.Contains("http"))
                {
                    return urlFriendly;
                }

                myUrl = string.Format(friendlyUrlFormat, info.Id, urlFriendly);
            }

            return myUrl;
        }

        public static List<IdentityPost> GetGroupArticlesByFilter(IdentityPost filter)
        {
            var myKey = string.Format("GROUP_ARTICLES_{0}_{1}", filter.CategoryId, filter.LangCode);
            List<IdentityPost> returnList = null;
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityPost>>(myKey, out returnList);

                if (returnList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePost>();
                    returnList = myStore.F_GetByCategory(filter);

                    if (returnList.HasData())
                    {
                        //Storage to cache
                        cacheProvider.Set(myKey, returnList);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetGroupArticlesByFilter: " + ex.ToString());
            }

            return returnList;
        }
        public static List<IdentityProject> GetNewestProjects(IdentityProject filter)
        {
            var myKey = string.Format("PROJECTS_NEWEST");
            List<IdentityProject> returnList = null;
            List<IdentityProject> myList = null;
            var langCode = UserCookieManager.GetCurrentLanguageOrDefault();

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityProject>>(myKey, out myList);

                if (returnList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreProject>();
                    myList = myStore.F_Project_GetNewest(filter);

                    if (myList.HasData())
                    {
                        var listIds = myList.Select(x => x.Id).ToList();

                        returnList = CommonHelpers.GetProjectFromList(listIds);

                        if (returnList.HasData())
                        {
                            foreach (var item in returnList)
                            {
                                var matchedLang = item.MyLanguages.Where(x => x.LangCode == langCode).FirstOrDefault();
                                if (matchedLang != null)
                                {
                                    item.Title = matchedLang.Title;
                                    item.Description = matchedLang.Description;
                                    item.BodyContent = matchedLang.BodyContent;
                                    item.UrlFriendly = matchedLang.UrlFriendly;
                                }
                            }
                        }

                        if (myList.HasData())
                            //Storage to cache
                            cacheProvider.Set(myKey, myList, 5);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetNewestProjects: " + ex.ToString());
            }

            return returnList;
        }
        public static List<int> ConvertStringToList(string value)
        {
            List<int> results = new List<int>();
            if (!string.IsNullOrEmpty(value))
            {
                var listValue = value.Split(',');
                if (listValue.HasData())
                {
                    foreach (var item in listValue)
                    {
                        results.Add(Utils.ConvertToInt32(item));
                    }
                }
            }
            return results;
        }
        #region Navigation Helpers

        public static void FindNavigationItemChilds(IdentityNavigation currentItem, List<IdentityNavigation> navList)
        {
            currentItem.SubNavigation = navList.Where(x => x.ParentId == currentItem.Id).ToList();
            if (currentItem.SubNavigation.HasData())
            {
                foreach (var item in currentItem.SubNavigation)
                {
                    FindNavigationItemChilds(item, navList);
                }
            }
        }

        public static List<IdentityNavigation> GetCurrentNavigationByLang(List<IdentityNavigation> allNavs)
        {
            try
            {
                var currentLangCode = UserCookieManager.GetCurrentLanguageOrDefault();
                if (allNavs.HasData())
                {
                    foreach (var item in allNavs)
                    {
                        var langItem = item.LangList.Where(x => x.NavigationId == item.Id && x.LangCode == currentLangCode).FirstOrDefault();
                        if (langItem != null)
                        {
                            item.CurrentTitleLang = langItem.Title;
                            item.CurrentAbsoluteUriLang = langItem.AbsoluteUri;
                        }
                        else
                        {
                            item.CurrentTitleLang = item.Title;
                            item.CurrentAbsoluteUriLang = item.AbsoluteUri;
                        }

                        if (item.SubNavigation != null && item.SubNavigation.Count() > 0)
                            item.SubNavigation = GetCurrentNavigationByLang(item.SubNavigation);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetCurrentNavigationByLang because :" + ex.ToString());
            }

            return allNavs;
        }

        private static bool CheckCurrentAction(IdentityNavigation mi, string actionName, string controllerName)
        {
            if (
                !string.IsNullOrEmpty(mi.Action)
                && !string.IsNullOrEmpty(mi.Controller)
                && !string.IsNullOrEmpty(actionName)
                && !string.IsNullOrEmpty(controllerName)
                && mi.Action.Equals(actionName, StringComparison.InvariantCultureIgnoreCase)
                && mi.Controller.Equals(controllerName, StringComparison.InvariantCultureIgnoreCase)
                )
            {
                return true;
            }

            return false;
        }

        public static bool CheckCurrentAction(IdentityNavigation mi)
        {
            if (HttpContext.Current == null)
                return false;

            string controllerName = null;
            string actionName = null;

            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            if (routeValues != null)
            {
                if (routeValues.ContainsKey("action"))
                {
                    actionName = routeValues["action"].ToString();
                }
                if (routeValues.ContainsKey("controller"))
                {
                    controllerName = routeValues["controller"].ToString();
                }
            }

            return CheckCurrentAction(mi, actionName, controllerName);
        }

        public static bool CheckCurrentGroup(IdentityNavigation gmi)
        {
            if (CheckCurrentAction(gmi))
            {
                return true;
            }

            if (gmi.SubNavigation != null && gmi.SubNavigation.Any())
            {
                foreach (var smi in gmi.SubNavigation)
                {
                    if (CheckCurrentGroup(smi))
                        return true;
                }
            }

            return false;
        }

        //public static List<IdentityFooter> GetAllFooters()
        //{
        //    var myKey = "FOOTERS";
        //    List<IdentityFooter> returnList = null;
        //    try
        //    {
        //        //Check from cache first
        //        var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
        //        cacheProvider.Get<List<IdentityFooter>>(myKey, out returnList);

        //        if (returnList == null)
        //        {
        //            var myStore = GlobalContainer.IocContainer.Resolve<IStoreFooter>();
        //            returnList = myStore.GetAll();

        //            //Storage to cache
        //            cacheProvider.Set(myKey, returnList);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Could not GetAllFooters: " + ex.ToString());
        //    }

        //    return returnList;
        //}

        #endregion        
    }
}