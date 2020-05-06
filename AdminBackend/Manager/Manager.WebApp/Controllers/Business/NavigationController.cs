using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity;
using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using System.Net;
using Manager.SharedLibs.Logging;
using System.Collections.Generic;
using Newtonsoft.Json;
using Manager.SharedLibs;
using MsSql.AspNet.Identity.MsSqlStores;
using Autofac;
using Manager.WebApp.Resources;
using System.Linq;
using Manager.SharedLibs;
using Manager.WebApp.Services;
using ApiJobMarket.DB.Sql.Entities;

namespace Manager.WebApp.Controllers
{
    public class NavigationController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<NavigationController>();
        private readonly IStoreNavigation _mainStore;

        public NavigationController()
        {
            _mainStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();
        }

        [AccessRoleChecker]
        public ActionResult Index(NavigationViewModels model)
        {
            try
            {
                var listResult = NavigationServices.GetListAsync().Result;
                model.AllNavigations = JsonConvert.DeserializeObject<List<IdentityNavigation>>(listResult.value.ToString());
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat(string.Format("Could not display Index page because: {0}", ex.ToString()));
            }

            return View(model);
        }

        public ActionResult Create(int parentid)
        {
            NavigationViewModels model = new NavigationViewModels();
            try
            {
                model.ParentId = parentid;
                model.Active = true;
                model.Visible = true;
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat("Failed to display Create page because: {0}", ex.ToString());
                PartialView("_Create", model);
            }
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NavigationViewModels model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (!ModelState.IsValid)
                    {
                        string messages = string.Join("; ", ModelState.Values
                                               .SelectMany(x => x.Errors)
                                               .Select(x => x.ErrorMessage + x.Exception));
                        this.AddNotification(messages, NotificationType.ERROR);
                        return View(model);
                    }
                }

                var identity = new ApiNavigationInsertModel();
                identity.ParentId = model.ParentId;
                identity.Active = (model.Active) ? 1 : 0;
                //identity.Controller = model.Controller;
                //identity.Action = model.Action;
                identity.CssClass = model.CssClass;
                identity.IconCss = model.IconCss;
                identity.SortOrder = model.SortOrder;
                identity.Name = model.Name;
                identity.AbsoluteUri = model.AbsoluteUri;
                identity.Visible = model.Visible;
                identity.Title = model.Title;

                NavigationServices.InsertAsync(identity);
                //_mainStore.Insert(identity);

                this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);

                //Clear Cache
                FrontendCachingHelpers.ClearNavigationCache();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Failed to create new because: {0}", ex.ToString());
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            NavigationViewModels model = new NavigationViewModels();
            try
            {
                var result = NavigationServices.GetDetailAsync(id).Result;
                var navigation = JsonConvert.DeserializeObject<IdentityNavigation>(result.value.ToString());

                if (navigation != null)
                {
                    model.Id = id;
                    model.ParentId = navigation.ParentId;
                    model.Active = (navigation.Active == 1) ? true : false;
                    //model.Controller = navigation.Controller;
                    //model.Action = navigation.Action;
                    model.CssClass = navigation.CssClass;
                    model.IconCss = navigation.IconCss;
                    model.SortOrder = navigation.SortOrder;
                    model.Name = navigation.Name;
                    model.AbsoluteUri = navigation.AbsoluteUri;
                    model.Visible = navigation.Visible;
                    model.Title = navigation.Title;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat("Failed to display Edit page because: {0}", ex.ToString());
                PartialView("_Update", model);
            }
            return PartialView("_Update", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(NavigationViewModels model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (!ModelState.IsValid)
                    {
                        string messages = string.Join("; ", ModelState.Values
                                               .SelectMany(x => x.Errors)
                                               .Select(x => x.ErrorMessage + x.Exception));
                        this.AddNotification(messages, NotificationType.ERROR);
                        return View(model);
                    }
                }

                var identity = new ApiNavigationEditModel();
                identity.Id = model.Id;
                identity.ParentId = model.ParentId;
                identity.Active = (model.Active) ? 1 : 0;
                //identity.Controller = model.Controller;
                //identity.Action = model.Action;
                identity.CssClass = model.CssClass;
                identity.IconCss = model.IconCss;
                identity.SortOrder = model.SortOrder;
                identity.Name = model.Name;
                identity.AbsoluteUri = model.AbsoluteUri;
                identity.Visible = model.Visible;
                identity.Title = model.Title;

                NavigationServices.UpdateAsync(identity);
                //_mainStore.Update(identity);

                this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);

                //Clear Cache
                FrontendCachingHelpers.ClearNavigationCache();
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat("Failed to update because: {0}", ex.ToString());
            }

            return RedirectToAction("Index");
        }

        private void ApplyNewSorting(List<SortingElement> sortList, int parentId = 0)
        {
            if (sortList.HasData())
            {
                var beginOrder = 0;
                foreach (var item in sortList)
                {
                    beginOrder++;
                    item.SortOrder = beginOrder;
                    item.ParentId = parentId;

                    if (item.children.HasData())
                    {
                        ApplyNewSorting(item.children, item.id);
                    }
                }
            }
        }

        public ActionResult UpdateSorting(string data)
        {
            try
            {
                var sortList = new List<SortingElement>();

                if (!string.IsNullOrEmpty(data))
                    sortList = JsonConvert.DeserializeObject<List<SortingElement>>(data);

                if (sortList != null)
                    ApplyNewSorting(sortList);

                //Update in DB
                if (sortList != null)
                    NavigationServices.UpdateSortingAsync(sortList);
                    //_mainStore.UpdateSorting(sortList);

                //Clear Cache
                FrontendCachingHelpers.ClearNavigationCache();

                return Json(new AjaxResponseModel { Success = true, Message = ManagerResource.LB_UPDATE_SUCCESS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat("Failed to UpdateSorting because: {0}" + ex.ToString());
                return Json(new AjaxResponseModel { Success = false, Message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT }, JsonRequestBehavior.AllowGet);
            }
        }

        //Show popup confirm delete        
        public ActionResult PopupDelete(string id)
        {
            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NavigationViewModels record = new NavigationViewModels();
            record.Id = Convert.ToInt32(id);

            return PartialView("_DeleteInfo", record);
        }

        [HttpPost, ActionName("PopupDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int Id = Convert.ToInt32(id);

                NavigationServices.DeleteAsync(Id);
                //_mainStore.Delete(Id);

                //Clear cache
                FrontendCachingHelpers.ClearNavigationCache();

                return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat("Failed to delete because: {0}", ex.ToString());
                return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShowMenuLang(int id)
        {
            ManageNavigationLangModel model = new ManageNavigationLangModel();
            try
            {
                model.NavigationId = id;
                model.Languages = LanguagesProvider.GetListLanguages();
                var result = NavigationServices.GetDetailAsync(id).Result;
                model.NavigationInfo = JsonConvert.DeserializeObject<IdentityNavigation>(result.value.ToString());
            }
            catch (Exception ex)
            {
                var strError = "Failed to get data because: " + ex.ToString();
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.Error(strError);
            }

            return PartialView("_Detail", model);
        }

        public ActionResult UpdateLang()
        {
            ManageNavigationLangModel model = new ManageNavigationLangModel();
            var id = Utils.ConvertToInt32(Request["Id"]);
            var navId = Utils.ConvertToInt32(Request["NavigationId"]);

            if (navId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id > 0)
            {
                model.IsUpdate = true;
            }

            try
            {
                model.Languages = LanguagesProvider.GetListLanguages();
                model.NavigationId = navId;

                //Begin db transaction
                var result = NavigationServices.GetDetailLangAsync(id).Result;

                var info = JsonConvert.DeserializeObject<IdentityNavigationLang>(result.value.ToString());

                if (info != null)
                {
                    model.NavigationId = navId;
                    model.Id = info.Id;
                    model.LangCode = info.LangCode;
                    model.Title = info.Title;
                    model.AbsoluteUri = info.AbsoluteUri;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed for Show UpdateLang form request: " + ex.ToString());
            }

            return PartialView("~/Views/Navigation/_UpdateLang.cshtml", model);
        }

        [HttpPost]
        public ActionResult UpdateLang(ManageNavigationLangModel model)
        {
            var msg = ManagerResource.LB_OPERATION_SUCCESS;
            var isSuccess = false;

            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);

                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = messages });
            }

            try
            {
                var code = 0;

                //Begin db transaction
              

                if (model.Id > 0)
                {
                    var data = new ApiNavigationLangEditModel();
                    data.Id = model.Id;
                    data.NavigationId = model.NavigationId;
                    data.Title = model.Title;
                    data.LangCode = model.LangCode;
                    data.AbsoluteUri = model.AbsoluteUri;

                    //Update
                    var result= NavigationServices.UpdateLangAsync(data).Result;
                    code = Utils.ConvertToInt32(result.status);
                    
                    if (code == EnumCommonCode.Error)
                    {
                        return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = ManagerResource.LB_DUPLICATE_DATA, clientcallback = string.Format(" ShowMenuLang({0})", model.NavigationId) });
                    }

                    //Clear cache
                    FrontendCachingHelpers.ClearNavigationCache();
                }
                else
                {
                    var data = new ApiNavigationLangInsertModel();
                    data.NavigationId = model.NavigationId;
                    data.Title = model.Title;
                    data.LangCode = model.LangCode;
                    data.AbsoluteUri = model.AbsoluteUri;
                    //Add new
                    var result = NavigationServices.InsertLangAsync(data).Result;
                    code = Utils.ConvertToInt32(result.status);

                    if (code == EnumCommonCode.Error)
                    {
                        return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = ManagerResource.LB_DUPLICATE_DATA, clientcallback = string.Format(" ShowMenuLang({0})", model.NavigationId) });
                    }

                    //Clear cache
                    FrontendCachingHelpers.ClearNavigationCache();
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                logger.Error("Failed for UpdateLang request: " + ex.ToString());

                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT });
            }

            return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = string.Format(" ShowMenuLang({0})", model.NavigationId) });
        }

        public ActionResult DeleteLang()
        {
            ManageNavigationLangModel model = new ManageNavigationLangModel();
            var id = Utils.ConvertToInt32(Request["Id"]);
            var groupId = Utils.ConvertToInt32(Request["NavigationId"]);
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            model.NavigationId = groupId;
            model.Id = id;

            return PartialView("~/Views/Navigation/_DeleteLangInfo.cshtml", model);
        }

        [HttpPost, ActionName("DeleteLang")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLang_Confirm(ManageNavigationLangModel model)
        {
            var strError = string.Empty;
            if (model.Id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                NavigationServices.DeleteLangAsync(model.Id);

                //Clear cache
                FrontendCachingHelpers.ClearNavigationCache();
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Navigation Lang because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format(" ShowMenuLang({0})", model.NavigationId) });
        }

        #region Helpers



        #endregion
    }
}