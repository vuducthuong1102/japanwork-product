//using System.Web.Mvc;
//using MySite.Logging;
//using MySite.Models;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using System;
//using MySite.Helpers;
//using System.Threading.Tasks;
//using MySite.Services;
//using SingleSignOn.DB.Sql.Entities;

//namespace MySite.Controllers
//{
//    public class SearchController : BaseController
//    {
//        private readonly ILog logger = LogProvider.For<SearchController>();

//        public ActionResult Top(string keyword,string type)
//        {
//            if (string.IsNullOrEmpty(keyword))
//            {
//                return RedirectToErrorPage();
//            }

//            var model = new BannerSearchModel()
//            {
//                Keyword = keyword,
//                TabActive = type
//            };
//            return View(model);
//        }

//        [HttpGet]
//        public async Task<ActionResult> GetSearchPeople(string keyword, int page, int pagesize)
//        {
//            List<IdentityUser> listItem = new List<IdentityUser>();
//            var apimodel = GetDataSearch(keyword, page, pagesize);
//            try
//            {
//                var postApiReturned = await SearchServices.SearchUserAsycn(apimodel);
//                if (postApiReturned != null)
//                {
//                    if (postApiReturned.Data != null)
//                    {
//                        listItem = JsonConvert.DeserializeObject<List<IdentityUser>>(postApiReturned.Data.ToString());
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying GetSearchPeople because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return PartialView("../Search/Item/_PeopleList", listItem);
//        }

//        //[HttpGet]
//        //public async Task<ActionResult> GetSearchCategory(string keyword, int page, int pagesize)
//        //{
//        //    List<IdentityCategory> listItem = new List<IdentityCategory>();
//        //    var apimodel = GetDataSearch(keyword, page, pagesize);
//        //    try
//        //    {
//        //        var result = await SearchServices.SearchCategoryAsycn(apimodel);
//        //        if (result != null)
//        //        {
//        //            if (result.Data != null)
//        //            {
//        //                listItem = JsonConvert.DeserializeObject<List<IdentityCategory>>(result.Data.ToString());
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        var strError = string.Format("Failed when trying GetSearchCategory because: {0}", ex.ToString());
//        //        logger.Error(strError);
//        //    }

//        //    return PartialView("../Search/Item/_CategoryList", listItem);
//        //}

//        //[HttpGet]
//        //public async Task<ActionResult> GetSearchDestination(string keyword, int page, int pagesize)
//        //{
//        //    List<IdentityPlace> listItem = new List<IdentityPlace>();
//        //    var apimodel = GetDataSearch(keyword, page, pagesize);
//        //    try
//        //    {
//        //        var result = await SearchServices.SearchDestinationAsycn(apimodel);
//        //        if (result != null)
//        //        {
//        //            if (result.Data != null)
//        //            {
//        //                listItem = JsonConvert.DeserializeObject<List<IdentityPlace>>(result.Data.ToString());
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        var strError = string.Format("Failed when trying GetSearchDestination because: {0}", ex.ToString());
//        //        logger.Error(strError);
//        //    }

//        //    return PartialView("../Search/Item/_DestinationList", listItem);
//        //}

//        //[HttpGet]
//        //public async Task<ActionResult> GetSearchPlace(string keyword, int page, int pagesize)
//        //{
//        //    List<IdentityPlace> listItem = new List<IdentityPlace>();
//        //    var apimodel = GetDataSearch(keyword, page, pagesize);
//        //    try
//        //    {
//        //        var result = await SearchServices.SearchPlaceAsycn(apimodel);
//        //        if (result != null)
//        //        {
//        //            if (result.Data != null)
//        //            {
//        //                listItem = JsonConvert.DeserializeObject<List<IdentityPlace>>(result.Data.ToString());
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        var strError = string.Format("Failed when trying GetSearchPlace because: {0}", ex.ToString());
//        //        logger.Error(strError);
//        //    }

//        //    return PartialView("../Search/Item/_PlaceList", listItem);
//        //}

//        //[HttpGet]
//        //public async Task<ActionResult> GetSearchPost(string keyword, int page, int pagesize)
//        //{
//        //    if (Request["keyword"] != null)
//        //    {
//        //        ViewBag.Keyword = Request["keyword"].ToString();
//        //    }

//        //    List<IdentityPost> listItem = new List<IdentityPost>();
//        //    var apimodel = GetDataSearch(keyword, page, pagesize);
//        //    try
//        //    {
//        //        var result = await SearchServices.SearchPostAsycn(apimodel);
//        //        if (result != null)
//        //        {
//        //            if (result.Data != null)
//        //            {
//        //                listItem = JsonConvert.DeserializeObject<List<IdentityPost>>(result.Data.ToString());
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        var strError = string.Format("Failed when trying GetSearchPost because: {0}", ex.ToString());
//        //        logger.Error(strError);
//        //    }

//        //    return PartialView("../Search/Item/_PostList", listItem);
//        //}

//        private ApiFilterModel GetDataSearch(string keyword, int index, int pagesize)
//        {
//            var apimodel = new ApiFilterModel()
//            {
//                Keyword = keyword,
//                PageIndex = index,
//                PageSize = pagesize
//            };
//            var currentUser = AccountHelper.GetCurrentUser();
//            if (currentUser != null)
//            {
//                apimodel.UserId = currentUser.Id;
//                apimodel.TokenKey = currentUser.TokenKey;
//            }

//            return apimodel;
//        }
//    }
//}
