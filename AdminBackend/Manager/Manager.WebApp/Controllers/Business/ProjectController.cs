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
using System.IO;
using System.Linq;
using Manager.SharedLibs;
using Manager.SharedLibs;

namespace Manager.WebApp.Controllers
{
    public class ProjectController : BaseAuthedController
    {
        private readonly IStoreProject _mainStore;
        private readonly ILog logger = LogProvider.For<ProjectController>();

        public ProjectController(IStoreProject mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManageProjectModel model)
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

            var filter = new IdentityProject
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

        public ActionResult Create()
        {
            ModelState.Clear();
            var model = new ProjectEditModel();
            model.BodyContent = string.Empty;

            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Project(ProjectEditModel model)
        {
            try
            {
                var coverImg = string.Empty;
                if(model.Cover != null)
                {                  
                    if(model.Cover[0] != null)
                    {
                        var uploadedUrls = UploadImages("Projects", model.CategoryId.ToString());
                        if (uploadedUrls.HasData())
                        {
                            coverImg = uploadedUrls[0];
                        }
                    }
                }
                
                if(string.IsNullOrEmpty(coverImg))
                    coverImg = model.CurrentCover;

                coverImg = RemoveServerUrl(coverImg);

                var info = new IdentityProject();
                info.Title = model.Title;
                info.BodyContent = model.BodyContent;
                info.Description = model.Description;
                info.CategoryId = model.CategoryId;
                info.CreatedBy = User.Identity.GetUserId();
                info.Cover = coverImg;
                info.Status = model.Status;
                info.LangCode = UserCookieManager.GetCurrentLanguageOrDefault();

                info.MetaData = ExtractMetaDataInForm(model);

                if (string.IsNullOrEmpty(model.UrlFriendly))
                    info.UrlFriendly = string.Format("{0}", UrlFriendly.ConvertToUrlFriendly(model.Title));
                else
                    info.UrlFriendly = model.UrlFriendly;

                var newId = _mainStore.Insert(info);
                if (newId > 0)
                {
                    //Clear Cache
                    //FrontendCachingHelpers.ClearProjectCacheByCategoryId(info.CategoryId);

                    this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                    return RedirectToAction("Edit/" + newId);
                }                
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to CreateProject because: " + ex.ToString());
            }
            return View(model);

        }

        public ActionResult Edit(int id)
        {
            ModelState.Clear();

            var model = new ProjectEditModel();
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
                    model.CurrentCover = info.Cover;
                    model.Status = info.Status;

                    //Images
                    model.Images = info.Images;

                    RenderMetaData(info, model);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditProject [{0}] because: {1}", id, ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Project(ProjectEditModel model)
        {
            try
            {
                var coverImg = string.Empty;
                if (model.Cover != null)
                {
                    if (model.Cover[0] != null)
                    {
                        var uploadedUrls = UploadImages("Project", model.CategoryId.ToString());
                        if (uploadedUrls.HasData())
                        {
                            coverImg = uploadedUrls[0];
                        }
                    }
                }

                var info = new IdentityProject();
                info.Id = model.Id;
                info.Title = model.Title;
                info.BodyContent = model.BodyContent;
                info.Description = model.Description;
                info.CategoryId = model.CategoryId;
                info.CreatedBy = User.Identity.GetUserId();
                info.Status = model.Status;
                info.LangCode = UserCookieManager.GetCurrentLanguageOrDefault();

                info.MetaData = ExtractMetaDataInForm(model);

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
                FrontendCachingHelpers.ClearProjectCateById(model.Id);

                this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);

                return RedirectToAction("Edit/" + model.Id);
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to EditProject because: " + ex.ToString());
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
                    FrontendCachingHelpers.ClearProjectCateById(id);
                }                                
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Project because: " + ex.ToString());

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

        //    return PartialView("../Project/_AssignCategory", model);
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
                    //FrontendCachingHelpers.ClearProjectCacheById(id);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying AssignCategory: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = "AfterAssign()" });
        }

        [HttpPost]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> file, int Id)
        {
            var fileName = string.Empty;
            var filePath = string.Empty;
            try
            {
                var imgUrl = string.Empty;
                if(Request.Files != null && Request.Files.Count > 0)
                    fileName = Request.Files[0].FileName;

                if (file != null)
                {
                    var uploadedUrls = UploadImages("Project", "0");
                    if (uploadedUrls.HasData())
                    {
                        imgUrl = uploadedUrls[0];
                    }
                }

                if (string.IsNullOrEmpty(imgUrl))
                {
                    if (Request["Url"] != null)
                        imgUrl = Request["Url"].ToString();

                    imgUrl = RemoveServerUrl(imgUrl);                                      
                }

                var imageIdentity = new IdentityProjectImage();
                var hashFileName = Utility.Md5HashingData(DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetFileNameWithoutExtension(fileName));
                imageIdentity.Id = string.Format("{0}_{1}", Id, hashFileName);
                imageIdentity.ProjectId = Id;
                imageIdentity.Name = System.IO.Path.GetFileName(fileName);
                imageIdentity.Url = imgUrl;

                //Storage to db
                _mainStore.AddNewImage(imageIdentity);

                //Clear cache
                FrontendCachingHelpers.ClearProjectCateById(Id);

                return Json(new { success = true, fileId = imageIdentity.Id }); // success  

            }
            catch (Exception ex)
            {
                logger.Error("Failed to Upload because: " + ex.ToString());

                return Json(new { success = false, errorMessage = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT });
            }
        }

        [HttpPost]
        public ActionResult RemoveImage(string Id, string Url, int ProjectId)
        {
            try
            {
                var hasRemoved = _mainStore.RemoveImage(Id);

                if (hasRemoved)
                {
                    //Remove physical image
                    //DeleteImageByUrl(Url);

                    //Clear cache
                    FrontendCachingHelpers.ClearProjectCateById(ProjectId);


                    return Json(new { success = true }); // success
                }
                else
                {
                    logger.Error("Failed to RemoveImage because the SQL Execution has an error");
                    return Json(new { success = false, errorMessage = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT });
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to RemoveImage because: " + ex.ToString());

                return Json(new { success = false, errorMessage = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT });
            }
        }

        [HttpPost]
        public ActionResult RefreshImageList(int Id)
        {
            List<IdentityProjectImage> myImages = null;
            try
            {
                myImages = _mainStore.GetListImage(Id);
            }
            catch (Exception ex)
            {
                logger.Error("Failed to RefreshImageList because: " + ex.ToString());
            }

            return PartialView("~/Views/Project/Partials/_Images.cshtml", myImages);
        }

        #region Helpers

        private MetaDataProject ExtractMetaDataInForm(ProjectEditModel model)
        {
            var metaData = new MetaDataProject();
            metaData.BeginDate = Utils.ConvertStringToDateTimeByFormat(model.BeginDate);
            metaData.FinishDate = Utils.ConvertStringToDateTimeByFormat(model.FinishDate);
            metaData.PersonInCharge = model.PersonInCharge;
            metaData.FrameWork = model.FrameWork;
            metaData.Customer = model.Customer;

            return metaData;
        }

        private void RenderMetaData(IdentityProject info, ProjectEditModel model)
        {
            if(info != null && info.MetaData != null)
            {
                model.BeginDate = info.MetaData.BeginDate.DateTimeQuestToString("dd/MM/yyyy");
                model.FinishDate = info.MetaData.FinishDate.DateTimeQuestToString("dd/MM/yyyy");
                model.PersonInCharge = info.MetaData.PersonInCharge;
                model.FrameWork = info.MetaData.FrameWork;
                model.Customer = info.MetaData.Customer;
            }
        }

        private void DeleteImageByUrl(string url)
        {
            try
            {
                string fullPath = Request.MapPath(url);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch
            {
                // Deliberately empty.
            }
        }

        #endregion

    }
}