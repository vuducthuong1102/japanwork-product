//using System;
//using System.Net;
//using System.Web.Mvc;
//using Manager.SharedLibs.Logging;
//using Manager.WebApp.Helpers;
//using Manager.WebApp.Models;
//using Manager.WebApp.Resources;
//using Manager.WebApp.Settings;
//using MsSql.AspNet.Identity.Stores;
//using Manager.WebApp.Caching;
//using ApiJobMarket.DB.Sql.Entities;
//using Manager.SharedLibs;
//using System.Collections.Generic;
//using System.Linq;
//using Manager.WebApp.Services;
//using Newtonsoft.Json;
//using Manager.SharedLibs;

//namespace Manager.WebApp.Controllers
//{
//    public class AgencyController : BaseAuthedController
//    {
//        private readonly ILog logger = LogProvider.For<AgencyController>();

//        public AgencyController()
//        {
//            _mainStore = mainStore;
//        }

//        [AccessRoleChecker]
//        public ActionResult Index(ManageAgencyModel model)
//        {
//            int currentPage = 1;
//            int pageSize = SystemSettings.DefaultPageSize;

//            if (string.IsNullOrEmpty(model.SearchExec))
//            {
//                model.SearchExec = "Y";
//                if (!ModelState.IsValid)
//                {
//                    ModelState.Clear();
//                }
//            }

//            if (Request["Page"] != null)
//            {
//                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
//            }

//            var filter = GetFilterConfig();

//            try
//            {
//                model.SearchResults = _mainStore.GetByPage(filter);
//                if (model.SearchResults.HasData())
//                {
//                    model.TotalCount = model.SearchResults[0].total_count;
//                    model.CurrentPage = currentPage;
//                    model.PageSize = pageSize;

//                    List<int> listIds = new List<int>();
//                    listIds = model.SearchResults.Select(x => x.agency_id).ToList();
//                    var apiUserInfoModel = new ApiListUserInfoModel();
//                    apiUserInfoModel.ListUserId = listIds;

//                    var listUserReturned = CustomerAccountServices.GetListUserProfileAsync(apiUserInfoModel).Result;
//                    if (listUserReturned.Data != null)
//                    {
//                        model.ListUsers = JsonConvert.DeserializeObject<List<dynamic>>(listUserReturned.Data.ToString());
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed to get data because: " + ex.ToString());

//                return View(model);
//            }

//            return View(model);
//        }

//        public ActionResult Create()
//        {
//            ModelState.Clear();
//            var model = new AgencyEditModel();

//            return View(model);
//        }

//        //[HttpPost, ActionName("Create")]
//        //[ValidateAntiForgeryToken]
//        //public ActionResult Create_Agency(AgencyEditModel model)
//        //{
//        //    try
//        //    {
//        //        var coverImg = string.Empty;
//        //        if (model.Cover != null)
//        //        {
//        //            if (model.Cover[0] != null)
//        //            {
//        //                var uploadedUrls = UploadImages("ProjectCategories");
//        //                if (uploadedUrls.HasData())
//        //                {
//        //                    coverImg = uploadedUrls[0];
//        //                }
//        //            }
//        //        }

//        //        if (string.IsNullOrEmpty(coverImg))
//        //            coverImg = model.CurrentCover;

//        //        coverImg = RemoveServerUrl(coverImg);

//        //        var info = new IdentityAgency();
//        //        info.Name = model.Name;
//        //        info.Description = model.Description;
//        //        info.ParentId = model.ParentId;
//        //        info.CreatedBy = User.Identity.GetUserId();
//        //        info.Cover = coverImg;

//        //        info.Status = model.Status;
//        //        info.LangCode = UserCookieManager.GetCurrentLanguageOrDefault();

//        //        if (string.IsNullOrEmpty(model.UrlFriendly))
//        //            info.UrlFriendly = string.Format("{0}", UrlFriendly.ConvertToUrlFriendly(model.Name));
//        //        else
//        //            info.UrlFriendly = model.UrlFriendly;

//        //        var newId = _mainStore.Insert(info);
//        //        if (newId > 0)
//        //        {
//        //            //Clear Cache
//        //            FrontendCachingHelpers.ClearAgencyCache();

//        //            this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
//        //            return RedirectToAction("Edit/" + newId);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

//        //        logger.Error("Failed to CreateAgency because: " + ex.ToString());
//        //    }
//        //    return View(model);

//        //}

//        public ActionResult Edit(int id)
//        {
//            ModelState.Clear();
//            var model = new AgencyEditModel();
//            try
//            {
//                if (id <= 0)
//                {
//                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//                }

//                var langCode = UserCookieManager.GetCurrentLanguageOrDefault();
//                var info = _mainStore.GetById(id, langCode);

//                if (info != null)
//                {
//                    model.Id = info.id;
//                    model.Name = info.agency;
//                    model.Status = info.status;

//                    var apiUserInfoModel = new ApiUserModel();
//                    apiUserInfoModel.UserId = info.agency_id;

//                    var listUserReturned = CustomerAccountServices.GetUserProfileAsync(apiUserInfoModel).Result;
//                    if (listUserReturned.Data != null)
//                    {
//                        var userInfo = JsonConvert.DeserializeObject<IdentityUser>(listUserReturned.Data.ToString());

//                        if(userInfo != null)
//                        {
//                            model.Avatar = userInfo.Avatar;
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

//                logger.Error(string.Format("Failed to display EditAgency [{0}] because: {1}", id, ex.ToString()));
//            }

//            return View(model);
//        }

//        [HttpPost, ActionName("Edit")]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit_Agency(AgencyEditModel model)
//        {
//            try
//            {                
//                var info = new IdentityAgency();
//                info.id = model.Id;
//                info.agency = model.Name;                
//                info.status = model.Status;
               
//                _mainStore.Update(info);

//                //Clear cache
//                //FrontendCachingHelpers.ClearAgencyCache();

//                this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
//                return RedirectToAction("Index");
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

//                logger.Error("Failed to EditAgency because: " + ex.ToString());
//            }

//            return View(model);
//        }

//        //Show popup confirm delete        
//        //[AccessRoleChecker]
//        public ActionResult Delete(int id)
//        {
//            if (id <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            return PartialView("_PopupDelete", id);
//        }

//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete_Confirm(int id)
//        {
//            var strError = string.Empty;
//            if (id <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            try
//            {
//                var info = _mainStore.GetById(id, UserCookieManager.GetCurrentLanguageOrDefault());

//                if (info != null)
//                {
//                    _mainStore.Delete(id);
//                }
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to get Delete Agency because: " + ex.ToString());

//                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload()" });
//        }

//        #region Helpers


//        #endregion

//    }
//}