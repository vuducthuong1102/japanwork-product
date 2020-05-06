using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Services;
using System;
using System.Net;
using System.Web.Mvc;

namespace Manager.WebApp.Controllers
{
    public class JobSeekerNoteController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<JobSeekerNoteController>();

        [HttpPost, ActionName("CreateNote")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNote_Post(ManageJobSeekerNoteModel model)
        {
            try
            {
                //Begin create
                var apiModel = ExtractJobSeekerNoteFormData(model);
                apiModel.staff_id = GetCurrentStaffId();
                var apiReturned = JobSeekerNoteServices.InsertAsync(apiModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            return Json(new { success = false, message = apiReturned.error.message, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowNote({0})", model.job_seeker_id) });
                        }
                        else
                        {
                            return Json(new { success = true, message = ManagerResource.LB_INSERT_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowNote({0})", model.job_seeker_id) });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to CreateNote_Post because: " + ex.ToString());
            }
            return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowNote({0})", model.job_seeker_id) });
        }

        [ActionName("EditNote"), HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateNote(ManageJobSeekerNoteModel model)
        {
            string message = string.Empty;
            bool isSuccess = false;
            try
            {
                ApiJobSeekerNoteUpdateModel expr_0F = this.ExtractJobSeekerNoteFormData(model);
                expr_0F.staff_id = base.GetCurrentStaffId();
                ApiResponseCommonModel apiReturned = JobSeekerNoteServices.UpdateAsync(expr_0F).Result;
                if (apiReturned != null && apiReturned.status == 200)
                {
                    ActionResult result;
                    if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                    {
                        message = apiReturned.error.message;
                        result = base.Json(new
                        {
                            success = isSuccess,
                            message = message,
                            title = ManagerResource.LB_NOTIFICATION,
                            clientcallback = string.Format("ShowNote({0})", model.job_seeker_id)
                        });
                        return result;
                    }
                    if (!string.IsNullOrEmpty(apiReturned.message))
                    {
                        result = base.Json(new
                        {
                            success = true,
                            message = apiReturned.message,
                            title = ManagerResource.LB_NOTIFICATION,
                            clientcallback = string.Format("ShowNote({0})", model.job_seeker_id)
                        });
                        return result;
                    }
                    result = base.Json(new
                    {
                        success = true,
                        message = ManagerResource.LB_UPDATE_SUCCESS,
                        title = ManagerResource.LB_NOTIFICATION,
                        clientcallback = string.Format("ShowNote({0})", model.job_seeker_id)
                    });
                    return result;
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                this.logger.Error("Failed to exec UpdateNote because: " + ex.ToString());
                ActionResult result = base.Json(new
                {
                    success = isSuccess,
                    message = message
                });
                return result;
            }
            return base.Json(new
            {
                success = isSuccess,
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT,
                title = ManagerResource.LB_NOTIFICATION,
                clientcallback = string.Format("ShowNote({0})", model.job_seeker_id)
            });
        }

        [ActionName("DeleteNote"), HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete_Confirm(JobSeekerNoteDeleteModel model)
        {
            string strError = string.Empty;
            if (model.id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                ApiResponseCommonModel arg_21_0 = JobSeekerNoteServices.DeleteAsync(model.id).Result;
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;
                this.logger.Error("Failed to get Delete JobSeekerNote because: " + ex.ToString());
                return base.Json(new
                {
                    success = false,
                    message = strError
                });
            }
            return base.Json(new
            {
                success = true,
                message = ManagerResource.LB_DELETE_SUCCESS,
                title = ManagerResource.LB_NOTIFICATION,
                clientcallback = string.Format("ShowNote({0})", model.job_seeker_id)
            });
        }

        private ApiJobSeekerNoteUpdateModel ExtractJobSeekerNoteFormData(ManageJobSeekerNoteModel model)
        {
            return new ApiJobSeekerNoteUpdateModel
            {
                job_seeker_id = model.job_seeker_id,
                id = model.Id,
                type = model.type,
                note = model.note,
                type_job_seeker = model.type_job_seeker,
                agency_id = GetCurrentAgencyId()
            };
        }
    }
}