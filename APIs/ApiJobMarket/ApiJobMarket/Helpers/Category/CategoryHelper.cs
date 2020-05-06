//using System;
//using Autofac;
//using ApiJobMarket;
//using ApiJobMarket.Caching.Providers;
//using ApiJobMarket.DB.Sql.Entities;
//using ApiJobMarket.DB.Sql.Stores;
//using ApiJobMarket.Logging;
//using ApiJobMarket.Settings;
//using System.Collections.Generic;
//using System.Linq;

//namespace ApiJobMarket.WebApp.Helpers
//{
//    public class CategoryHelper
//    {
//        private static readonly ILog logger = LogProvider.For<CategoryHelper>();
//        private static readonly string _lstCategoryCacheKey = "LIST_CATEGORIES";
        
//        public static List<IdentityCategory> GetListCategory()
//        {
//            var myKey = _lstCategoryCacheKey;
//            List<IdentityCategory> lstInfo = null;
//            try
//            {
//                //Check from cache first
//                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
//                cacheProvider.Get<List<IdentityCategory>>(myKey, out lstInfo);

//                if(lstInfo == null)
//                {
//                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCategory>();
//                    lstInfo = myStore.GetList(string.Empty);

//                    if(lstInfo != null)
//                    {
//                        //Storage to cache
//                        cacheProvider.Set(myKey, lstInfo, SystemSettings.UserCachingTime);
//                    }                   
//                }         
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Could not GetListCategory: " + ex.ToString());
//            }

//            return lstInfo;
//        }

//        public static IdentityCategory GetByUrlFriendly(string urlFriendLy, string langCode)
//        {
//            IdentityCategory returnItem = null;
//            try
//            {
//                IdentityCategoryLang langReturn = null;
//                var categories = GetListCategory();
//                if(categories != null && categories.Count > 0)
//                {
//                    foreach (var item in categories)
//                    {
//                        if(item.UrlFriendly == urlFriendLy)
//                        {
//                            returnItem = item;

//                            return returnItem;
//                        }

//                        if (item.LangList != null && item.LangList.Count > 0)
//                        {
//                            langReturn = item.LangList.Where(x => x.LangCode == langCode && x.UrlFriendly == urlFriendLy).FirstOrDefault();
//                            if (langReturn != null)
//                            {
//                                returnItem = item;
//                                returnItem.Name = langReturn.Name;
//                                returnItem.UrlFriendly = langReturn.UrlFriendly;

//                                break;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Failed when GetByUrlFriendly [{0}] due to {1}", urlFriendLy, ex.ToString()));
//            }

//            return returnItem;
//        }

//        public static IdentityCategory GetById(int CategoryId)
//        {
//            var result = new IdentityCategory();
//            var listCategory = GetListCategory();

//            if (listCategory != null && listCategory.Count > 0)
//            {
//                result = listCategory.FirstOrDefault(s => s.Id == CategoryId);
//            }
//            return result;
//        }

//        public static void ClearCache()
//        {
//            try
//            {
//                var myKey = _lstCategoryCacheKey;
//                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
//                cacheProvider.Clear(myKey);                
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Could not ClearCache: " + ex.ToString());
//            }
//        }

//        public static IdentityCategory GetCategoryByLang(IdentityCategory identity, string langCode)
//        {
//            IdentityCategory returnItem = identity;
//            try
//            {
//                IdentityCategoryLang langReturn = null;
//                if (identity.LangList != null && identity.LangList.Count > 0)
//                {
//                    langReturn = identity.LangList.Where(x => x.LangCode == langCode).FirstOrDefault();
//                }

//                if (langReturn != null)
//                {
//                    returnItem.Name = langReturn.Name;
//                    returnItem.UrlFriendly = langReturn.UrlFriendly;
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Failed when GetCategoryByLang [{0}] due to {1}", identity.Id, ex.ToString()));
//            }

//            return identity;
//        }

//        public static IdentityCategory GetCategoryById(int categoryId, string langCode)
//        {
//            var catName = string.Empty;
//            try
//            {
//                var listCats = GetListCategory();
//                if(listCats != null && listCats.Count > 0)
//                {
//                    IdentityCategory catItem = null;
//                    IdentityCategoryLang langReturn = null;
//                    catItem = listCats.Where(x => x.Id == categoryId).FirstOrDefault();

//                    if(catItem != null)
//                    {
//                        if(catItem.LangList != null && catItem.LangList.Count > 0)
//                        {
//                            langReturn = catItem.LangList.Where(x => x.LangCode == langCode).FirstOrDefault();
//                        }

//                        if (langReturn != null)
//                        {
//                            catItem.Name = langReturn.Name;
//                            catItem.UrlFriendly = langReturn.UrlFriendly;
//                        }

//                        return catItem;
//                    }                    
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Could not GetCategoryById: " + ex.ToString());
//            }

//            return null;
//        }

//        public static IdentityPost GetPostCategory(IdentityPost identity, string langCode)
//        {
//            if (identity.CategoryId > 0)
//            {
//                var catInfo = CategoryHelper.GetCategoryById(identity.CategoryId.Value, langCode);
//                if (catInfo != null)
//                {
//                    identity.CategoryName = catInfo.Name;
//                    identity.CategoryFriendlyUrl = catInfo.UrlFriendly;
//                }
//            }

//            return identity;
//        }

//        public static List<IdentityPost> GetListPostCategory(List<IdentityPost> list, string langCode)
//        {
//            var returnList = new List<IdentityPost>();
//            if (list != null && list.Count > 0)
//            {
//                foreach (var item in list)
//                {
//                    var newItem = CategoryHelper.GetPostCategory(item, langCode);
//                    returnList.Add(newItem);
//                }
//            }
            
//            return returnList;
//        }
//    }
//}