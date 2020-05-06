//using System;
//using System.Collections.Generic;
//using System.Web;
//using System.Linq;
//using MySite.Logging;
//using ApiJobMarket.DB.Sql.Entities;

//namespace MySite.Helpers
//{
//    public class FrontendHelpers
//    {
//        private static readonly ILog logger = LogProvider.For<FrontendHelpers>();

//        public static string GenerateJobDetailLink(IdentityJob info, string language_code = "vi-VN")
//        {
//            var friendlyUrlFormat = "/job/detail/{0}/{1}";
//            var myUrl = string.Empty;
//            var urlFriendly = string.Empty;
//            if (info.Job_translations.HasData())
//            {
//                var matchedLang = info.Job_translations.Where(x => x.language_code == language_code).FirstOrDefault();
//                if (matchedLang != null)
//                    urlFriendly = matchedLang.friendly_url;
//            }

//            if (info != null)
//            {
//                if (!string.IsNullOrEmpty(urlFriendly) && urlFriendly.Contains("http"))
//                {
//                    return urlFriendly;
//                }

//                myUrl = string.Format(friendlyUrlFormat, info.id, urlFriendly);
//            }

//            return myUrl;
//        }

//        public static string RenderRangeIntegerNumber(int fromNum, int toNum, string seperator = " - ")
//        {
//            var returnStr = string.Empty;
//            if (fromNum > 0)
//            {
//                if (toNum > 0)
//                    returnStr = string.Format("{0}{1}{2}", fromNum.FormatWithComma(), seperator, toNum.FormatWithComma());
//                else
//                    returnStr = string.Format("{0}", fromNum.FormatWithComma());
//            }
//            else
//            {
//                if (toNum > 0)
//                    returnStr = string.Format("{0}", toNum.FormatWithComma());
//            }

//            return returnStr;
//        }

//        public static string RenderRangeYear(DateTime? fromDate, DateTime? toDate, string seperator = " - ")
//        {
//            var returnStr = string.Empty;
//            if (fromDate != null)
//            {
//                if (toDate != null)
//                    returnStr = string.Format("{0}{1}{2}", fromDate.DateTimeQuestToString("yyyy"), seperator, toDate.DateTimeQuestToString("yyyy"));
//                else
//                    returnStr = string.Format("{0}", fromDate.DateTimeQuestToString("yyyy"));
//            }
//            else
//            {
//                if (toDate != null)
//                    returnStr = string.Format("{0}", toDate.DateTimeQuestToString("yyyy"));
//            }

//            return returnStr;
//        }

//        public static string RenderJobWorkingAddress(IdentityJobAddress workAddress, bool canShowDetail = true)
//        {
//            var addressDetail = string.Empty;
//            if (workAddress != null)
//            {
//                //if (workAddress.region_info != null)
//                //{
//                //    if (string.IsNullOrEmpty(addressDetail))
//                //    {
//                //        addressDetail = string.Format("{0} ({1})", workAddress.region_info.region, workAddress.region_info.furigana);
//                //    }
//                //    else
//                //    {
//                //        addressDetail += ", " + string.Format("{0} ({1})", workAddress.region_info.region, workAddress.region_info.furigana);
//                //    }
//                //}

//                if (workAddress.prefecture_info != null)
//                {
//                    addressDetail = workAddress.prefecture_info.furigana;
//                }

//                if (workAddress.city_info != null)
//                {
//                    if (string.IsNullOrEmpty(addressDetail))
//                    {
//                        addressDetail = string.Format("{0}", workAddress.city_info.furigana);
//                    }
//                    else
//                    {
//                        addressDetail += " - " + string.Format("{0}", workAddress.city_info.furigana);
//                    }
//                }

//                if (canShowDetail)
//                {
//                    if (string.IsNullOrEmpty(addressDetail))
//                    {
//                        if (!string.IsNullOrEmpty(workAddress.detail))
//                        {
//                            if (!string.IsNullOrEmpty(workAddress.furigana))
//                            {
//                                addressDetail = string.Format("{0} ({1})", workAddress.detail, workAddress.furigana);
//                            }
//                            else
//                            {
//                                addressDetail = string.Format("{0}", workAddress.detail);
//                            }
//                        }
//                        else
//                        {
//                            if (!string.IsNullOrEmpty(workAddress.furigana))
//                            {
//                                addressDetail = string.Format("{0}", workAddress.furigana);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        if (!string.IsNullOrEmpty(workAddress.detail))
//                        {
//                            if (!string.IsNullOrEmpty(workAddress.furigana))
//                            {
//                                addressDetail += " - " + string.Format("{0} ({1})", workAddress.detail, workAddress.furigana);
//                            }
//                            else
//                            {
//                                addressDetail += " - " + string.Format("{0}", workAddress.detail);
//                            }
//                        }
//                        else
//                        {
//                            if (!string.IsNullOrEmpty(workAddress.furigana))
//                            {
//                                addressDetail = " - " + string.Format("{0}", workAddress.furigana);
//                            }
//                        }
//                    }
//                }
//            }

//            return addressDetail;
//        }

//        public static string RenderJobCompanyAddress(IdentityJob jobInfo, IdentityCompany companyInfo)
//        {
//            var companyAddress = string.Empty;

//            if (companyInfo.address_info != null)
//            {
//                if (companyInfo.address_info.prefecture_info != null)
//                {
//                    companyAddress = companyInfo.address_info.prefecture_info.furigana;
//                }

//                if (companyInfo.address_info.city_info != null)
//                {
//                    if (string.IsNullOrEmpty(companyAddress))
//                    {
//                        companyAddress = string.Format("{0}", companyInfo.address_info.city_info.furigana);
//                    }
//                    else
//                    {
//                        companyAddress += " - " + string.Format("{0}", companyInfo.address_info.city_info.furigana);
//                    }
//                }
//            }

//            if (jobInfo.view_company && companyInfo.address_info != null)
//            {
//                var companyAddressInfo = companyInfo.address_info;
//                if (string.IsNullOrEmpty(companyAddress))
//                {
//                    if (!string.IsNullOrEmpty(companyInfo.address_detail))
//                    {
//                        if (!string.IsNullOrEmpty(companyInfo.address_furigana))
//                        {
//                            companyAddress = string.Format("{0} ({1})", companyInfo.address_detail, companyInfo.address_furigana);
//                        }
//                        else
//                        {
//                            companyAddress = string.Format("{0}", companyInfo.address_detail);
//                        }
//                    }
//                    else
//                    {
//                        if (!string.IsNullOrEmpty(companyInfo.address_furigana))
//                        {
//                            companyAddress = string.Format("{0}", companyInfo.address_furigana);
//                        }
//                    }
//                }
//                else
//                {
//                    if (!string.IsNullOrEmpty(companyInfo.address_detail))
//                    {
//                        if (!string.IsNullOrEmpty(companyInfo.address_furigana))
//                        {
//                            companyAddress += " - " + string.Format("{0} ({1})", companyInfo.address_detail, companyInfo.address_furigana);
//                        }
//                        else
//                        {
//                            companyAddress += " - " + string.Format("{0}", companyInfo.address_detail);
//                        }
//                    }
//                    else
//                    {
//                        if (!string.IsNullOrEmpty(companyInfo.address_furigana))
//                        {
//                            companyAddress = " - " + string.Format("{0}", companyInfo.address_furigana);
//                        }
//                    }
//                }

//            }

//            return companyAddress;
//        }
//    }
//}