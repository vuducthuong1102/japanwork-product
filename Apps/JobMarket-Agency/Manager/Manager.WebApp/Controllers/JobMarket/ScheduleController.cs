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

namespace Manager.WebApp.Controllers
{
    public class ScheduleController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<ScheduleController>();
        public const string DATE_FORMAT = "yyyy-MM-dd";
        public const string CST_EventTimeFormat = "yyyy-MM-dd H:mm";
        public const string CST_MomentTimeFormat = "yyyy-MM-ddTHH:mm:ss";

        public ActionResult ShowCalendar()
        {
            var model = new ScheduleCalendarModel();
            var currentUser = GetCurrentUser();
            if (currentUser.ParentId == 0)
                model.isAgency = true;

            return PartialView("Partials/_Calendar", model);
        }

        [HttpPost]
        public ActionResult GetSchedules()
        {
            var schedules = new List<ScheduleJsonModel>();
            var strError = string.Empty;
            try
            {
                var apiModel = new ApiScheduleByStaffModel();
                apiModel.agency_id = GetCurrentAgencyId();

                var staff_id = Request["staff_id"] != null ? Utils.ConvertToInt32(Request["staff_id"]) : 0;
                if (staff_id == 0)
                    staff_id = GetCurrentStaffId();

                apiModel.start_time = Request["start"] != null ? Request["start"].ToString() : string.Empty;
                apiModel.end_time = Request["end"] != null ? Request["end"].ToString() : string.Empty;
                apiModel.staff_id = staff_id;

                var apiResult = AgencyServices.GetSchedulesByStaffAsync(apiModel).Result;
                if(apiResult != null && apiResult.value != null)
                {
                    List<IdentitySchedule> listSchedules = JsonConvert.DeserializeObject<List<IdentitySchedule>>(apiResult.value.ToString());
                    if (listSchedules.HasData())
                    {
                        foreach (var item in listSchedules)
                        {
                            var sd = new ScheduleJsonModel();
                            sd.id = item.id.ToString();
                            sd.title = item.title;

                            if (item.schedule_cat != (int)EnumScheduleCategory.Others)
                            {                               
                                foreach (var en in Enum.GetValues(typeof(EnumScheduleCategory)))
                                {
                                    var chkItem = (int)en;
                                    var chkStr = string.Empty;
                                    if (chkItem == item.schedule_cat)
                                    {
                                        sd.title = EnumExtensions.GetEnumDescription((Enum)en);
                                    }
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(item.title))
                                    sd.title = EnumExtensions.GetEnumDescription(EnumScheduleCategory.Others);
                            }
                            
                            sd.pic_id = item.pic_id;
                            sd.content = item.content;
                            sd.start = item.start_time.DateTimeQuestToString(CST_MomentTimeFormat, false);
                            sd.end = item.end_time.DateTimeQuestToString(CST_MomentTimeFormat, false);
                            sd.editable = true;

                            schedules.Add(sd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to get GetSchedules because: " + ex.ToString());
            }

            return new JsonResult
            {
                Data = new { success = string.IsNullOrEmpty(strError), message = strError, events = schedules },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        [PreventCrossOrigin]
        public ActionResult GetTodayTasks()
        {
            var schedules = new List<ScheduleWidgetModel>();
            var strError = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                var apiModel = new ApiScheduleByStaffModel();
                apiModel.agency_id = GetCurrentAgencyId();

                var staff_id = Request["staff_id"] != null ? Utils.ConvertToInt32(Request["staff_id"]) : 0;
                if (staff_id == 0)
                    staff_id = GetCurrentStaffId();

                var dtNowStr = DateTime.Now.ToString(DATE_FORMAT);

                apiModel.start_time = dtNowStr;
                apiModel.end_time = dtNowStr;
                apiModel.staff_id = staff_id;

                List<IdentitySchedule> listSchedules = null;
                var apiResult = AgencyServices.GetTodaySchedulesByStaffAsync(apiModel).Result;
                if (apiResult != null && apiResult.value != null)
                {
                    listSchedules = JsonConvert.DeserializeObject<List<IdentitySchedule>>(apiResult.value.ToString());

                    if (listSchedules.HasData())
                    {
                        foreach (var item in listSchedules)
                        {
                            var sd = new ScheduleWidgetModel();
                            sd.title = item.title;

                            if (item.schedule_cat != (int)EnumScheduleCategory.Others)
                            {
                                foreach (var en in Enum.GetValues(typeof(EnumScheduleCategory)))
                                {
                                    var chkItem = (int)en;
                                    var chkStr = string.Empty;
                                    if (chkItem == item.schedule_cat)
                                    {
                                        sd.title = EnumExtensions.GetEnumDescription((Enum)en);
                                    }
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(item.title))
                                    sd.title = EnumExtensions.GetEnumDescription(EnumScheduleCategory.Others);
                            }

                            sd.data = item;
                            schedules.Add(sd);
                        }
                    }
                }

                htmlReturn = PartialViewAsString("../Widgets/Area/Schedule/_TodayTasks", schedules);
            }
            catch (Exception ex)
            {
                logger.Error("Failed to get GetTodayTasks because: " + ex.ToString());
            }

            return new JsonResult
            {
                Data = new { success = true, html = htmlReturn },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpGet]
        public ActionResult EditSchedulePartial(string id)
        {            
            ScheduleUpdateModel returnModel = null;
            var isReadOnly = false;
            try
            {
                var apiModel = new ApiScheduleModel();
                apiModel.id = Utils.ConvertToInt32(Request["id"]);
                apiModel.agency_id = GetCurrentAgencyId();              

                var staff_id = Request["staff_id"] != null ? Utils.ConvertToInt32(Request["staff_id"]) : 0;
                if (staff_id == 0)
                    staff_id = GetCurrentStaffId();
                else
                    isReadOnly = true;

                apiModel.staff_id = staff_id;

                var apiResult = A_ScheduleServices.GetDetailAsync(apiModel).Result;
                if (apiResult != null && apiResult.value != null)
                {
                    IdentitySchedule info = JsonConvert.DeserializeObject<IdentitySchedule>(apiResult.value.ToString());
                    if(info != null)
                    {
                        returnModel = new ScheduleUpdateModel();

                        returnModel.sd_id = info.id.ToString();
                        returnModel.sd_pic_id = info.pic_id;
                        returnModel.sd_schedule_cat = info.schedule_cat;
                        returnModel.sd_title = info.title;
                        returnModel.sd_content = info.content;
                        returnModel.sd_start_time = info.start_time.DateTimeQuestToString("yyyy-MM-dd HH:mm", false);
                        returnModel.sd_end_time = info.end_time.DateTimeQuestToString("yyyy-MM-dd HH:mm", false);

                        returnModel.StaffList = CommonHelpers.GetListUser(GetCurrentAgencyId());
                    }
                }                
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Could not EditOrViewSchedulePartial Schedule: {0}", ex.ToOriginalExeception().ToString());
            }
            
            if(!isReadOnly)
                return PartialView("Partials/_EditSchedulePartial", returnModel);
            else
                return PartialView("Partials/_ViewSchedulePartial", returnModel);
        }

        [HttpGet]
        public ActionResult CreateSchedulePartial()
        {
            ScheduleUpdateModel returnModel = new ScheduleUpdateModel();
            try
            {
                returnModel.StaffList = CommonHelpers.GetListUser(GetCurrentAgencyId());

                var startTime = Request["start_time"] != null ? Request["start_time"].ToString() : string.Empty;
                var dtNow = DateTime.Now;
                var defaultDt = dtNow.ToString("yyyy-MM-dd") + " 00:00";
                if(startTime == defaultDt)
                {
                    returnModel.sd_start_time = dtNow.ToString("yyyy-MM-dd HH:mm");
                    returnModel.sd_end_time = dtNow.AddHours(1).ToString("yyyy-MM-dd HH:mm");
                }
                else
                {
                    returnModel.sd_start_time = startTime.Replace("00:00", "09:00");
                    returnModel.sd_end_time = startTime.Replace("00:00", "10:00"); ;
                }
                //var agency_id = GetCurrentAgencyId();

                //returnModel.StaffList = new List<MsSql.AspNet.Identity.IdentityUser>();
                //var tmpList = CommonHelpers.GetListUser(GetCurrentAgencyId());
                //if (tmpList.HasData())
                //{
                //    foreach (var item in tmpList)
                //    {
                //        if (item.StaffId != agency_id)
                //        {
                //            returnModel.StaffList.Add(item);
                //        }
                //    }
                //}                
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Could not CreateSchedulePartial Schedule: {0}", ex.ToOriginalExeception().ToString());
            }

            return PartialView("Partials/_CreateSchedulePartial", returnModel);  
        }

        [HttpPost]
        //[AccessRoleChecker]
        public JsonResult CreateSchedule(ScheduleUpdateModel model)
        {
            var message = string.Empty;           
            var eventId = string.Empty;
            var isSuccess = false;
            DateTime? startTime = null;
            DateTime? endTime = null;

            try
            {
                startTime = DateTime.ParseExact(model.sd_start_time.Trim(), CST_EventTimeFormat, null);
            }
            catch
            {
                startTime = null;
            }

            try
            {
                endTime = DateTime.ParseExact(model.sd_end_time.Trim(), CST_EventTimeFormat, null);
            }
            catch
            {
                endTime = null;
            }

            if (startTime == null || endTime == null)
            {
                message = ManagerResource.LB_START_END_TIME_REQUIRED;
            }
            else
            {
                if (startTime > endTime)
                {
                    message = ManagerResource.LB_START_LARGE_END_TIME_ERROR;
                }
            }

            if (string.IsNullOrEmpty(message))
            {
                var apiModel = new ApiScheduleModel
                {
                    title = model.sd_title,
                    start_time = model.sd_start_time,
                    end_time = model.sd_end_time,
                    schedule_cat = model.sd_schedule_cat,
                    pic_id = model.sd_pic_id,
                    content = model.sd_content,
                    agency_id = GetCurrentAgencyId(),
                    staff_id = GetCurrentStaffId()
                };

                try
                {
                    if(model.sd_schedule_cat != (int)EnumScheduleCategory.Others)
                    {
                        apiModel.title = string.Empty;
                    }

                    var apiResult = A_ScheduleServices.UpdateAsync(apiModel).Result;
                    if (apiResult != null && apiResult.value != null)
                    {
                        isSuccess = true;
                        message = ManagerResource.LB_UPDATE_SUCCESS;

                        eventId = apiResult.value.ToString();
                    }
                    else
                    {
                        message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("Could not Create Schedule: {0}", ex.ToOriginalExeception().ToString());
                    message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                }
            }

            return new JsonResult { Data = new { success = isSuccess, message = message, eventid = eventId } };
        }

        [HttpPost]
        //[AccessRoleChecker]
        public JsonResult UpdateSchedule(ScheduleUpdateModel model)
        {
            var message = string.Empty;
            var eventId = string.Empty;
            var isSuccess = false;
            DateTime? startTime = null;
            DateTime? endTime = null;

            if (string.IsNullOrEmpty(message))
            {
                try
                {
                    try
                    {
                        startTime = DateTime.ParseExact(model.sd_start_time.Trim(), CST_EventTimeFormat, null);
                    }
                    catch
                    {
                        startTime = null;
                    }

                    try
                    {
                        endTime = DateTime.ParseExact(model.sd_end_time.Trim(), CST_EventTimeFormat, null);
                    }
                    catch
                    {
                        endTime = null;
                    }


                    if (startTime == null || endTime == null)
                    {
                        message = string.Format("Both start_time and end_time are required.");
                    }
                    else
                    {
                        if (startTime > endTime)
                        {
                            message = string.Format("start_time cannot be larger than end_time.");
                        }
                    }

                    var apiModel = new ApiScheduleModel
                    {
                        id = Utils.ConvertToInt32(model.sd_id),
                        schedule_cat = model.sd_schedule_cat,
                        pic_id = model.sd_pic_id,
                        title = model.sd_title,
                        start_time = model.sd_start_time,
                        end_time = model.sd_end_time,
                        content = model.sd_content,
                        agency_id = GetCurrentAgencyId(),
                        staff_id = GetCurrentStaffId()
                    };

                    if (model.sd_schedule_cat != (int)EnumScheduleCategory.Others)
                    {
                        apiModel.title = string.Empty;
                    }

                    var apiResult = A_ScheduleServices.UpdateAsync(apiModel).Result;
                    if(apiResult != null && apiResult.value != null)
                    {
                        isSuccess = true;
                        message = ManagerResource.LB_UPDATE_SUCCESS;
                    }
                    else
                    {
                        message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("Could not Update Schedule: {0}", ex.ToOriginalExeception().ToString());
                    message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                }
            }

            return new JsonResult { Data = new { success = isSuccess, message = message } };
        }

        [HttpPost]
        //[AccessRoleChecker]
        public JsonResult UpdateTime()
        {
            var message = string.Empty;
            var eventId = string.Empty;
            var isSuccess = false;

            try
            {
                var isoStartTime = Request["start"] != null ? Request["start"].ToString() : string.Empty;
                var isoEndTime = Request["end"] != null ? Request["end"].ToString() : string.Empty;

                DateTime? startTime = Utils.ConvertStringToDateTimeByFormat(isoStartTime, CST_MomentTimeFormat);
                DateTime? endTime = Utils.ConvertStringToDateTimeByFormat(isoEndTime, CST_MomentTimeFormat);

                var apiModel = new ApiScheduleModel();
                apiModel.id = Utils.ConvertToInt32(Request["id"]);
                apiModel.start_time = startTime.DateTimeQuestToString("yyyy-MM-dd HH:mm", false);
                apiModel.end_time = endTime.DateTimeQuestToString("yyyy-MM-dd HH:mm", false);
                apiModel.agency_id = GetCurrentAgencyId();
                apiModel.staff_id = GetCurrentStaffId();

                var apiResult = A_ScheduleServices.UpdateScheduleTimeAsync(apiModel).Result;
                if (apiResult != null && apiResult.value != null)
                {
                    isSuccess = true;
                    message = ManagerResource.LB_UPDATE_SUCCESS;
                }
                else
                {
                    message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Could not UpdateScheduleTime : {0}", ex.ToOriginalExeception().ToString());
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
            }

            return new JsonResult { Data = new { success = isSuccess, message = message } };
        }

        [HttpPost]
        //[AccessRoleChecker]
        public JsonResult DeleteSchedule(ScheduleUpdateModel model)
        {
            var message = string.Empty;
            var eventId = string.Empty;
            var isSuccess = false;
            try
            {
                var apiModel = new ApiScheduleModel
                {
                    id = Utils.ConvertToInt32(model.sd_id),
                    agency_id = GetCurrentAgencyId(),
                    staff_id = GetCurrentStaffId()
                };

                var apiResult = A_ScheduleServices.DeleteAsync(apiModel).Result;
                if (apiResult != null && apiResult.value != null)
                {
                    isSuccess = true;
                    message = ManagerResource.LB_DELETE_SUCCESS;
                }
                else
                {
                    message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Could not Delete Schedule: {0}", ex.ToOriginalExeception().ToString());
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
            }

            return new JsonResult { Data = new { success = isSuccess, message = message } };
        }

        //[AccessRoleChecker]
        public ActionResult Index()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                //return View(model);
            }

            return View();
        }
    }
}