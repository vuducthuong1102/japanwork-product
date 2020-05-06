using System;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Settings;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Stores;
using Manager.WebApp.Caching;
using Manager.SharedLibs;
using Manager.SharedLibs;

namespace Manager.WebApp.Controllers
{
    public class ProjectCategoryController : BaseAuthedController
    {
        private readonly IStoreProjectCategory _mainStore;
        private readonly ILog logger = LogProvider.For<ProjectCategoryController>();

        public ProjectCategoryController(IStoreProjectCategory mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManageProjectCategoryModel model)
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

            var filter = new IdentityProjectCategory
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                Status = model.Status == null ? -1 : (int)model.Status,
                PageIndex = currentPage,
                PageSize = pageSize,
                LangCode = UserCookieManager.GetCurrentLanguageOrDefault()
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

        public ActionResult Create()
        {
            ModelState.Clear();
            var model = new ProjectCategoryEditModel();

            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_ProjectCategory(ProjectCategoryEditModel model)
        {
            try
            {
                var coverImg = string.Empty;
                if(model.Cover != null)
                {                  
                    if(model.Cover[0] != null)
                    {
                        var uploadedUrls = UploadImages("ProjectCategories");
                        if (uploadedUrls.HasData())
                        {
                            coverImg = uploadedUrls[0];
                        }
                    }
                }
                
                if(string.IsNullOrEmpty(coverImg))
                    coverImg = model.CurrentCover;

                coverImg = RemoveServerUrl(coverImg);

                var info = new IdentityProjectCategory();
                info.Name = model.Name;
                info.Description = model.Description;
                info.ParentId = model.ParentId;
                info.CreatedBy = User.Identity.GetUserId();
                info.Cover = coverImg;

                info.Status = model.Status;
                info.LangCode = UserCookieManager.GetCurrentLanguageOrDefault();

                if (string.IsNullOrEmpty(model.UrlFriendly))
                    info.UrlFriendly = string.Format("{0}", UrlFriendly.ConvertToUrlFriendly(model.Name));
                else
                    info.UrlFriendly = model.UrlFriendly;

                var newId = _mainStore.Insert(info);
                if (newId > 0)
                {
                    //Clear Cache
                    FrontendCachingHelpers.ClearProjectCategoryCache();

                    this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                    return RedirectToAction("Edit/" + newId);
                }                
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to CreateProjectCategory because: " + ex.ToString());
            }
            return View(model);

        }

        public ActionResult Edit(int id)
        {
            ModelState.Clear();

            var model = new ProjectCategoryEditModel();
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
                        model.Name = info.MyLanguages[0].Name;
                        model.Description = info.MyLanguages[0].Description;
                        model.UrlFriendly = info.MyLanguages[0].UrlFriendly;
                    }

                    model.ParentId = info.ParentId;
                    model.CurrentCover = info.Cover;
                    model.Status = info.Status;

                    //Images
                    //model.Images = info.Images;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditProjectCategory [{0}] because: {1}", id, ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_ProjectCategory(ProjectCategoryEditModel model)
        {
            try
            {
                var coverImg = string.Empty;
                if (model.Cover != null)
                {
                    if (model.Cover[0] != null)
                    {
                        var uploadedUrls = UploadImages("ProjectCategories");
                        if (uploadedUrls.HasData())
                        {
                            coverImg = uploadedUrls[0];
                        }
                    }
                }

                var info = new IdentityProjectCategory();
                info.Id = model.Id;
                info.Name = model.Name;
                info.Description = model.Description;
                info.ParentId = model.ParentId;
                info.CreatedBy = User.Identity.GetUserId();
                info.Status = model.Status;
                info.LangCode = UserCookieManager.GetCurrentLanguageOrDefault();

                if (string.IsNullOrEmpty(model.UrlFriendly))
                    info.UrlFriendly = string.Format("{0}", UrlFriendly.ConvertToUrlFriendly(model.Name));
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
                FrontendCachingHelpers.ClearProjectCategoryCache();

                this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);

                return RedirectToAction("Edit/" + model.Id);
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to EditProjectCategory because: " + ex.ToString());
            }

            return View(model);
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

                    //Clear cache
                    FrontendCachingHelpers.ClearProjectCategoryCache();
                }                   
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete ProjectCategory because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
        }

        #region Helpers


        #endregion

    }
}