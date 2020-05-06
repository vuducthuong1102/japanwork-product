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
using System.Linq;

namespace Manager.WebApp.Controllers
{
    public class JobSeekerController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<JobSeekerController>();

        public JobSeekerController()
        {

        }

        [AccessRoleChecker]
        public ActionResult Index(ManageJobSeekerModel model)
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

            var filter = new ApiJobSeekerByPageModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize
            };

            try
            {
                var result = JobSeekerServices.GetByPageAsync(filter).Result;
                if(result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJobSeeker>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        var listIds = model.SearchResults.Select(x => x.user_id).ToList();

                        var userApiResult = AccountServices.GetListUserProfileAsync(new ApiListUserInfoModel { ListUserId = listIds }).Result;
                        var hasUsers = false;
                        List< IdentityUser> listUsers = null;
                        if (userApiResult != null && userApiResult.Data != null)
                        {
                            listUsers = JsonConvert.DeserializeObject<List<IdentityUser>>(userApiResult.Data.ToString());
                            hasUsers = listUsers.HasData();
                        }

                        if (hasUsers)
                        {
                            foreach (var item in model.SearchResults)
                            {
                                var currentUser = listUsers.Where(x => x.Id == item.user_id).FirstOrDefault();
                                if(currentUser != null)
                                {
                                    item.account_active = currentUser.EmailConfirmed;
                                }
                            }
                        }

                        model.TotalCount = Utils.ConvertToInt32(result.total);
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
                //var info = _mainStore.GetById(id, UserCookieManager.GetCurrentLanguageOrDefault());

                //if(info != null)
                //{
                //    _mainStore.Delete(id);

                //    //Clear cache
                //    FrontendCachingHelpers.ClearJobSeekerCateById(id);
                //}                                
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete JobSeeker because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
        }

        public ActionResult ClearData()
        {
            var model = new JobSeekerEditModel();
            try
            {
                
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display ClearData because: {0}", ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("ClearData")]
        [ValidateAntiForgeryToken]
        public ActionResult ClearData_Confirm(JobSeekerEditModel model)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to ClearData because: " + ex.ToString());
            }

            return RedirectToAction("Index");
        }

        [IsValidURLRequest]
        public ActionResult GetListCvs(ManageJobSeekerCvModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            var apiFilterModel = new ApiCvByPageModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                job_seeker_id = model.JobSeekerId,
                page_index = currentPage,
                page_size = pageSize
            };

            try
            {
                var apiResult = CvServices.GetListByJobSeekerAsync(apiFilterModel).Result;

                if (apiResult != null && apiResult.value != null)
                {
                    model.TotalCount = apiResult.total;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;

                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCv>>(apiResult.value.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to show CvPaging form: " + ex.ToString());
            }

            return PartialView("Partials/_CvPaging", model);
        }

        [HttpPost]
        [PreventSpam(DelayRequest = 1)]
        [PreventCrossOrigin]
        public ActionResult CvSearch(ManageJobSeekerCvModel model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }

            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            if (Request["CurrentPage"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["CurrentPage"], 1);
            }

            var apiFilterModel = new ApiCvByPageModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                job_seeker_id = model.JobSeekerId,
                page_index = currentPage,
                page_size = pageSize
            };

            try
            {
                var apiResult = CvServices.GetListByJobSeekerAsync(apiFilterModel).Result;

                if (apiResult != null && apiResult.value != null)
                {
                    model.TotalCount = apiResult.total;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCv>>(apiResult.value.ToString());
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to show _CvList form: " + ex.ToString());
            }

            return PartialView("Partials/_CvList", model);
        }
    }
}