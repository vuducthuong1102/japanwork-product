using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Services;
using Manager.WebApp.Settings;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Stores;
using Newtonsoft.Json;
using Manager.WebApp.Caching;
using Manager.SharedLibs;
using Manager.SharedLibs;

namespace Manager.WebApp.Controllers
{
    public class PostController : BaseAuthedController
    {
        private readonly IStorePost _mainStore;
        private readonly ILog logger = LogProvider.For<PostController>();

        public PostController(IStorePost mainStore)
        {
            _mainStore = mainStore;
        }

        //[AccessRoleChecker]
        //public ActionResult Index(ManagePostModel model)
        //{
        //    return View(model);
        //}

        [AccessRoleChecker]
        public ActionResult Index(ManagePostModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            var filter = new IdentityPost
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                Status = model.Status == null ? -1 : (int)model.Status,
                PageIndex = currentPage,
                PageSize = pageSize,
                LangCode = UserCookieManager.GetCurrentLanguageOrDefault(),
                CategoryId = model.CategoryId == null ? -1 : (int)model.CategoryId
            };

            //if (!string.IsNullOrEmpty(model.PropertyList))
            //    filter.SelectedProperties = model.PropertyList.Split(',');

            try
            {
                model.SearchResults = _mainStore.GetByPage(filter);
                if (model.SearchResults != null && model.SearchResults.Count > 0)
                {
                    model.TotalCount = model.SearchResults[0].TotalCount;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }

        //[HttpPost]
        //public JsonResult GetListPost()
        //{
        //    var currentPage = 1;
        //    var status = -1;
        //    var keyword = string.Empty;
        //    var pageSize = (Request["pagination[perpage]"] != null) ? Utils.ConvertToInt32(Request["pagination[perpage]"]) : SystemSettings.DefaultPageSize;

        //    if (Request["pagination[page]"] != null)
        //    {
        //        currentPage = Utils.ConvertToInt32(Request["pagination[page]"], 1);
        //    }

        //    if (Request["query[Status]"] != null)
        //    {
        //        status = Utils.ConvertToInt32(Request["query[Status]"], -1);
        //    }

        //    if (Request["query[generalSearch]"] != null)
        //    {
        //        keyword = Request["query[generalSearch]"].ToString();
        //    }

        //    var model = new ManagePostModel();
        //    model.meta = new CommonMetaPagingModel();

        //    model.meta.field = Request["sort[field]"];
        //    model.meta.sort = Request["sort[sort]"];

        //    var filter = new IdentityPost
        //    {
        //        //Name = !string.IsNullOrEmpty(model.Name) ? model.Name.Trim() : null,
        //        //Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone.Trim() : null,
        //        //Code = !string.IsNullOrEmpty(model.Code) ? model.Code.Trim() : null,                
        //        //FromDate = string.IsNullOrEmpty(model.FromDate) ? null : (DateTime?)DateTime.ParseExact(model.FromDate, ManagePostModel.DATETIME_FORMAT, null),
        //        //ToDate = string.IsNullOrEmpty(model.ToDate) ? null : (DateTime?)DateTime.ParseExact(model.ToDate, ManagePostModel.DATETIME_FORMAT, null),
        //        Keyword = keyword,
        //        PageIndex = currentPage,
        //        PageSize = pageSize,
        //        SortField = model.meta.field,
        //        SortType = model.meta.sort,
        //        Status = status,
        //        LangCode = UserCookieManager.GetCurrentLanguageOrDefault()
        //    };

        //    try
        //    {
        //        model.data = _mainStore.GetByPage(filter);

        //        model.meta.perpage = pageSize;
        //        if (model.data != null && model.data.Count > 0)
        //        {
        //            model.meta.total = model.data[0].TotalCount;
        //            model.meta.page = currentPage;
        //            model.meta.pages = (int)Math.Ceiling((double)model.meta.total / (double)model.meta.perpage);
        //        }

        //        if (model.data.HasData())
        //        {
        //            foreach (var item in model.data)
        //            {
        //                item.Cover = SocialCdnHelper.GetFullImgPath(item.Cover);
        //                item.CreatedDateLabel = item.CreatedDate.DateTimeQuestToString();

        //                foreach (var type in Enum.GetValues(typeof(EnumPostType)))
        //                {
        //                    if((int)type == item.CategoryId)
        //                        item.CategoryLabel = EnumExtensions.GetEnumDescription((Enum)type);
        //                }
        //            }
        //        }

        //        return Json(new { data = model.data, meta = model.meta }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

        //        logger.Error("Failed to GetListPost because: " + ex.ToString());
        //    }

        //    return Json(new { model }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Create()
        {
            ModelState.Clear();
            var model = new PostEditModel();
            model.BodyContent = string.Empty;

            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Post(PostEditModel model)
        {
            try
            {
                var coverImg = string.Empty;
                if(model.Cover != null)
                {                  
                    if(model.Cover[0] != null)
                    {
                        var uploadedUrls = UploadImages("Post/Stories", model.CategoryId.ToString());
                        if (uploadedUrls.HasData())
                        {
                            coverImg = uploadedUrls[0];
                        }
                    }
                }
                
                if(string.IsNullOrEmpty(coverImg))
                    coverImg = model.CurrentCover;

                coverImg = RemoveServerUrl(coverImg);

                var info = new IdentityPost();
                info.Title = model.Title;
                info.BodyContent = model.BodyContent;
                info.Description = model.Description;
                info.CategoryId = model.CategoryId;
                info.CreatedBy = User.Identity.GetUserId();
                info.IsHighlights = model.IsHighlights;
                info.Cover = coverImg;
                info.Status = model.Status;
                info.LangCode = UserCookieManager.GetCurrentLanguageOrDefault();

                if (string.IsNullOrEmpty(model.UrlFriendly))
                    info.UrlFriendly = string.Format("{0}", UrlFriendly.ConvertToUrlFriendly(model.Title));
                else
                    info.UrlFriendly = model.UrlFriendly;

                var newId = _mainStore.Insert(info);
                if (newId > 0)
                {
                    //Clear Cache
                    FrontendCachingHelpers.ClearPostCacheByCategoryId(info.CategoryId);

                    this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                    return RedirectToAction("Edit/" + newId);
                }                
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to CreatePost because: " + ex.ToString());
            }
            return View(model);

        }

        public ActionResult Edit(int id)
        {
            ModelState.Clear();

            var model = new PostEditModel();
            try
            {                
                if(id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var langCode = UserCookieManager.GetCurrentLanguageOrDefault();
                var info = _mainStore.GetById(id, langCode);
                if(info != null)
                {
                    model.Id = info.Id;
                    if (info.MyLanguages.HasData())
                    {
                        model.Title = info.MyLanguages[0].Title;
                        model.BodyContent = info.MyLanguages[0].BodyContent;
                        model.Description = info.MyLanguages[0].Description;
                        model.UrlFriendly = info.MyLanguages[0].UrlFriendly;
                    }

                    model.CategoryId = info.CategoryId;
                    model.IsHighlights = info.IsHighlights;
                    model.CurrentCover = info.Cover;
                    model.Status = info.Status;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditPost [{0}] because: {1}", id, ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(PostEditModel model)
        {
            try
            {
                var coverImg = string.Empty;
                if (model.Cover != null)
                {
                    if (model.Cover[0] != null)
                    {
                        var uploadedUrls = UploadImages("Post/Stories", model.CategoryId.ToString());
                        if (uploadedUrls.HasData())
                        {
                            coverImg = uploadedUrls[0];
                        }
                    }
                }

                var info = new IdentityPost();
                info.Id = model.Id;
                info.Title = model.Title;
                info.BodyContent = model.BodyContent;
                info.Description = model.Description;
                info.CategoryId = model.CategoryId;
                info.CreatedBy = User.Identity.GetUserId();
                info.IsHighlights = model.IsHighlights;
                info.Status = model.Status;
                info.LangCode = UserCookieManager.GetCurrentLanguageOrDefault();

                if (string.IsNullOrEmpty(model.UrlFriendly))
                    info.UrlFriendly = string.Format("{0}", UrlFriendly.ConvertToUrlFriendly(model.Title));
                else
                    info.UrlFriendly = model.UrlFriendly;

                if (!string.IsNullOrEmpty(coverImg))
                    info.Cover = coverImg;
                else
                    info.Cover = model.CurrentCover;

                model.CurrentCover = info.Cover;

                info.Cover = RemoveServerUrl(info.Cover);

                _mainStore.Update(info);

                //Clear cache
                FrontendCachingHelpers.ClearPostCacheById(model.Id);

                //Clear Cache By Category
                FrontendCachingHelpers.ClearPostCacheByCategoryId(info.CategoryId);

                this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to EditPost because: " + ex.ToString());
            }

            return View(model);
        }

        public ActionResult UploadPostImage(List<HttpPostedFileBase> image)
        {
            var apiReturn = CdnServices.UploadImagesAsync(image, "1", "Post/Stories").Result;
            if(apiReturn != null)
            {
                if (apiReturn.Data != null)
                {
                    var resultData = JsonConvert.DeserializeObject<List<string>>(apiReturn.Data.ToString());
                    var returnData = new List<string>();
                    if (resultData.HasData())
                    {
                        foreach (var item in resultData)
                        {
                            returnData.Add(SocialCdnHelper.GetFullImgPath(item));
                        }
                    }

                    return Json(new { success = true, data = returnData });
                }
            }

            return Json(new { success = false, data = "" });
        }

        //Show popup confirm delete        
        //[AccessRoleChecker]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return PartialView("_PopupDelete", id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Confirm(int id)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var info = _mainStore.GetById(id, UserCookieManager.GetCurrentLanguageOrDefault());

                if(info != null)
                {
                    _mainStore.Delete(id);

                    //Clear Cache
                    FrontendCachingHelpers.ClearPostCacheByCategoryId(info.CategoryId);
                }                   

                //Clear cache
                FrontendCachingHelpers.ClearPostCacheById(id);                
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Post because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
        }

        //[HttpGet]
        //public ActionResult AssignCategory(int id)
        //{
        //    if (id <= 0)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    AssignCategoryModel model = new AssignCategoryModel();
        //    try
        //    {
        //        model.Id = id;
        //        model.Categories = CommonHelpers.GetListCategory();
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed when trying load AssignCategory: {0}", ex.ToString());
        //        logger.Error(strError);
        //    }

        //    return PartialView("../Post/_AssignCategory", model);
        //}

        [HttpPost, ActionName("AssignCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptAssignCategory(int id)
        {
            var msg = ManagerResource.LB_OPERATION_SUCCESS;
            var isSuccess = false;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var catId = Utils.ConvertToInt32(Request["cat"]);
                if (catId > 0 && id > 0)
                {
                    isSuccess = _mainStore.AssignCategory(id, catId);

                    //Clear cache
                    FrontendCachingHelpers.ClearPostCacheById(id);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying AssignCategory: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = "AfterAssign()" });
        }

        #region Helpers

        #endregion

    }
}