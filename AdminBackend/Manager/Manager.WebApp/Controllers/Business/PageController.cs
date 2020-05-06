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
using Newtonsoft.Json;
using Manager.WebApp.Caching;
using MsSql.AspNet.Identity.MsSqlStores;
using MsSql.AspNet.Identity.Entities;
using Manager.SharedLibs;
using Manager.SharedLibs;

namespace Manager.WebApp.Controllers
{
    public class PageController : BaseAuthedController
    {
        private readonly IStorePage _mainStore;
        private readonly ILog logger = LogProvider.For<PageController>();

        public PageController(IStorePage mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManagePageModel model)
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

            var filter = new IdentityPage
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                Status = model.Status == null ? -1 : (int)model.Status,
                LangCode = UserCookieManager.GetCurrentLanguageOrDefault()
            };

            try
            {
                model.SearchResults = _mainStore.GetByPage(filter, currentPage, SystemSettings.DefaultPageSize);
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

        public ActionResult Create()
        {
            ModelState.Clear();
            var model = new PageEditModel();
            model.BodyContent = string.Empty;

            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Page(PageEditModel model)
        {
            try
            {               
                var info = new IdentityPage();
                info.Title = model.Title;
                info.BodyContent = model.BodyContent;
                info.Description = model.Description;
                info.CreatedBy = User.Identity.GetUserId();
                info.Status = model.Status;
                info.LangCode = UserCookieManager.GetCurrentLanguageOrDefault();

                if (string.IsNullOrEmpty(model.UrlFriendly))
                    info.UrlFriendly = string.Format("{0}", UrlFriendly.ConvertToUrlFriendly(model.Title));
                else
                    info.UrlFriendly = model.UrlFriendly;

                var newId = _mainStore.Insert(info);
                if (newId > 0)
                {
                    this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                    return RedirectToAction("Edit/" + newId);
                }                
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to CreatePage because: " + ex.ToString());
            }
            return View(model);

        }

        public ActionResult Edit(int id)
        {
            ModelState.Clear();

            var model = new PageEditModel();
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

                    model.UrlFriendly = info.UrlFriendly;
                    if (info.MyLanguages.HasData())
                    {
                        model.Title = info.MyLanguages[0].Title;
                        model.BodyContent = info.MyLanguages[0].BodyContent;
                        model.Description = info.MyLanguages[0].Description;
                        model.UrlFriendly = info.MyLanguages[0].UrlFriendly;
                    }

                    model.Status = info.Status;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditPage [{0}] because: {1}", id, ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Page(PageEditModel model)
        {
            try
            {                
                var info = new IdentityPage();
                info.Id = model.Id;
                info.Title = model.Title;
                info.BodyContent = model.BodyContent;
                info.Description = model.Description;
                info.CreatedBy = User.Identity.GetUserId();
                info.Status = model.Status;
                info.LangCode = UserCookieManager.GetCurrentLanguageOrDefault();

                if (string.IsNullOrEmpty(model.UrlFriendly))
                    info.UrlFriendly = string.Format("{0}", UrlFriendly.ConvertToUrlFriendly(model.Title));
                else
                    info.UrlFriendly = model.UrlFriendly;

                _mainStore.Update(info);

                this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to EditPage because: " + ex.ToString());
            }

            return View(model);
        }

        public ActionResult UploadPageImage(List<HttpPostedFileBase> image)
        {
            var apiReturn = CdnServices.UploadImagesAsync(image, "1", "Page/Stories").Result;
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
                _mainStore.Delete(id);
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Page because: " + ex.ToString());

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

        //    return PartialView("../Page/_AssignCategory", model);
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
                    //isSuccess = _mainStore.AssignCategory(id, catId);
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