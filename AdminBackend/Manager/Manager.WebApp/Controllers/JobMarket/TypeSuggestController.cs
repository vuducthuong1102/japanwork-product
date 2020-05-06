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
    public class TypeSuggestController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<TypeSuggestController>();

        public TypeSuggestController()
        {

        }

        [AccessRoleChecker]
        public ActionResult Index(ManageTypeSuggestModel model)
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

            var filter = new ApiTypeSuggestSearchModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize,
            };

            try
            {
                var result = TypeSuggestServices.GetAllAsync().Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityTypeSuggest>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
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
            var model = new TypeSuggestEditModel();
            model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
            model.form_id = (int)EnumFormCv.Official;
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_TypeSuggest(TypeSuggestEditModel model)
        {
            try
            {
                //Begin create
                var apiTypeSuggestModel = ExtractTypeSuggestInsertFormData(model);

                var apiReturned = TypeSuggestServices.InsertAsync(apiTypeSuggestModel).Result;
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

                logger.Error("Failed to CreateTypeSuggest because: " + ex.ToString());
            }

            return RedirectToAction("Index");
        }
        [AccessRoleChecker]
        public ActionResult Edit(int? id)
        {
            ModelState.Clear();
            var Id = Utils.ConvertToIntFromQuest(id);
            var model = new TypeSuggestEditModel();
            
            try
            {
                if (Id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var result = TypeSuggestServices.GetDetailAsync(id ?? 0).Result;
                if (result != null && result.value != null)
                {
                    var info = JsonConvert.DeserializeObject<IdentityTypeSuggest>(result.value.ToString());
                    model = ParseDataToForm(info);
                }
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                model.id = Id;
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditTypeSuggest [{0}] because: {1}", id, ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_TypeSuggest(TypeSuggestEditModel model)
        {
            try
            {
                //Begin update
                var apiTypeSuggestModel = ExtractTypeSuggestFormData(model);
                var apiReturned = TypeSuggestServices.UpdateAsync(apiTypeSuggestModel).Result;
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

                logger.Error("Failed to EditTypeSuggest because: " + ex.ToString());
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

                var apiReturned = TypeSuggestServices.DeleteAsync(id).Result;
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

                logger.Error("Failed to get Delete TypeSuggest because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }
            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
        }

        #region Helpers

        private TypeSuggestEditModel ParseDataToForm(IdentityTypeSuggest info)
        {
            var model = new TypeSuggestEditModel();
            if (info != null)
            {
                model.id = info.id;
                model.form_id = info.form_id;
                model.type = info.type;
                model.description = info.description;
                model.icon = info.icon;
            }

            return model;
        }
        private ApiTypeSuggestInsertModel ExtractTypeSuggestInsertFormData(TypeSuggestEditModel model)
        {
            var info = new ApiTypeSuggestInsertModel();
            info.form_id = model.id;
            info.type = model.type;
            info.description = model.description;
            info.icon = model.icon;
            return info;
        }
        private ApiTypeSuggestEditModel ExtractTypeSuggestFormData(TypeSuggestEditModel model)
        {
            var info = new ApiTypeSuggestEditModel();
            info.id = model.id;
            info.type = model.type;
            info.form_id = model.form_id;
            info.description = model.description;
            info.icon = model.icon;
            return info;
        }

        #endregion

    }
}