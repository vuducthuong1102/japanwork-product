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
using Newtonsoft.Json;
using Manager.SharedLibs;
using ApiJobMarket.DB.Sql.Entities;
using Manager.SharedLibs;
using System.Linq;

namespace Manager.WebApp.Controllers
{
    public class SuggestController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<SuggestController>();

        public SuggestController()
        {

        }

        [AccessRoleChecker]
        public ActionResult Index(ManageSuggestModel model)
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

            var filter = new ApiSuggestSearchModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize,
            };

            try
            {
                var result = SuggestServices.GetByPageAsync(filter).Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentitySuggest>>(result.value.ToString());
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

        public ActionResult Create()
        {
            var model = new SuggestEditModel();
            model.form = (int)EnumFormCv.Official;
            model.ListTypeSuggests = CommonHelpers.GetListTypeSuggests();
            if (model.ListTypeSuggests.HasData()) model.ListTypeSuggests = model.ListTypeSuggests.Where(s => s.form_id == model.form).ToList();
            if (model.ListTypeSuggests.HasData())
            {
                model.type = model.ListTypeSuggests[0].id;
            }

            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Suggest(SuggestEditModel model)
        {
            try
            {
                //Begin create
                var apiSuggestModel = ExtractSuggestInsertFormData(model);

                var apiReturned = SuggestServices.InsertAsync(apiSuggestModel).Result;
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

                logger.Error("Failed to CreateSuggest because: " + ex.ToString());
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            ModelState.Clear();
            var suggestId = Utils.ConvertToIntFromQuest(id);
            var model = new SuggestEditModel();
            try
            {
                if (suggestId <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var result = SuggestServices.GetDetailAsync(id ?? 0).Result;
                if (result != null && result.value != null)
                {
                    var info = JsonConvert.DeserializeObject<IdentitySuggest>(result.value.ToString());
                    model = ParseDataToForm(info);
                }

                model.id = suggestId;

                model.ListTypeSuggests = CommonHelpers.GetListTypeSuggests();
                if (model.ListTypeSuggests.HasData()) model.ListTypeSuggests = model.ListTypeSuggests.Where(s => s.form_id == model.form).ToList();

            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditSuggest [{0}] because: {1}", id, ex.ToString()));
            }

            return View(model);
        }

        public JsonResult GetListTypeSuggests(int? form_id)
        {
            List<IdentityTypeSuggest> listResult = new List<IdentityTypeSuggest>();
            try
            {
                listResult = CommonHelpers.GetListTypeSuggests().Where(s => s.form_id == form_id).ToList();
            }
            catch
            {
            }
            return Json(listResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Suggest(SuggestEditModel model)
        {
            try
            {
                //Begin update
                var apiSuggestModel = ExtractSuggestFormData(model);
                var apiReturned = SuggestServices.UpdateAsync(apiSuggestModel).Result;
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

                logger.Error("Failed to EditSuggest because: " + ex.ToString());
            }

            return RedirectToAction("Edit/" + model.id);
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

                var apiReturned = SuggestServices.DeleteAsync(id).Result;
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

                logger.Error("Failed to get Delete Suggest because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }
            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
        }

        #region Helpers

        private SuggestEditModel ParseDataToForm(IdentitySuggest info)
        {
            var model = new SuggestEditModel();
            if (info != null)
            {
                model.id = model.id;
                model.content = info.content;
                model.title = info.title;
                model.type = info.type;
                model.form = info.form;
                if (info.LangList.HasData())
                {
                    var myLang = info.LangList.Where(x => x.language_code == _currentLanguage).FirstOrDefault();
                    if (myLang != null)
                    {
                        model.content = myLang.content;
                        model.title = myLang.title;
                    }
                }
            }

            return model;
        }
        private ApiSuggestInsertModel ExtractSuggestInsertFormData(SuggestEditModel model)
        {
            var info = new ApiSuggestInsertModel();
            info.content = model.content;
            info.title = model.title;
            info.type = model.type;
            info.form = model.form;
            return info;
        }
        private ApiSuggestEditModel ExtractSuggestFormData(SuggestEditModel model)
        {
            var info = new ApiSuggestEditModel();
            info.id = model.id;
            info.content = model.content;
            info.title = model.title;
            info.type = model.type;
            info.form = model.form;
            return info;
        }

        #endregion

    }
}