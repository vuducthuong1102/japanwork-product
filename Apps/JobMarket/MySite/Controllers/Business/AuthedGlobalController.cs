using System.Web.Mvc;
using MySite.Logging;
using MySite.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using MySite.Helpers;
using System.Threading.Tasks;
using MySite.ShareLibs;
using MySite.Attributes;
using MySite.Services;
using MySite.Caching;
using System.Linq;
using System.Net;
using ApiJobMarket.DB.Sql.Entities;
using MySite.Resources;
using MySite.Models.Cv;
using Rotativa;
using MySite.Settings;

namespace MySite.Controllers
{
    [Authorize]
    public class AuthedGlobalController : BaseAuthenticatedController
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
                var apiInputModel = new ApiCommonFilterModel
                {
                    job_seeker_id = AccountHelper.GetCurrentUserId()
                };

                var apiReturned = JobSeekerServices.GetUnreadNotifCountAsync(apiInputModel).Result;

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
            List<JobSeekerNotificationModel> returnList = new List<JobSeekerNotificationModel>();
            var msg = string.Empty;
            try
            {
                var apiFilterModel = new ApiGetListByPageModel();
                var jobSeekerId = AccountHelper.GetCurrentUserId();

                apiFilterModel.job_seeker_id = jobSeekerId;
                apiFilterModel.page_index = 1;
                apiFilterModel.page_size = SystemSettings.NotifPageSize;
                var apiReturned = JobSeekerServices.GetNotificationsAsync(apiFilterModel).Result;

                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        List<IdentityNotification> listNotif = JsonConvert.DeserializeObject<List<IdentityNotification>>(apiReturned.value.ToString());
                        List<IdentityJob> listJobs = new List<IdentityJob>();
                        await Task.FromResult(listNotif);

                        if (listNotif.HasData())
                        {
                            var jobIds = new List<int>();
                            foreach (var item in listNotif)
                            {
                                var notif = new JobSeekerNotificationModel();
                                notif.NotifInfo = item;

                                if(item.target_type == (int)EnumNotifTargetType.Job)
                                {
                                    jobIds.Add(item.target_id);
                                }

                                returnList.Add(notif);
                            }

                            if (jobIds.HasData())
                            {
                                var apiJobResult = JobServices.GetListAsync(new ApiGetListByIdsModel { ListIds = jobIds }).Result;
                                if(apiJobResult != null && apiJobResult.value != null)
                                {
                                    listJobs = JsonConvert.DeserializeObject<List<IdentityJob>>(apiJobResult.value.ToString());
                                }
                            }

                            var hasJobs = listJobs.HasData();
                            if (returnList.HasData())
                            {
                                foreach (var item in returnList)
                                {
                                    if (item.NotifInfo != null)
                                    {
                                        if (hasJobs && item.NotifInfo.target_type == (int)EnumNotifTargetType.Job)
                                            item.JobInfo = listJobs.Where(x => x.id == item.NotifInfo.target_id).FirstOrDefault(); 
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
    }
}
