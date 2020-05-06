using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MsSql.AspNet.Identity;
using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using System.Configuration;
using System.Net;
using Manager.WebApp.Resources;
using Manager.SharedLibs.Logging;

using MsSql.AspNet.Identity.Repositories;
using Manager.SharedLibs;
using Manager.WebApp.Models.Api;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Manager.WebApp.Controllers
{
    public class ProcessStatusController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<ProcessStatusController>();

        [AccessRoleChecker]
        public ActionResult Index(ManageProcessStatusModel model)
        {
            try
            {
                model.SearchResults = CommonHelpers.GetListProcessStatus(GetCurrentAgencyId());
            }
            catch (Exception ex)
            {
                this.AddNotification(string.Format("Could not get all index because: {0}", ex.ToString()), NotificationType.ERROR);
            }

            return View(model);
        }
        [AccessRoleChecker]
        public ActionResult Create()
        {
            var model = new ProcessStatusEditModel();
            return PartialView("Create", model);
        }
        public ActionResult SaveCreate(ProcessStatusEditModel model)
        {
            try
            {
                //Begin create
                var apiProcessStatusModel = ExtractProcessStatusInsertFormData(model);

                var apiReturned = ProcessStatusServices.InsertAsync(apiProcessStatusModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            return Json(new AjaxResponseModel { Success = false, Message = apiReturned.error.message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new AjaxResponseModel { Success = true, Message = ManagerResource.LB_INSERT_SUCCESS }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to CreateProcessStatus because: " + ex.ToString());
            }

            return Json(new AjaxResponseModel { Success = true, Message = ManagerResource.LB_INSERT_SUCCESS }, JsonRequestBehavior.AllowGet);
        }
        [AccessRoleChecker]
        public ActionResult Edit(int? id)
        {
            ModelState.Clear();
            var Id = Utils.ConvertToIntFromQuest(id);
            var model = new ProcessStatusEditModel();

            try
            {
                if (Id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var result = ProcessStatusServices.GetDetailAsync(id ?? 0).Result;
                if (result != null && result.value != null)
                {
                    var info = JsonConvert.DeserializeObject<IdentityProcessStatus>(result.value.ToString());
                    model = ParseDataToForm(info);
                }
                model.id = Id;
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditProcessStatus [{0}] because: {1}", id, ex.ToString()));
            }

            return PartialView("Edit", model);
        }

        public ActionResult SaveUpDate(ProcessStatusEditModel model)
        {
            try
            {
                //Begin update
                var apiProcessStatusModel = ExtractProcessStatusFormData(model);
                var apiReturned = ProcessStatusServices.UpdateAsync(apiProcessStatusModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            return Json(new AjaxResponseModel { Success = false, Message = apiReturned.error.message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new AjaxResponseModel { Success = true, Message = ManagerResource.LB_UPDATE_SUCCESS }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to EditProcessStatus because: " + ex.ToString());
            }

            return Json(new AjaxResponseModel { Success = true, Message = ManagerResource.LB_UPDATE_SUCCESS }, JsonRequestBehavior.AllowGet);

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

                var apiReturned = ProcessStatusServices.DeleteAsync(id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            return Json(new AjaxResponseModel { Success = false, Message = apiReturned.error.message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete ProcessStatus because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }
            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
        }
        [HttpPost]
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
                {
                    var apiReturned = ProcessStatusServices.UpdateSortingAsync(sortList, GetCurrentAgencyId()).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.status == (int)HttpStatusCode.OK)
                        {
                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                            {
                                return Json(new AjaxResponseModel { Success = false, Message = apiReturned.error.message }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new AjaxResponseModel { Success = true, Message = ManagerResource.LB_UPDATE_SUCCESS }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                return Json(new AjaxResponseModel { Success = true, Message = ManagerResource.LB_UPDATE_SUCCESS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                return Json(new AjaxResponseModel { Success = false, Message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT }, JsonRequestBehavior.AllowGet);
            }
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
        #region Helpers

        private ProcessStatusEditModel ParseDataToForm(IdentityProcessStatus info)
        {
            var model = new ProcessStatusEditModel();
            if (info != null)
            {
                model.id = info.id;
                model.status_name = info.status_name;
                model.status = info.status == 1 ? true : false;
                model.description = info.description;
                model.order = info.order;
            }

            return model;
        }
        private ApiProcessStatusInsertModel ExtractProcessStatusInsertFormData(ProcessStatusEditModel model)
        {
            var info = new ApiProcessStatusInsertModel();
            if (info != null)
            {
                info.status_name = model.status_name;
                info.status = model.status == true ? 1 : 0;
                info.description = model.description;
                info.order = model.order;
                info.agency_id = GetCurrentAgencyId();
            }
            return info;
        }
        private ApiProcessStatusEditModel ExtractProcessStatusFormData(ProcessStatusEditModel model)
        {
            var info = new ApiProcessStatusEditModel();
            if (info != null)
            {
                info.id = model.id;
                info.status_name = model.status_name;
                info.status = model.status == true ? 1 : 0;
                info.description = model.description;
                info.order = model.order;
                info.agency_id = GetCurrentAgencyId();
            }
            return info;
        }

        #endregion

    }
}