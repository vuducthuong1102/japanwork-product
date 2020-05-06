using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Services;
using Manager.WebApp.Settings;
using Newtonsoft.Json;
using Manager.SharedLibs;
using ApiJobMarket.DB.Sql.Entities;
using Manager.SharedLibs;

namespace Manager.WebApp.Controllers
{
    public class SalaryFilterController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<SalaryFilterController>();

        public SalaryFilterController()
        {

        }

        [AccessRoleChecker]
        public ActionResult Index(ManageSalaryFilterModel model)
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

            var filter = new ApiSalaryFilterSearchModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize,
            };

            try
            {
                var result = SalaryFilterServices.GetAllAsync().Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentitySalaryFilter>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        model.TotalCount = model.SearchResults[0].total_count;
                        model.CurrentPage = currentPage;
                        model.PageSize = pageSize;
                    }
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
        [AccessRoleChecker]
        public ActionResult Create()
        {
            var model = new SalaryFilterEditModel();
            model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();

            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_SalaryFilter(SalaryFilterEditModel model)
        {
            try
            {
                //Begin create
                var apiSalaryFilterModel = ExtractSalaryFilterInsertFormData(model);

                var apiReturned = SalaryFilterServices.InsertAsync(apiSalaryFilterModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            this.AddNotification(apiReturned.error.message, NotificationType.ERROR);
                        }
                        else
                        {
                            this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to CreateSalaryFilter because: " + ex.ToString());
            }

            return RedirectToAction("Index");
        }
        [AccessRoleChecker]
        public ActionResult Edit(int? id)
        {
            ModelState.Clear();
            var Id = Utils.ConvertToIntFromQuest(id);
            var model = new SalaryFilterEditModel();
            
            try
            {
                if (Id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var result = SalaryFilterServices.GetDetailAsync(id ?? 0).Result;
                if (result != null && result.value != null)
                {
                    var info = JsonConvert.DeserializeObject<IdentitySalaryFilter>(result.value.ToString());
                    model = ParseDataToForm(info);
                }
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                model.id = Id;
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditSalaryFilter [{0}] because: {1}", id, ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_SalaryFilter(SalaryFilterEditModel model)
        {
            try
            {
                //Begin update
                var apiSalaryFilterModel = ExtractSalaryFilterFormData(model);
                var apiReturned = SalaryFilterServices.UpdateAsync(apiSalaryFilterModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            this.AddNotification(apiReturned.error.message, NotificationType.ERROR);
                        }
                        else
                        {
                            this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to EditSalaryFilter because: " + ex.ToString());
            }

            return RedirectToAction("Edit/" + model.id);
        }

        //Show popup confirm delete        
        [AccessRoleChecker]
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

                var apiReturned = SalaryFilterServices.DeleteAsync(id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            this.AddNotification(apiReturned.error.message, NotificationType.ERROR);
                        }
                        else
                        {
                            this.AddNotification(ManagerResource.LB_DELETE_SUCCESS, NotificationType.SUCCESS);
                        }
                    }
                }                          
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete SalaryFilter because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }
            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
        }

        #region Helpers

        private SalaryFilterEditModel ParseDataToForm(IdentitySalaryFilter info)
        {
            var model = new SalaryFilterEditModel();
            if (info != null)
            {
                model.id = info.id;
                if (info.min > 0)
                {
                    model.key = "min";
                    model.number = info.min;
                }
                if (info.max > 0)
                {
                    model.key = "max";
                    model.number = info.max;
                }
                model.type = info.type;
            }

            return model;
        }
        private ApiSalaryFilterInsertModel ExtractSalaryFilterInsertFormData(SalaryFilterEditModel model)
        {
            var info = new ApiSalaryFilterInsertModel();
            if (model.key == "min")
            {
                info.min = model.number;
            }
            else
            {
                info.max = model.number;
            }
            info.type = model.type;
            return info;
        }
        private ApiSalaryFilterEditModel ExtractSalaryFilterFormData(SalaryFilterEditModel model)
        {
            var info = new ApiSalaryFilterEditModel();
            if (model.key == "min")
            {
                info.min = model.number;
            }
            else
            {
                info.max = model.number;
            }
            info.id = model.id;
            info.type = model.type;
            return info;
        }

        #endregion

    }
}