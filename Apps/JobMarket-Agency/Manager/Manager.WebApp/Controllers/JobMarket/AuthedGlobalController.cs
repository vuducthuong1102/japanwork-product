using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Linq;
using ApiJobMarket.DB.Sql.Entities;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Models;
using Manager.WebApp.Services;
using Manager.WebApp.Settings;
using Manager.SharedLibs;
using Manager.WebApp.Helpers;

namespace Manager.WebApp.Controllers
{
    [Authorize]
    public class AuthedGlobalController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<AuthedGlobalController>();

        [HttpPost]
        [VerifyLoggedInUser]
        public ActionResult GetNotifCount()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var msg = string.Empty;
            var data = 0;
            try
            {
                var apiInputModel = new ApiAgencyStaffModel
                {
                    staff_id = GetCurrentStaffId()
                };

                var apiReturned = A_NotificationServices.GetUnreadNotifCountAsync(apiInputModel).Result;

                if (apiReturned != null)
                {
                    isSuccess = true;
                    data = apiReturned.total;
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetNotifCount because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, message = msg, data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [VerifyLoggedInUser]
        public async Task<ActionResult> GetNotification()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            List<AgencyNotificationModel> returnList = new List<AgencyNotificationModel>();
            var msg = string.Empty;
            try
            {
                var apiFilterModel = new ApiAgencyGetListNotificationModel();

                apiFilterModel.agency_id = GetCurrentAgencyId();
                apiFilterModel.staff_id = GetCurrentStaffId();
                apiFilterModel.page_index = 1;
                apiFilterModel.page_size = SystemSettings.DefaultPageSize;
                apiFilterModel.is_update_view = 1;

                var apiReturned = A_NotificationServices.GetNotificationsAsync(apiFilterModel).Result;

                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        List<IdentityAgencyNotification> listNotif = JsonConvert.DeserializeObject<List<IdentityAgencyNotification>>(apiReturned.value.ToString());
                        List<IdentityCv> listCvs = new List<IdentityCv>();
                        List<IdentityJobSeeker> listJobSeekers = new List<IdentityJobSeeker>();
                        await Task.FromResult(listNotif);

                        if (listNotif.HasData())
                        {
                            var cvIds = new List<int>();
                            var job_seekerIds = new List<int>();
                            var jobIds = new List<int>();
                            foreach (var item in listNotif)
                            {
                                var notif = new AgencyNotificationModel();
                                notif.NotifInfo = item;

                                if (item.target_type == (int)EnumNotifTargetType.Cv)
                                {
                                    if (item.action_type == (int)EnumNotifActionTypeForAgency.Invitation_Canceled)
                                    {
                                        job_seekerIds.Add(item.target_id);
                                    }
                                    else
                                    {
                                        cvIds.Add(item.target_id);
                                    }
                                }

                                if (item.target_type == (int)EnumNotifTargetType.Job)
                                {
                                    jobIds.Add(item.target_id);
                                }

                                returnList.Add(notif);
                            }

                            if (cvIds.HasData())
                            {
                                var apiResult = CvServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = cvIds }).Result;
                                if (apiResult != null && apiResult.value != null)
                                {
                                    listCvs = JsonConvert.DeserializeObject<List<IdentityCv>>(apiResult.value.ToString());
                                }
                            }

                            if (job_seekerIds.HasData())
                            {
                                var apiResult = JobSeekerServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = job_seekerIds }).Result;
                                if (apiResult != null && apiResult.value != null)
                                {
                                    listJobSeekers = JsonConvert.DeserializeObject<List<IdentityJobSeeker>>(apiResult.value.ToString());
                                }
                            }

                            var hasCvs = listCvs.HasData();
                            var hasJobSeekers = listJobSeekers.HasData();
                            if (returnList.HasData())
                            {
                                foreach (var item in returnList)
                                {
                                    if (item.NotifInfo != null)
                                    {

                                        if (item.NotifInfo.action_type == (int)EnumNotifActionTypeForAgency.Invitation_Canceled && hasJobSeekers)
                                        {
                                            var jobSeekerInfo = listJobSeekers.FirstOrDefault(x => x.user_id == item.NotifInfo.target_id);
                                            if (jobSeekerInfo != null)
                                            {
                                                item.CvInfo = ParseDataToForm(jobSeekerInfo);
                                            }
                                        }
                                        else
                                        {
                                            if (hasCvs && item.NotifInfo.target_type == (int)EnumNotifTargetType.Cv)
                                                item.CvInfo = listCvs.Where(x => x.id == item.NotifInfo.target_id).FirstOrDefault();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    isSuccess = true;
                    htmlReturn = PartialViewAsString("../Widgets/Items/Notification/_NotificationItems", returnList);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetNotification because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        private IdentityCv ParseDataToForm(IdentityJobSeeker identity)
        {
            var CvInfo = new IdentityCv();
            CvInfo.job_seeker_id = identity.user_id;
            CvInfo.id = identity.user_id;
            CvInfo.email = identity.email;
            CvInfo.phone = identity.phone;
            CvInfo.marriage = (identity.marriage == 0 ? false : true);
            CvInfo.dependent_num = identity.dependent_num;
            CvInfo.fullname = identity.fullname;
            CvInfo.fullname_furigana = identity.fullname_furigana;
            CvInfo.image = identity.image;
            CvInfo.birthday = identity.birthday;
            CvInfo.date = identity.created_at;
            CvInfo.gender = identity.gender;
            CvInfo.status = 1;
            CvInfo.qualification_id = identity.qualification_id;

            CvInfo.japanese_level_number = identity.japanese_level_number;

            CvInfo.Extensions = identity.Extensions;
            return CvInfo;
        }
        [HttpPost]
        [VerifyLoggedInUser]
        public async Task<ActionResult> GetDashboardNotification()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            List<AgencyNotificationModel> returnList = new List<AgencyNotificationModel>();
            var msg = string.Empty;
            try
            {
                var filterModel = new ApiAgencyGetListNotificationModel();

                filterModel.agency_id = GetCurrentAgencyId();
                filterModel.staff_id = GetCurrentStaffId();
                filterModel.page_index = Utils.ConvertToInt32(Request["page"]);
                filterModel.is_update_view = 0;
                if (filterModel.page_index == 0)
                    filterModel.page_index = 1;

                filterModel.page_size = SystemSettings.DefaultPageSize;

                var apiReturned = A_NotificationServices.GetNotificationsAsync(filterModel).Result;

                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        List<IdentityAgencyNotification> listNotif = JsonConvert.DeserializeObject<List<IdentityAgencyNotification>>(apiReturned.value.ToString());
                        List<IdentityCv> listCvs = new List<IdentityCv>();
                        await Task.FromResult(listNotif);

                        if (listNotif.HasData())
                        {
                            var cvIds = new List<int>();
                            foreach (var item in listNotif)
                            {
                                var notif = new AgencyNotificationModel();
                                notif.NotifInfo = item;

                                if (item.target_type == (int)EnumNotifTargetType.Cv)
                                {
                                    cvIds.Add(item.target_id);
                                }

                                returnList.Add(notif);
                            }

                            if (cvIds.HasData())
                            {
                                var apiResult = CvServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = cvIds }).Result;
                                if (apiResult != null && apiResult.value != null)
                                {
                                    listCvs = JsonConvert.DeserializeObject<List<IdentityCv>>(apiResult.value.ToString());
                                }
                            }

                            var hasCvs = listCvs.HasData();
                            if (returnList.HasData())
                            {
                                foreach (var item in returnList)
                                {
                                    if (item.NotifInfo != null)
                                    {
                                        if (hasCvs && item.NotifInfo.target_type == (int)EnumNotifTargetType.Cv)
                                            item.CvInfo = listCvs.Where(x => x.id == item.NotifInfo.target_id).FirstOrDefault();
                                    }
                                }
                            }

                            isSuccess = true;
                        }
                    }
                }

                htmlReturn = PartialViewAsString("../Widgets/Area/Notification/_NotificationList", returnList);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetDashboardNotification because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        [IsValidURLRequest]
        [PreventCrossOrigin]
        public async Task<ActionResult> ReadNotif(int? id = 0)
        {
            var model = new AgencyNotificationModel();
            try
            {
                var notifId = Utils.ConvertToIntFromQuest(id);
                if (notifId == 0)
                    return RedirectToErrorPage();

                var apiModel = new ApiMarkIsReadAgencyNotificationModel();
                apiModel.id = notifId;
                apiModel.staff_id = GetCurrentStaffId();
                apiModel.is_read = true;

                IdentityAgencyNotification info = null;

                var apiResult = A_NotificationServices.MarkIsReadAsync(apiModel).Result;
                await Task.FromResult(apiResult);
                if (apiResult != null && apiResult.value != null)
                {
                    info = JsonConvert.DeserializeObject<IdentityAgencyNotification>(apiResult.value.ToString());
                    if (info == null)
                        return RedirectToErrorPage();

                    if (info.target_type == (int)EnumNotifTargetType.Cv)
                    {
                        if (info.action_type == (int)EnumNotifActionTypeForAgency.Invitation_Accepted
                            || info.action_type == (int)EnumNotifActionTypeForAgency.Invitation_Canceled)
                        {
                            return RedirectToAction("Index", "InvitationHistory");
                        }
                        return Redirect(SecurityHelper.GenerateSecureLink("Application", "Index", new { job_id = info.action_id, cv_id = info.target_id }));
                    }
                    if (info.target_type == (int)EnumNotifTargetType.Job)
                    {
                        if (info.action_type == (int)EnumNotifJob.Accepted || info.action_type == (int)EnumNotifJob.Canceled)
                        {
                            return RedirectToAction("Index", "Job");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to ReadNotification because: " + ex.ToString());
            }

            return Content("");
        }
    }
}
