using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using MySite.Logging;
using ApiJobMarket.DB.Sql.Entities;
using MySite.Caching.Providers;
using Autofac;
using MySite.Services;
using Newtonsoft.Json;
using MySite.Caching;
using System.Text.RegularExpressions;
using MySite.Models.Cv;
using System.Globalization;
using MySite.Settings;
using MySite.ShareLibs;

namespace MySite.Helpers
{
    public class FrontendHelpers
    {
        private static readonly ILog logger = LogProvider.For<FrontendHelpers>();

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

                if (returnList == null)
                {
                    var myList = NavigationServices.GetListAsync().Result;
                    if (myList != null)
                    {
                        List<IdentityNavigation> listNavigation = JsonConvert.DeserializeObject<List<IdentityNavigation>>(myList.value.ToString());
                        if (listNavigation.HasData())
                        {

                            returnList = listNavigation.Where(x => x.ParentId <= 0).ToList();

                            if (returnList.HasData())
                            {
                                foreach (var item in returnList)
                                {
                                    FindNavigationItemChilds(item, listNavigation);
                                }
                            }
                            //Storage to cache
                            cacheProvider.Set(myKey, returnList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetAllNavigations: " + ex.ToString());
            }

            return returnList;
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

        public static string KeepSomeCharactersInWords(string rawStr, bool isHidden = true, string replaceCharacter = "x", int lengthOfStringToKeep = 4)
        {
            if (!isHidden)
                return rawStr;

            var totalChars = rawStr.Length;
            var returnStr = string.Empty;
            if (totalChars <= 0)
            {
                return string.Empty;
            }
            else
            {
                if (totalChars > lengthOfStringToKeep)
                {
                    returnStr = Regex.Replace(rawStr.Substring(0, totalChars - lengthOfStringToKeep), @"\w", replaceCharacter);
                    var lastCharacters = rawStr.Substring(rawStr.Length - lengthOfStringToKeep, lengthOfStringToKeep);
                    returnStr += lastCharacters;
                }
                else
                {
                    returnStr = Regex.Replace(rawStr, @"\w", replaceCharacter);
                }
            }

            return returnStr;
        }

        public static string KeepSomeWordsInSentense(string rawStr, bool isHidden = true, string replaceCharacter = "x", int numberOfWordsToKeep = 1)
        {
            if (!isHidden)
                return rawStr;

            if (string.IsNullOrEmpty(rawStr))
                return string.Empty;

            rawStr = rawStr.Trim();
            var totalWords = 0;

            string[] allWords = null;
            char[] delimiters = new char[] { ' ', '\r', '\n' };

            allWords = rawStr.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            totalWords = allWords.Length;

            List<string> myReturnWords = new List<string>();
            if (totalWords <= 0)
            {
                return string.Empty;
            }
            else
            {
                if (totalWords > numberOfWordsToKeep)
                {
                    var beginWords = allWords.Take(totalWords - numberOfWordsToKeep).Select(s => Regex.Replace(s, @"\w", replaceCharacter)).ToList();

                    var lastWords = allWords.Skip(totalWords - numberOfWordsToKeep).Take(numberOfWordsToKeep).ToList();

                    myReturnWords.AddRange(beginWords);
                    myReturnWords.AddRange(lastWords);
                }
                else
                {
                    var beginWords = allWords.Select(s => Regex.Replace(s, @"\w", replaceCharacter)).ToList();
                    myReturnWords.AddRange(beginWords);
                }
            }

            if (myReturnWords.HasData())
                return myReturnWords.Aggregate((x, y) => x + " " + y);

            return string.Empty;
        }

        public static bool NeedToKeepSomeWordsInSentense(string rawStr, int numberOfWordsToKeep = 10)
        {
            if (string.IsNullOrEmpty(rawStr))
                return false;

            rawStr = rawStr.Trim();
            var totalWords = 0;

            string[] allWords = null;
            char[] delimiters = new char[] { ' ', '\r', '\n' };

            allWords = rawStr.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            totalWords = allWords.Length;

            List<string> myReturnWords = new List<string>();
            if (totalWords <= 0)
            {
                return false;
            }
            else
            {
                if (totalWords > numberOfWordsToKeep)
                {
                    return true;
                }                
            }

            return false;
        }

        public static string KeepSomeWordsInSentense(string rawStr, bool isHidden = true, int numberOfWordsToKeep = 10)
        {
            if (!isHidden)
                return rawStr;

            if (string.IsNullOrEmpty(rawStr))
                return string.Empty;

            rawStr = rawStr.Trim();
            var totalWords = 0;

            string[] allWords = null;
            char[] delimiters = new char[] { ' ', '\r', '\n' };

            allWords = rawStr.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            totalWords = allWords.Length;

            List<string> myReturnWords = new List<string>();
            if (totalWords <= 0)
            {
                return string.Empty;
            }
            else
            {
                if (totalWords > numberOfWordsToKeep)
                {
                    var beginWords = allWords.Take(numberOfWordsToKeep).Select(s => s).ToList();

                    //var lastWords = allWords.Skip(totalWords - numberOfWordsToKeep).Take(numberOfWordsToKeep).ToList();

                    myReturnWords.AddRange(beginWords);
                   // myReturnWords.AddRange(lastWords);
                }
                else
                {
                    var beginWords = allWords.Select(s => s).ToList();
                    myReturnWords.AddRange(beginWords);
                }
            }

            if (myReturnWords.HasData())
                return myReturnWords.Aggregate((x, y) => x + " " + y);

            return string.Empty;
        }

        public static IdentityFooter GetFooter()
        {
            var currentAction = "GetFooter";
            IdentityFooter info = null;
            var languageCode = CultureInfo.CurrentCulture.ToString();

            var myKey = string.Format(EnumListCacheKeys.EmploymentTypes, languageCode);

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
                    cacheProvider.Get<IdentityFooter>(myKey, out info);
                    if (info == null)
                    {
                        needToCallApi = true;
                    }
                    else
                    {
                        return info;
                    }
                }

                if (needToCallApi)
                {
                    var apiReturned = FooterServices.GetFooterAsync().Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            info = JsonConvert.DeserializeObject<IdentityFooter>(apiReturned.value.ToString());

                            if (SystemSettings.MasterCategoryCachingEnable)
                            {
                                if (info != null)
                                    cacheProvider.Set(myKey, info, SystemSettings.DefaultCachingTimeInMinutes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when [{0}] due to {1}", currentAction, ex.ToString()));
            }

            return info;
        }
    }
}