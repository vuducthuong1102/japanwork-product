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
using System.Threading.Tasks;

namespace Manager.WebApp.Controllers
{
    public class ApplicationController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<ApplicationController>();

        public ApplicationController()
        {

        }

        public ActionResult Index(ManageApplicationModel model)
        {
            model = GetDefaultFilterModel(model);
            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.gender = -1;
                model.major_id = -1;
                model.SearchExec = "Y";                
                model.Status = (int)(EnumApplicationStatus.Applied);
                if (!ModelState.IsValid)
                    ModelState.Clear();
            }
            model.type_job_seeker = 0;

            var filter = new ApiApplicationModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = model.CurrentPage,
                page_size = model.PageSize,
                status = (model.Status == null) ? -99 : Utils.ConvertToIntFromQuest(model.Status),
                agency_id = GetCurrentAgencyId(),
                type_job_seeker = model.type_job_seeker,
                staff_id = model.staff_id,
                gender = model.gender,
                major_id = model.major_id,
                has_process = (model.has_process == true ? 1 : 0),
                employment_type_id = model.employment_type_id,
                sub_field_id = model.sub_field_id,
                qualification_id = model.qualification_id,
                prefecture_id = model.prefecture_id,
                age_min = model.age_min,
                age_max = model.age_max,
                japanese_level_number = model.japanese_level_number,
                visa_id = model.visa_id
            };

            var listCompanyIds = new List<int>();
            var listCVs = new List<int>();
            try
            {
                //if (model.company_id > 0)
                //{
                //    listCompanyIds.Add(model.company_id);

                //    var companyResult = CompanyServices.GetDetailAsync(new ApiCompanyModel { id = model.company_id }).Result;
                //    if (companyResult != null)
                //    {
                //        if (companyResult.value != null)
                //        {
                //            IdentityCompany companyInfo = JsonConvert.DeserializeObject<IdentityCompany>(companyResult.value.ToString());
                //            if (companyInfo != null)
                //            {
                //                if (companyInfo.agency_id != GetCurrentAgencyId())
                //                {
                //                    return RedirectToErrorPage();
                //                }

                //                model.CompanyInfo = companyInfo;
                //                if (companyInfo.LangList.HasData())
                //                {
                //                    model.company_name = companyInfo.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.company_name).FirstOrDefault();
                //                }

                //                if (string.IsNullOrEmpty(model.company_name))
                //                    model.company_name = companyInfo.company_name;
                //            }
                //            else
                //            {
                //                return RedirectToErrorPage();
                //            }
                //        }
                //    }
                //}

                var result = AgencyServices.GetApplicationsByPageAsync(filter).Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityApplication>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        model.TotalCount = Utils.ConvertToInt32(result.total);
                        //if (model.company_id <= 0)
                        //{
                        //    var resultListCompanyIds = model.SearchResults.Where(x => x.job_info != null).Select(x => x.job_info.company_id).ToList();
                        //    listCompanyIds.AddRange(resultListCompanyIds);
                        //}
                        listCVs = model.SearchResults.Where(x => x.cv_id > 0).Select(x => x.cv_id).ToList();
                    }
                }
                //if (listCompanyIds.HasData())
                //{
                //    var companyReturnApi = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCompanyIds }).Result;
                //    if (companyReturnApi != null && companyReturnApi.value != null)
                //        model.Companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companyReturnApi.value.ToString());

                //    if (model.company_id > 0 && model.Companies.HasData())
                //    {
                //        model.CompanyInfo = model.Companies.Where(x => x.id == model.company_id).FirstOrDefault();
                //    }
                //}
                //model.ListCompanies = CommonHelpers.GetCompanyManagers(GetCurrentStaffId(), GetCurrentLanguageOrDefault());

                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                model.Majors = CommonHelpers.GetListMajors();
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                model.Qualifications = CommonHelpers.GetListQualifications();
                //model.Fields = CommonHelpers.GetListFields();
                model.Visas = CommonHelpers.GetListVisas();

                //if (model.employment_type_id == 0 && model.EmploymentTypes.HasData())
                //{
                //    model.employment_type_id = model.EmploymentTypes[0].id;
                //}
                model.Fields = CommonHelpers.GetListFieldsByEmploymentType(model.employment_type_id);

                var country_id = 81;
                var apiReturned = RegionServices.GetListAsync(country_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        model.Regions = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiReturned.value.ToString());
                    }
                }
                //if (listCVs.HasData())
                //{
                //    var cvReturnApi = CvServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCVs }).Result;
                //    if (cvReturnApi != null && cvReturnApi.value != null)
                //        model.CVs = JsonConvert.DeserializeObject<List<IdentityCv>>(cvReturnApi.value.ToString());
                //}
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }
        public ActionResult SearchRender()
        {
            ManageApplicationModel model = new ManageApplicationModel();
            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.gender = -1;
                model.major_id = -1;
                model.SearchExec = "Y";
                model.type_job_seeker = 0;
                if (!ModelState.IsValid)
                    ModelState.Clear();
            }
            try
            {
                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                model.Majors = CommonHelpers.GetListMajors();
                model.Qualifications = CommonHelpers.GetListQualifications();
                //model.Fields = CommonHelpers.GetListFields();
                model.Visas = CommonHelpers.GetListVisas();

                //if (model.employment_type_id == 0 && model.EmploymentTypes.HasData())
                //{
                //    model.employment_type_id = model.EmploymentTypes[0].id;
                //}
                model.Fields = CommonHelpers.GetListFieldsByEmploymentType(model.employment_type_id);

                var country_id = 81;
                var apiReturned = RegionServices.GetListAsync(country_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        model.Regions = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to get PopupSearch: " + ex.ToString());
            }

            return PartialView("Partials/_PopupSearch", model);
        }

        [HttpPost]
        [AccessRoleChecker]
        public ActionResult UpdatePic(int job_seeker_id, int pic_id)
        {
            var strError = string.Empty;
            try
            {
                var model = new ApiApplicationPicModel
                {
                    job_seeker_id = job_seeker_id,
                    pic_id = pic_id,
                    agency_id = GetCurrentAgencyId()
                };
                var result = ApplicationServices.UpdatePicAsync(model).Result;
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not UpdatePic because: {0}", ex.ToString());
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }
            this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);

            return RedirectToAction("Index", "Application");
        }

        //[AccessRoleChecker]
        //public ActionResult Candidate(ManageApplicationModel model)
        //{
        //    int currentPage = 1;
        //    int pageSize = SystemSettings.DefaultPageSize;

        //    if (string.IsNullOrEmpty(model.SearchExec))
        //    {
        //        model.SearchExec = "Y";
        //        if (!ModelState.IsValid)
        //        {
        //            ModelState.Clear();
        //        }
        //    }

        //    if (Request["Page"] != null)
        //    {
        //        currentPage = Utils.ConvertToInt32(Request["Page"], 1);
        //    }

        //    var filter = new ApiApplicationModel
        //    {
        //        keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
        //        page_index = currentPage,
        //        page_size = pageSize,
        //        company_id = model.company_id,
        //        status = (model.Status == null) ? -99 : Utils.ConvertToIntFromQuest(model.Status),
        //        agency_id = GetCurrentAgencyId()
        //    };

        //    var listCompanyIds = new List<int>();
        //    var listCVs = new List<int>();
        //    try
        //    {
        //        if (model.company_id > 0)
        //        {
        //            listCompanyIds.Add(model.company_id);

        //            var companyResult = CompanyServices.GetDetailAsync(new ApiCompanyModel { id = model.company_id }).Result;
        //            if (companyResult != null)
        //            {
        //                if (companyResult.value != null)
        //                {
        //                    IdentityCompany companyInfo = JsonConvert.DeserializeObject<IdentityCompany>(companyResult.value.ToString());
        //                    if (companyInfo != null)
        //                    {
        //                        if (companyInfo.agency_id != GetCurrentAgencyId())
        //                        {
        //                            return RedirectToErrorPage();
        //                        }

        //                        model.CompanyInfo = companyInfo;
        //                        if (companyInfo.LangList.HasData())
        //                        {
        //                            model.company_name = companyInfo.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.company_name).FirstOrDefault();
        //                        }

        //                        if (string.IsNullOrEmpty(model.company_name))
        //                            model.company_name = companyInfo.company_name;
        //                    }
        //                    else
        //                    {
        //                        return RedirectToErrorPage();
        //                    }
        //                }
        //            }
        //        }

        //        var result = AgencyServices.GetApplicationsAsync(filter).Result;
        //        if (result != null && result.value != null)
        //        {
        //            model.SearchResults = JsonConvert.DeserializeObject<List<IdentityApplication>>(result.value.ToString());
        //            if (model.SearchResults.HasData())
        //            {
        //                model.TotalCount = Utils.ConvertToInt32(result.total);
        //                model.CurrentPage = currentPage;
        //                model.PageSize = pageSize;
        //                //model.Industries = CommonHelpers.GetListIndustries();
        //                if (model.company_id <= 0)
        //                {
        //                    var resultListCompanyIds = model.SearchResults.Where(x => x.job_info != null).Select(x => x.job_info.company_id).ToList();
        //                    listCompanyIds.AddRange(resultListCompanyIds);
        //                }

        //                listCVs = model.SearchResults.Where(x => x.cv_id > 0).Select(x => x.cv_id).ToList();
        //            }
        //        }

        //        if (listCompanyIds.HasData())
        //        {
        //            var companyReturnApi = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCompanyIds }).Result;
        //            if (companyReturnApi != null && companyReturnApi.value != null)
        //                model.Companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companyReturnApi.value.ToString());

        //            if (model.company_id > 0 && model.Companies.HasData())
        //            {
        //                model.CompanyInfo = model.Companies.Where(x => x.id == model.company_id).FirstOrDefault();
        //            }
        //        }

        //        if (listCVs.HasData())
        //        {
        //            var cvReturnApi = CvServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCVs }).Result;
        //            if (cvReturnApi != null && cvReturnApi.value != null)
        //                model.CVs = JsonConvert.DeserializeObject<List<IdentityCv>>(cvReturnApi.value.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

        //        logger.Error("Failed to get data because: " + ex.ToString());

        //        return View(model);
        //    }

        //    return View(model);
        //}

        //public ActionResult Delete(int id)
        //{
        //    if (id <= 0)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    return PartialView("Partials/_PopupDelete", id);
        //}


        //[IsValidURLRequest]
        [AccessRoleChecker]
        public ActionResult Delete(int id, int job_seeker_id, int type, int is_ignore)
        {
            JobSeekerDeleteModel model = new JobSeekerDeleteModel();
            model.id = id;
            model.type = type;
            model.is_ignore = is_ignore;
            model.job_seeker_id = job_seeker_id;
            model.tk = SecurityHelper.GenerateSecureLink("Application", "Delete", new { id = model.id, type = model.type, is_ignore = is_ignore,job_seeker_id = job_seeker_id });
            try
            {
                var jResult = JobSeekerServices.GetCounterForDeletionAsync(
                        new ApiJobSeekerDeleteModel { agency_id = GetCurrentAgencyId(), id = job_seeker_id }
                        ).Result;
                model.Counter = jResult.ConvertData<IdentityJobSeekerCounter>();
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display DeleteApplication because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_Delete", model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDelete()
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;

            try
            {
                var id = Request["id"];
                var type = Utils.ConvertToInt32(Request["type"]);
                var tk = Request["tk"];
                var is_ignore = Utils.ConvertToInt32(Request["is_ignore"]);
                var job_seeker_id = Utils.ConvertToInt32(Request["job_seeker_id"]);
                var tkSys = SecurityHelper.GenerateSecureLink("Application", "Delete", new { id = id, type = type, is_ignore = is_ignore,job_seeker_id = job_seeker_id });
                if (tk != tkSys)
                {
                    return Json(new { success = false, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_NOTIFICATION });
                }

                var apiModel = new ApiJobSeekerDeleteModel();
                apiModel.ids = id;
                apiModel.type = type;
                apiModel.agency_id = GetCurrentAgencyId();

                if (!string.IsNullOrEmpty(apiModel.ids))
                {
                    var apiReturned = await ApplicationServices.DeletesAsync(apiModel);
                    if (apiReturned != null)
                    {
                        if (apiReturned.status == (int)HttpStatusCode.OK)
                        {
                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                            {
                                message = apiReturned.error.message;

                                return Json(new { success = false, html = htmlReturn, message = message, title = ManagerResource.LB_NOTIFICATION });
                            }
                            else
                            {

                                return Json(new { success = true, html = htmlReturn, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload();" });
                            }
                        }
                    }
                    if (is_ignore == 1)
                    {
                        var apiIgnoreModel = new ApiApplicationIgnoreModel();
                        apiIgnoreModel.id = Utils.ConvertToInt32(id);
                        apiIgnoreModel.cv_id = 0;
                        apiIgnoreModel.agency_id = GetCurrentAgencyId();
                        apiIgnoreModel.staff_id = GetCurrentStaffId();

                        var result = ApplicationServices.IgnoreAsync(apiIgnoreModel).Result;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec Delete because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }

            return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }

        //Show popup confirm        
        [AccessRoleChecker]
        public ActionResult Ignore(int? id, int? cv_id)
        {
            var model = new ApplicationIgnoreModel();
            model.id = Utils.ConvertToIntFromQuest(id);
            model.cv_id = Utils.ConvertToIntFromQuest(cv_id);

            return PartialView("Partials/_Ignore", model);
        }

        [HttpPost, ActionName("Ignore")]
        [ValidateAntiForgeryToken]
        public ActionResult Ignore_Confirm(ApiApplicationIgnoreModel model)
        {
            var strError = string.Empty;
            if (model.id <= 0 || model.cv_id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var apiModel = new ApiApplicationIgnoreModel();
                apiModel.id = model.id;
                apiModel.cv_id = model.cv_id;
                apiModel.agency_id = GetCurrentAgencyId();
                apiModel.staff_id = GetCurrentStaffId();

                var result = ApplicationServices.IgnoreAsync(apiModel).Result;
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Ignore Application because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_APPLICATION_IGNORE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload(); " });
        }

        //Show popup confirm        
        [AccessRoleChecker]
        public ActionResult Accept(int? id, int? cv_id, int pic_id)
        {
            var model = new ApplicationAcceptModel();
            model.id = Utils.ConvertToIntFromQuest(id);
            model.cv_id = Utils.ConvertToIntFromQuest(cv_id);
            model.pic_id = pic_id;
            model.ListStaffs = CommonHelpers.GetListUser(GetCurrentAgencyId());

            return PartialView("Partials/_Accept", model);
        }

        [HttpPost, ActionName("Accept")]
        [ValidateAntiForgeryToken]
        public ActionResult Accept_Confirm(ApiApplicationAcceptModel model)
        {
            var strError = string.Empty;
            if (model.id <= 0 || model.cv_id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var apiModel = new ApiApplicationAcceptModel();
                apiModel.id = model.id;
                apiModel.cv_id = model.cv_id;
                apiModel.pic_id = model.pic_id;
                apiModel.agency_id = GetCurrentAgencyId();
                apiModel.staff_id = GetCurrentStaffId();

                var result = ApplicationServices.AcceptAsync(apiModel).Result;
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Accept Application because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_APPLICATION_ACCEPT_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload(); " });
        }

        [PreventCrossOrigin]
        [IsValidURLRequest]
        public ActionResult SendEmail()
        {
            ManageEmailSendingModel model = new ManageEmailSendingModel();
            try
            {
                model.job_seeker_id = Utils.ConvertToInt32(Request["job_seeker_id"]);
                model.receiver = Request["receiver"] != null ? Request["receiver"].ToString() : string.Empty;
                model.is_online = Utils.ConvertToInt32(Request["is_online"]);
                model.controller_name = "Application";
                model.action_name = "SendEmail";
                model.tk = Request["tk"];
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get MailToMyJobSeeker because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("../MyEmail/Sub/_PopupComposer", model);
        }

        //[AccessRoleChecker]
        //public ActionResult InvitationHistory(ManageInvitationModel model)
        //{
        //    int currentPage = 1;
        //    int pageSize = SystemSettings.DefaultPageSize;

        //    if (string.IsNullOrEmpty(model.SearchExec))
        //    {
        //        model.SearchExec = "Y";
        //        if (!ModelState.IsValid)
        //        {
        //            ModelState.Clear();
        //        }
        //    }

        //    if (Request["Page"] != null)
        //    {
        //        currentPage = Utils.ConvertToInt32(Request["Page"], 1);
        //    }

        //    var filter = new ApiCvInvitationModel
        //    {
        //        keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
        //        page_index = currentPage,
        //        page_size = pageSize,
        //        company_id = model.company_id,
        //        status = (model.Status == null) ? -99 : Utils.ConvertToIntFromQuest(model.Status),
        //        agency_id = GetCurrentAgencyId()
        //    };

        //    var listCompanyIds = new List<int>();
        //    var listCVs = new List<int>();
        //    try
        //    {
        //        if (model.company_id > 0)
        //        {
        //            listCompanyIds.Add(model.company_id);

        //            var companyResult = CompanyServices.GetDetailAsync(new ApiCompanyModel { id = model.company_id }).Result;
        //            if (companyResult != null)
        //            {
        //                if (companyResult.value != null)
        //                {
        //                    IdentityCompany companyInfo = JsonConvert.DeserializeObject<IdentityCompany>(companyResult.value.ToString());
        //                    if (companyInfo != null)
        //                    {
        //                        if (companyInfo.agency_id != GetCurrentAgencyId())
        //                        {
        //                            return RedirectToErrorPage();
        //                        }

        //                        model.CompanyInfo = companyInfo;
        //                        if (companyInfo.LangList.HasData())
        //                        {
        //                            model.company_name = companyInfo.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.company_name).FirstOrDefault();
        //                        }

        //                        if (string.IsNullOrEmpty(model.company_name))
        //                            model.company_name = companyInfo.company_name;
        //                    }
        //                    else
        //                    {
        //                        return RedirectToErrorPage();
        //                    }
        //                }
        //            }
        //        }

        //        var result = AgencyServices.GetInvitationsAsync(filter).Result;
        //        if (result != null && result.value != null)
        //        {
        //            model.SearchResults = JsonConvert.DeserializeObject<List<IdentityInvitation>>(result.value.ToString());
        //            if (model.SearchResults.HasData())
        //            {
        //                model.TotalCount = Utils.ConvertToInt32(result.total);
        //                model.CurrentPage = currentPage;
        //                model.PageSize = pageSize;
        //                //model.Industries = CommonHelpers.GetListIndustries();
        //                if (model.company_id <= 0)
        //                {
        //                    var resultListCompanyIds = model.SearchResults.Where(x => x.job_info != null).Select(x => x.job_info.company_id).ToList();
        //                    listCompanyIds.AddRange(resultListCompanyIds);
        //                }

        //                listCVs = model.SearchResults.Where(x => x.cv_id > 0).Select(x => x.cv_id).ToList();
        //            }
        //        }

        //        if (listCompanyIds.HasData())
        //        {
        //            var companyReturnApi = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCompanyIds }).Result;
        //            if (companyReturnApi != null && companyReturnApi.value != null)
        //                model.Companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companyReturnApi.value.ToString());

        //            if (model.company_id > 0 && model.Companies.HasData())
        //            {
        //                model.CompanyInfo = model.Companies.Where(x => x.id == model.company_id).FirstOrDefault();
        //            }
        //        }

        //        if (listCVs.HasData())
        //        {
        //            var cvReturnApi = CvServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCVs }).Result;
        //            if (cvReturnApi != null && cvReturnApi.value != null)
        //                model.CVs = JsonConvert.DeserializeObject<List<IdentityCv>>(cvReturnApi.value.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

        //        logger.Error("Failed to get data because: " + ex.ToString());

        //        return View(model);
        //    }

        //    return View(model);
        //}
        #region Helpers

        private JobEditModel ParseDataToForm(IdentityJob info)
        {
            var model = new JobEditModel();
            model.id = info.id;
            model.company_id = info.company_id;
            model.quantity = Utils.ConvertZeroToEmpty(info.quantity);
            model.age_min = Utils.ConvertZeroToEmpty(info.age_min);
            model.age_max = Utils.ConvertZeroToEmpty(info.age_max);
            model.salary_min = Utils.ConvertZeroToEmpty(info.salary_min);
            model.salary_max = Utils.ConvertZeroToEmpty(info.salary_max);
            model.salary_type_id = info.salary_type_id;
            model.work_start_time = info.work_start_time.TimeSpanQuestToString();
            model.work_end_time = info.work_end_time.TimeSpanQuestToString();
            model.probation_duration = Utils.ConvertZeroToEmpty(info.probation_duration);
            model.employment_type_id = info.employment_type_id;
            model.flexible_time = info.flexible_time;
            model.language_level = info.language_level;
            model.work_experience_doc_required = info.work_experience_doc_required;
            model.duration = info.duration;
            model.view_company = info.view_company;
            model.qualification_id = info.qualification_id;
            model.japanese_level_number = info.japanese_level_number;
            model.sub_field_id = info.sub_field_id;
            model.sub_industry_id = info.sub_industry_id;

            if (info.Job_translations.HasData())
            {
                var myLang = info.Job_translations.Where(x => x.language_code == _currentLanguage).FirstOrDefault();
                if (myLang != null)
                {
                    model.title = myLang.title;
                    model.subsidy = myLang.subsidy;
                    model.paid_holiday = myLang.paid_holiday;
                    model.bonus = myLang.bonus;
                    model.certificate = myLang.certificate;
                    model.work_content = myLang.work_content;
                    model.requirement = myLang.requirement;
                    model.plus = myLang.plus;
                    model.welfare = myLang.welfare;
                    model.training = myLang.training;
                    model.recruitment_procedure = myLang.recruitment_procedure;
                    model.remark = myLang.remark;
                }
            }

            if (info.Tags.HasData())
            {
                model.TagIds = info.Tags.Select(x => x.id).ToList();
                model.SelectedTags = info.Tags;
            }

            return model;
        }

        private ApiJobUpdateModel ExtractFormData(JobEditModel model)
        {
            var apiModel = new ApiJobUpdateModel();
            apiModel.job = new ApiJobUpdateInfoModel();

            apiModel.job.id = model.id;
            apiModel.job.company_id = model.company_id;
            apiModel.job.quantity = Utils.ConvertToInt32(model.quantity);
            apiModel.job.age_min = Utils.ConvertToInt32(model.age_min);
            apiModel.job.age_max = Utils.ConvertToInt32( model.age_max);
            apiModel.job.salary_min = Utils.ConvertToInt32(model.salary_min);
            apiModel.job.salary_max = Utils.ConvertToInt32(model.salary_max);
            apiModel.job.salary_type_id = model.salary_type_id;
            apiModel.job.work_start_time = model.work_start_time;
            apiModel.job.work_end_time = model.work_end_time;
            apiModel.job.probation_duration = Utils.ConvertToInt32(model.probation_duration);
            apiModel.job.employment_type_id = model.employment_type_id;
            apiModel.job.flexible_time = model.flexible_time;
            apiModel.job.language_level = model.language_level;
            apiModel.job.work_experience_doc_required = model.work_experience_doc_required;
            apiModel.job.duration = model.duration;
            apiModel.job.view_company = model.view_company;
            apiModel.job.qualification_id = model.qualification_id;
            apiModel.job.japanese_level_number = model.japanese_level_number;
            apiModel.job.sub_field_id = model.sub_field_id;
            apiModel.job.sub_industry_id = model.sub_industry_id;

            if (model.Addresses.HasData())
            {
                apiModel.job.Addresses = new List<ApiJobUpdateAddressModel>();
                foreach (var item in model.Addresses)
                {
                    var addressInfo = new ApiJobUpdateAddressModel();
                    addressInfo.id = item.id;
                    addressInfo.country_id = item.country_id;
                    addressInfo.region_id = item.region_id;
                    addressInfo.prefecture_id = item.prefecture_id;
                    addressInfo.city_id = item.city_id;
                    addressInfo.furigana = item.furigana;
                    addressInfo.detail = item.detail;

                    if (item.StationsIds.HasData())
                    {
                        addressInfo.Stations = new List<ApiJobUpdateStationModel>();
                        foreach (var st in item.StationsIds)
                        {
                            var station = new ApiJobUpdateStationModel();
                            station.id = st;

                            addressInfo.Stations.Add(station);
                        }
                    }

                    apiModel.job.Addresses.Add(addressInfo);
                }
            }

            apiModel.job.Job_translations = new List<ApiJobUpdateTranslationModel>();
            var tranInfo = new ApiJobUpdateTranslationModel();

            tranInfo.title = model.title;
            tranInfo.subsidy = HtmlHelpers.RemoveScriptTags(model.subsidy);
            tranInfo.paid_holiday = HtmlHelpers.RemoveScriptTags(model.paid_holiday);
            tranInfo.bonus = HtmlHelpers.RemoveScriptTags(model.bonus);
            tranInfo.certificate = HtmlHelpers.RemoveScriptTags(model.certificate);
            tranInfo.work_content = HtmlHelpers.RemoveScriptTags(model.work_content);
            tranInfo.requirement = HtmlHelpers.RemoveScriptTags(model.requirement);
            tranInfo.plus = HtmlHelpers.RemoveScriptTags(model.plus);
            tranInfo.welfare = HtmlHelpers.RemoveScriptTags(model.welfare);
            tranInfo.training = HtmlHelpers.RemoveScriptTags(model.training);
            tranInfo.recruitment_procedure = HtmlHelpers.RemoveScriptTags(model.recruitment_procedure);
            tranInfo.remark = HtmlHelpers.RemoveScriptTags(model.remark);
            tranInfo.language_code = _currentLanguage;

            apiModel.job.Job_translations.Add(tranInfo);

            //if (model.Sub_fields.HasData())
            //{
            //    apiModel.job.Sub_fields = new List<ApiJobUpdateSubFieldModel>();
            //    foreach (var item in model.Sub_fields)
            //    {
            //        var subFieldInfo = new ApiJobUpdateSubFieldModel();
            //        subFieldInfo.id = item;

            //        apiModel.job.Sub_fields.Add(subFieldInfo);
            //    }
            //}

            if (model.TagIds.HasData())
            {
                apiModel.job.Tags = new List<ApiJobUpdateTagModel>();
                foreach (var item in model.TagIds)
                {
                    var tagInfo = new ApiJobUpdateTagModel();

                    tagInfo.id = item;
                    //tagInfo.tag = item.tag;

                    apiModel.job.Tags.Add(tagInfo);
                }
            }

            return apiModel;
        }

        #endregion

    }
}