//using System;
//using Autofac;
//using ApiJobMarket;
//using ApiJobMarket.Caching.Providers;
//using ApiJobMarket.DB.Sql.Entities;
//using ApiJobMarket.DB.Sql.Stores;
//using ApiJobMarket.Logging;
//using System.Collections.Generic;
//using ApiJobMarket.Settings;

//namespace ApiJobMarket.WebApp.Helpers
//{
//    public class PlaceHelper
//    {
//        private static readonly ILog logger = LogProvider.For<PlaceHelper>();
//        private static readonly string _placeCacheKey = "PLACE_";
        
//        public static IdentityPlace GetPlaceData(int id)
//        {
//            var myKey = string.Format("{0}{1}", _placeCacheKey, id);
//            IdentityPlace baseInfo = null;
//            try
//            {
//                //Check from cache first
//                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
//                cacheProvider.Get<IdentityPlace>(myKey, out baseInfo);

//                if(baseInfo == null)
//                {
//                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePlace>();
//                    baseInfo = myStore.GetPlaceById(id, true);

//                    if(baseInfo != null)
//                    {
//                        //Storage to cache
//                        cacheProvider.Set(myKey, baseInfo, (int)ApiJobMarketSettings.DefaultCachedTimeout);
//                    }                   
//                }         
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Could not GetPlaceData: " + ex.ToString());
//            }

//            return baseInfo;
//        }        

//        public static List<IdentityPlace> GetPlacesFromList(List<int> arrPlaceIds)
//        {
//            var returnList = new List<IdentityPlace>();
//            var nonExistedIds = new List<int>();
//            try
//            {              
//                if (arrPlaceIds != null && arrPlaceIds.Count > 0)
//                {
//                    foreach (var placeId in arrPlaceIds)
//                    {
//                        //Get from cache first
//                        var placeData = GetPlaceData(placeId);
//                        if (placeData != null)
//                        {
//                            returnList.Add(placeData);
//                        }
//                        else
//                        {
//                            nonExistedIds.Add(placeId);
//                        }                    
//                    }
//                }

//                if(nonExistedIds.Count > 0)
//                {
//                    var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

//                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePlace>();
//                    var placeIds = string.Join(",", nonExistedIds);
//                    if (string.IsNullOrEmpty(placeIds))
//                    {
//                        //Get from database
//                        var places = myStore.GetFromList(placeIds);
//                        if(places != null && places.Count > 0)
//                        {
//                            returnList.AddRange(places);

//                            foreach (var item in places)
//                            {
//                                var myKey = string.Format("{0}{1}", _placeCacheKey, item.Id);

//                                //Storage to cache
//                                cacheProvider.Set(myKey, item, (int)ApiJobMarketSettings.DefaultCachedTimeout);
//                            }
//                        }
//                    }
//                }                
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Could not GetPlacesFromList: " + ex.ToString());
//            }

//            return returnList;
//        }
//    }
//}