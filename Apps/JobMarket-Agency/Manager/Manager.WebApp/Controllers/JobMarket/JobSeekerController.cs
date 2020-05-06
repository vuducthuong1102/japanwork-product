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
    public class JobSeekerController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<JobSeekerController>();
        private readonly string DATE_FORMAT = "dd-MM-yyyy";
        private readonly string MONTH_YEAR_FORMAT = "MM-yyyy";

        public ActionResult Index(ManageJobSeekerModel model)
        {
            model = GetDefaultFilterModel(model);
            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.gender = -1;
                model.major_id = -1;
                model.SearchExec = "Y";                
                if (!ModelState.IsValid)
                    ModelState.Clear();
            }
            model.type_job_seeker = 1;
            try
            {
                var filter = new ApiJobSeekerByPageModel
                {
                    keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                    page_index = model.CurrentPage,
                    page_size = model.PageSize,
                    status = -1,
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

                var result = A_JobSeekerServices.GetListByPageAsync(filter).Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJobSeeker>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        var listPrefectureIds = new List<int>();
                        foreach (var item in model.SearchResults)
                        {
                            if (item.Addresses.HasData())
                            {
                                var currentPrefectId = item.Addresses[0].prefecture_id;
                                if (currentPrefectId > 0)
                                    listPrefectureIds.Add(currentPrefectId);
                            }
                        }
                        if (listPrefectureIds.HasData())
                        {
                            model.Prefectures = CommonHelpers.GetListPrefectures(listPrefectureIds);
                        }
                        model.TotalCount = Utils.ConvertToInt32(result.total);
                    }
                }
                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                model.Majors = CommonHelpers.GetListMajors();
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                model.Qualifications = CommonHelpers.GetListQualifications();
                model.Visas = CommonHelpers.GetListVisas();

                var country_id = 81;
                var apiReturned = RegionServices.GetListAsync(country_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        model.Regions = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiReturned.value.ToString());
                    }
                }

                //if (model.employment_type_id == 0 && model.EmploymentTypes.HasData())
                //{
                //    model.employment_type_id = model.EmploymentTypes[0].id;
                //}
                model.Fields = CommonHelpers.GetListFieldsByEmploymentType(model.employment_type_id);
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
            ManageJobSeekerModel model = new ManageJobSeekerModel();
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
                var country_id = 81;
                var apiReturned = RegionServices.GetListAsync(country_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        model.Regions = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiReturned.value.ToString());
                    }
                }
                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                model.Majors = CommonHelpers.GetListMajors();
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                model.Qualifications = CommonHelpers.GetListQualifications();
                model.Visas = CommonHelpers.GetListVisas();

                //if (model.employment_type_id == 0 && model.EmploymentTypes.HasData())
                //{
                //    model.employment_type_id = model.EmploymentTypes[0].id;
                //}
                model.Fields = CommonHelpers.GetListFieldsByEmploymentType(model.employment_type_id);
            }
            catch (Exception ex)
            {
                logger.Error("Failed to get PopupSearch: " + ex.ToString());
            }

            return PartialView("Partials/_PopupSearch", model);
        }

        public ActionResult ShowNote(int job_seeker_id, int type_job_seeker)
        {
            ManageJobSeekerNoteModel model = new ManageJobSeekerNoteModel();
            model.job_seeker_id = job_seeker_id;
            model.type_job_seeker = type_job_seeker;
            model.CurrentPage = 1;
            model.PageSize = SystemSettings.DefaultPageSize;
            model.staff_id = GetCurrentStaffId();
            var apiModel = new ApiJobSeekerNoteModel()
            {
                job_seeker_id = model.job_seeker_id,
                type_job_seeker = model.type_job_seeker,
                page_index = model.CurrentPage,
                page_size = model.PageSize,
                staff_id = model.staff_id,
                agency_id = GetCurrentAgencyId()
            };
            try
            {
                var resultList = JobSeekerNoteServices.GetByPageAsync(apiModel).Result;
                if (!string.IsNullOrEmpty(resultList.value.ToString()))
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJobSeekerNote>>(resultList.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        model.TotalCount = Utils.ConvertToInt32(resultList.total);
                    }
                }
                model.ListStaffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
            }
            return PartialView("../JobSeekerNote/Partials/_Detail", model);
        }
        public ActionResult NoteSearch(ManageJobSeekerNoteModel model)
        {
            //ManageJobSeekerNoteModel model = new ManageJobSeekerNoteModel();
            //model.job_seeker_id = job_seeker_id;
            //model.type_job_seeker = type_job_seeker;
            //model.CurrentPage = currentPage;
            model.PageSize = SystemSettings.DefaultPageSize;
            model.staff_id = GetCurrentStaffId();
            var apiModel = new ApiJobSeekerNoteModel()
            {
                job_seeker_id = model.job_seeker_id,
                type_job_seeker = model.type_job_seeker,
                page_index = model.CurrentPage,
                page_size = model.PageSize,
                staff_id = model.staff_id,
                agency_id = GetCurrentAgencyId()
            };
            try
            {
                var resultList = JobSeekerNoteServices.GetByPageAsync(apiModel).Result;
                if (!string.IsNullOrEmpty(resultList.value.ToString()))
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJobSeekerNote>>(resultList.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        model.TotalCount = Utils.ConvertToInt32(resultList.total);
                    }
                }
                model.ListStaffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
            }
            return PartialView("../JobSeekerNote/Partials/_DetailItem", model);
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
                model.controller_name = "JobSeeker";
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

        [PreventCrossOrigin]
        [HttpPost]
        public ActionResult BatchSendEmail(ManageEmailBatchSendingModel model)
        {
            ModelState.Clear();

            model.is_online = 0;
            model.target_type = (int)EnumEmailTargetType.JobSeeker;

            return PartialView("../MyEmail/Sub/_PopUpComposerBatch", model);
        }

        [PreventCrossOrigin]
        public ActionResult BatchSendEmail_Old()
        {
            ManageEmailBatchSendingModel model = new ManageEmailBatchSendingModel();
            try
            {
                var ids = Request["ids"] != null ? Request["ids"].ToString() : string.Empty;
                List<int> listIds = null;
                model.SelectedObjects = new List<ManageEmailObjectInfoModel>();

                if (!string.IsNullOrEmpty(ids))
                {
                    listIds = ids.Split(',').Select(Int32.Parse).ToList();
                }

                if (listIds.HasData())
                {
                    foreach (var item in listIds)
                    {
                        var r = new ManageEmailObjectInfoModel();
                        r.object_id = item;
                        r.email = "xxxx";

                        model.SelectedObjects.Add(r);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get BatchSendEmail because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("../MyEmail/Sub/_PopUpComposerBatch", model);
        }

        public ActionResult CreateNote(int job_seeker_id, int type_job_seeker)
        {
            var model = new ManageJobSeekerNoteModel();
            try
            {
                model.job_seeker_id = job_seeker_id;
                model.type_job_seeker = type_job_seeker;
                model.controller_name = base.ControllerContext.RouteData.Values["controller"].ToString();
                model.action_name = base.ControllerContext.RouteData.Values["action"].ToString();
                model.tk = base.Request["tk"].ToString();
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display CreateNote because: {0}", ex.ToString()));
            }
            return PartialView("../JobSeekerNote/Partials/_CreateNote", model);
        }

        public ActionResult EditNote(int id)
        {
            ModelState.Clear();
            var model = new ManageJobSeekerNoteModel();
            try
            {
                if (id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var resultIvP = JobSeekerNoteServices.GetDetailAsync(id).Result;
                if (resultIvP != null && resultIvP.value != null)
                {
                    var info = JsonConvert.DeserializeObject<IdentityJobSeekerNote>(resultIvP.value.ToString());
                    model = ParseDataNoteToForm(info);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditNote [{0}] because: {1}", id, ex.ToString()));
            }

            return PartialView("../JobSeekerNote/Partials/_EditNote", model);
        }


        public ActionResult DeleteNote(int id, int job_seeker_id, int type_job_seeker)
        {
            JobSeekerNoteDeleteModel model = new JobSeekerNoteDeleteModel
            {
                id = id,
                job_seeker_id = job_seeker_id,
                type_job_seeker = type_job_seeker
            };
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return this.PartialView("../JobSeekerNote/Partials/_DeleteNote", model);
        }

        private JobSeekerUpdateProfileModel PreparingMyProfileViewModel(int jobSeekerId, int is_detail = 0)
        {
            var model = new JobSeekerUpdateProfileModel();
            try
            {
                if (jobSeekerId > 0)
                {
                    var apiModel = new ApiJobSeekerModel();
                    apiModel.id = jobSeekerId;

                    var apiReturned = A_JobSeekerServices.GetDetailForUpdateAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityJobSeeker info = JsonConvert.DeserializeObject<IdentityJobSeeker>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model = ParseDataToForm(info);

                                if (info.Addresses.HasData())
                                {
                                    model.address = info.Addresses.Where(x => x.is_contact_address == true).FirstOrDefault();
                                    model.address_contact = info.Addresses.Where(x => x.is_contact_address == false).FirstOrDefault();

                                    if (model.address != null)
                                    {
                                        if (model.address.country_id == (int)EnumCountry.Japan)
                                        {
                                            if (model.address.train_line_id > 0)
                                            {
                                                var trainLineApiReturn = TrainLineServices.GetDetailAsync(model.address.train_line_id).Result;
                                                if (trainLineApiReturn != null)
                                                {
                                                    if (trainLineApiReturn.value != null)
                                                        model.train_line_info = JsonConvert.DeserializeObject<IdentityTrainLine>(trainLineApiReturn.value.ToString());
                                                }
                                            }

                                            if (model.address.station_id > 0)
                                            {
                                                var stationApiReturn = StationServices.GetDetailAsync(model.address.station_id).Result;
                                                if (stationApiReturn != null)
                                                {
                                                    if (stationApiReturn.value != null)
                                                        model.station_info = JsonConvert.DeserializeObject<IdentityStation>(stationApiReturn.value.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var defaultYear = DateTime.Now.Year - 19;
                    model.birthday = "01-01-" + defaultYear;
                }

                model.Countries = CommonHelpers.GetListCountries();
                model.Regions = CommonHelpers.GetListRegions();
                model.Qualifications = CommonHelpers.GetListQualifications();
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display PreparingMyProfileViewModel because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return model;
        }

        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> MyProfilePartial(int? id, int? exclude_ct_add)
        {
            JobSeekerUpdateProfileModel model = null;

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }

            try
            {
                var jobSeekerId = Utils.ConvertToIntFromQuest(id);
                model = PreparingMyProfileViewModel(jobSeekerId);
                model.WishModel = new List<JobSeekerWishModel>();
                if (jobSeekerId > 0)
                {
                    var apiReturned = A_JobSeekerWishServices.GetDetailAsync(jobSeekerId).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            List<IdentityJobSeekerWish> info = JsonConvert.DeserializeObject<List<IdentityJobSeekerWish>>(apiReturned.value.ToString());
                            if (info.HasData())
                            {
                                foreach (var record in info)
                                {
                                    var item = ExtractJobSeekerWishData(record);
                                    model.WishModel.Add(item);
                                }
                            }
                        }

                        //if (model.WishModel != null)
                        //{
                        //    if (model.WishModel.start_date == null) model.WishModel.start_date = "";
                        //}
                    }
                }

                model.SelectedWishModel = "";

                if (model.WishModel.HasData())
                {
                    var wishIdList = new List<int>();
                    foreach (var item in model.WishModel)
                    {
                        if (item.id > 0)
                        {
                            wishIdList.Add(item.id);
                        }
                    }
                    model.SelectedWishModel = string.Join(",", wishIdList);
                }
                else
                {
                    model.WishModel.Add(new JobSeekerWishModel());
                }

                var country_id = 81;
                var apiRegion = RegionServices.GetListAsync(country_id).Result;
                if (apiRegion != null)
                {
                    if (apiRegion.value != null)
                    {
                        model.Regions = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiRegion.value.ToString());
                    }
                }
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();

                var employment_type = 0;
                if (model.EmploymentTypes.HasData())
                {
                    employment_type = model.EmploymentTypes[0].id;
                }
                model.Fields = CommonHelpers.GetListFieldsByEmploymentType(employment_type);

                model.agency_id = GetCurrentAgencyId();
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                if (exclude_ct_add != null)
                    model.exclude_ct_add = Utils.ConvertToInt32(exclude_ct_add);

                await Task.FromResult(model);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display MyProfilePartial Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_Profile", model);
        }

        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> MyProfilePartialDetail(int? id, int? exclude_ct_add)
        {
            JobSeekerUpdateProfileModel model = null;

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }

            try
            {
                var jobSeekerId = Utils.ConvertToIntFromQuest(id);
                model = PreparingMyProfileViewModel(jobSeekerId);
                model.agency_id = GetCurrentAgencyId();
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                if (exclude_ct_add != null)
                    model.exclude_ct_add = Utils.ConvertToInt32(exclude_ct_add);

                await Task.FromResult(model);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display MyProfilePartial Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_ProfileDetail", model);
        }

        [AccessRoleChecker]
        public ActionResult Create()
        {
            JobSeekerUpdateProfileModel model = new JobSeekerUpdateProfileModel();
            try
            {
                //model = PreparingMyProfileViewModel(0);
                model.agency_id = GetCurrentAgencyId();
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display Create Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return View(model);
        }
        [AccessRoleChecker]
        public ActionResult AssignmentWork(int id, int type_job_seeker)
        {
            InterviewProcessInsertModel model = new InterviewProcessInsertModel();
            model.type_job_seeker = type_job_seeker;
            model.agency_id = GetCurrentAgencyId();
            model.job_seeker_id = id;
            try
            {
                var apiModel = new ApiJobSeekerModel();
                apiModel.id = id;
                if (type_job_seeker == 1)
                {
                    var apiJobSeekerReturned = A_JobSeekerServices.GetDetailForUpdateAsync(apiModel).Result;
                    if (apiJobSeekerReturned != null)
                    {
                        if (apiJobSeekerReturned.value != null)
                        {
                            IdentityJobSeeker info = JsonConvert.DeserializeObject<IdentityJobSeeker>(apiJobSeekerReturned.value.ToString());
                            model.full_name = info.fullname;
                        }
                    }
                }
                else
                {
                    var apiJobSeekerReturned = JobSeekerServices.GetDetailForUpdateAsync(apiModel).Result;
                    if (apiJobSeekerReturned != null)
                    {
                        if (apiJobSeekerReturned.value != null)
                        {
                            IdentityJobSeeker info = JsonConvert.DeserializeObject<IdentityJobSeeker>(apiJobSeekerReturned.value.ToString());
                            model.full_name = info.fullname;
                        }
                    }
                }

                model.PageSize = SystemSettings.DefaultPageSize;
                if (model.CurrentPage == 0) model.CurrentPage = 1;
                var apiJobModel = new ApiJobModel()
                {
                    type_job_seeker = model.type_job_seeker,
                    page_size = model.PageSize,
                    agency_id = GetCurrentAgencyId(),
                    company_id = model.company_id,
                    job_seeker_id = model.job_seeker_id,
                    keyword = model.Keyword,
                    page_index = model.CurrentPage
                };

                var listStatus = new List<int>();
                listStatus.Add((int)EnumJobStatus.Published);
                if (model.type_job_seeker == 1)
                {
                    listStatus.Add((int)EnumJobStatus.Draft);
                    listStatus.Add((int)EnumJobStatus.Saved);

                }
                apiJobModel.list_status = string.Join(",", listStatus);

                var apiReturned = JobServices.GetListAssignmentWorkByPageAsync(apiJobModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJob>>(apiReturned.value.ToString());
                        model.TotalCount = Utils.ConvertToInt32(apiReturned.total);
                        if (model.SearchResults.HasData())
                        {
                            List<int> listCompanyIds = model.SearchResults.Select(x => x.company_id).ToList();
                            if (listCompanyIds.HasData())
                            {
                                var companyReturnApi = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCompanyIds }).Result;
                                if (companyReturnApi != null && companyReturnApi.value != null)
                                    model.Companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companyReturnApi.value.ToString());
                            }
                        }
                    }
                }
                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.SubFields = CommonHelpers.GetListSubFields();

                var apiCompanymodel = new ApiCompanyModel
                {
                    agency_id = GetCurrentStaffId(),
                    page_index = 1,
                    page_size = 1000
                };
                var listDataCompany = AgencyServices.GetCompaniesAsync(apiCompanymodel).Result;
                List<IdentityCompany> listCompany = JsonConvert.DeserializeObject<List<IdentityCompany>>(listDataCompany.value.ToString());
                var listResult = AgencyCompanyServices.GetListCompanysByIdsAsync(GetCurrentStaffId()).Result;
                if (listResult != null && listResult.value != null)
                {
                    var listSelected = JsonConvert.DeserializeObject<List<int>>(listResult.value.ToString());
                    List<SelectListItem> listItem = new List<SelectListItem>();
                    if (listCompany.HasData())
                    {
                        foreach (var record in listCompany)
                        {
                            var item = new SelectListItem();
                            item.Value = record.id.ToString();
                            var company_name = "";
                            if (record.LangList.HasData())
                            {
                                var companyLang = record.LangList.FirstOrDefault(s => s.language_code == GetCurrentLanguageOrDefault());
                                if (companyLang != null) company_name = companyLang.company_name;
                            }
                            if (string.IsNullOrEmpty(company_name))
                            {
                                company_name = record.company_name;
                            }

                            item.Text = company_name;
                            model.ListCompanies.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display AssignmentWork because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_AssignmentWork", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignmentWork_Post(InterviewProcessInsertModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            try
            {
                if (model.type_job_seeker == 1)
                {
                    //Begin update
                    var apiModel = ExtractCandidateFormData(model);
                    apiModel.job_seeker_id = model.job_seeker_id;
                    apiModel.list_job_ids = string.Join(",", model.list_ids);
                    var apiReturned = CandidateServices.InsertMultiJobAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.status == (int)HttpStatusCode.OK)
                        {
                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                            {
                                message = apiReturned.error.message;

                                return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(apiReturned.message))
                                {
                                    return Json(new { success = true, message = apiReturned.message, title = ManagerResource.LB_NOTIFICATION });
                                }
                                else
                                {
                                    return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
                                }
                            }
                        }
                    }
                }
                else
                {
                    var apiModel = new ApiCvInvitationModel();
                    apiModel.agency_id = GetCurrentAgencyId();
                    apiModel.staff_id = GetCurrentStaffId();
                    apiModel.job_ids = string.Join(",", model.list_ids);
                    apiModel.job_seeker_id = model.job_seeker_id;
                    var apiResult = JobServices.ApplicationInviteMultiJobsAsync(apiModel).Result;
                    if (apiResult != null)
                    {
                        if (apiResult.status == (int)HttpStatusCode.OK)
                        {
                            if (apiResult.error != null && !string.IsNullOrEmpty(apiResult.error.error_code))
                            {
                                return Json(new { success = false, message = apiResult.error.message, title = ManagerResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(apiResult.message))
                                {
                                    return Json(new { success = true, message = apiResult.message, title = ManagerResource.LB_NOTIFICATION, clientcallback = "JobChoosenJobSeekerSearch();" });
                                }
                                else
                                {
                                    return Json(new { success = true, message = ManagerResource.LB_APPLICATION_INVITATION_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "JobChoosenJobSeekerSearch();" });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec AssignmentWork because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AssignmentWork_ListPost(int job_id, int type_job_seeker, string[] list_ids)
        {
            var message = string.Empty;
            var isSuccess = false;
            try
            {
                var list_job_seeker_ids = string.Join(",", list_ids);
                if (type_job_seeker == 1)
                {
                    //Begin update
                    var apiModel = new ApiCandidateInsertModel();
                    apiModel.type = type_job_seeker;
                    apiModel.job_id = job_id;
                    apiModel.agency_id = GetCurrentAgencyId();
                    apiModel.staff_id = GetCurrentStaffId();
                    apiModel.list_job_seeker_ids = list_job_seeker_ids;
                    var apiReturned = CandidateServices.InsertAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.status == (int)HttpStatusCode.OK)
                        {
                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                            {
                                message = apiReturned.error.message;

                                return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(apiReturned.message))
                                {
                                    return Json(new { success = true, message = apiReturned.message, title = ManagerResource.LB_NOTIFICATION, clientcallback = "JobChoosenJobSeekerSearch();" });
                                }
                                else
                                {
                                    return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "JobChoosenJobSeekerSearch();" });
                                }
                            }
                        }
                    }
                }
                else
                {
                    var apiModel = new ApiCvInvitationModel();
                    apiModel.agency_id = GetCurrentAgencyId();
                    apiModel.staff_id = GetCurrentStaffId();
                    apiModel.job_id = job_id;
                    if (list_ids.HasData())
                    {
                        apiModel.JobSeekers = new List<ApiCvInvitationItemModel>();

                        foreach (var item in list_ids)
                        {
                            var inviteInfo = new ApiCvInvitationItemModel();
                            inviteInfo.job_seeker_id = Utils.ConvertToInt32(item);

                            apiModel.JobSeekers.Add(inviteInfo);
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = ManagerResource.ERROR_ALEAST_ONE_APPLICANT_SELECT, title = ManagerResource.LB_NOTIFICATION });
                    }

                    var apiResult = JobServices.ApplicationInviteJobSeekerAsync(apiModel).Result;
                    if (apiResult != null)
                    {
                        if (apiResult.status == (int)HttpStatusCode.OK)
                        {
                            if (apiResult.error != null && !string.IsNullOrEmpty(apiResult.error.error_code))
                            {
                                return Json(new { success = false, message = apiResult.error.message, title = ManagerResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(apiResult.message))
                                {
                                    return Json(new { success = true, message = apiResult.message, title = ManagerResource.LB_NOTIFICATION, clientcallback = "JobChoosenJobSeekerSearch();" });
                                }
                                else
                                {
                                    return Json(new { success = true, message = ManagerResource.LB_APPLICATION_INVITATION_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "JobChoosenJobSeekerSearch();" });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec AssignmentWork because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }


        [HttpPost]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfile(JobSeekerUpdateProfileModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = ExtractFormData(model);
                if (model.image_file_upload != null)
                {
                    var apiUploadReturned = await A_JobSeekerServices.UploadImageAsync(apiModel, model.image_file_upload);
                    if (apiUploadReturned != null)
                    {
                        if (apiUploadReturned.value != null)
                        {
                            List<FileUploadResponseModel> images = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiUploadReturned.value.ToString());
                            if (images.HasData())
                            {
                                apiModel.image = images[0].Path;
                            }
                        }
                    }
                }

                var apiReturned = await A_JobSeekerServices.UpdateProfileAsync(apiModel);
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            var msg = (apiModel.id > 0) ? ManagerResource.LB_UPDATE_SUCCESS : ManagerResource.LB_INSERT_SUCCESS;

                            if (apiReturned.value != null)
                            {
                                var returnId = Utils.ConvertToInt32(apiReturned.value.ToString());
                                var urlEditWithoutProfile = SecurityHelper.GenerateSecureLink("JobSeeker", "Update", new { id = returnId, hide_pf = 1, next_step = 1 });
                                var urlEditProfile = SecurityHelper.GenerateSecureLink("JobSeeker", "Update", new { id = returnId, hide_pf = 0, next_step = 1 });
                                model.job_seeker_id = returnId;

                                var apiWishModel = ExtractWishFormData(model);
                                var selectedWishList = new string[] { };

                                if (!string.IsNullOrEmpty(model.SelectedWishModel))
                                {
                                    selectedWishList = model.SelectedWishModel.Split(new char[] { ',' });
                                    foreach (var item in selectedWishList)
                                    {
                                        var wish_id = Utils.ConvertToInt32(item);
                                        if (apiWishModel.FindIndex(x => x.id == wish_id) == -1)
                                        {
                                            var wishDelete = new ApiJobSeekerWishModel();
                                            wishDelete.job_seeker_id = model.job_seeker_id;
                                            wishDelete.id = wish_id;
                                            await A_JobSeekerWishServices.DeleteAsync(wishDelete);
                                        }
                                    }
                                }

                                if (apiWishModel.HasData())
                                {
                                    foreach (var item in apiWishModel)
                                    {
                                        await A_JobSeekerWishServices.UpdateAsync(item);
                                    }
                                }

                                var detailLink = SecurityHelper.GenerateSecureLink("CV", "Preview", new { id = returnId, status = (int)EnumApplicationStatus.Interview, type_job_seeker = 1, show_more = 0, hide_pf = 1 });

                                if (apiModel.id > 0)
                                {
                                    return Json(new { success = true, message = msg, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("RedirectTo('{0}')", detailLink) });
                                }
                                else
                                {
                                    if (model.Id == 0)
                                    {
                                        return Json(new { success = true, message = msg, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("RedirectTo('{0}')", urlEditWithoutProfile) });
                                    }
                                    else
                                    {
                                        return Json(new { success = true, message = msg, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("RedirectTo('{0}')", detailLink) });
                                    }
                                }
                            }

                            return Json(new { success = true, message = msg, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("RedirectTo('{0}')", "/JobSeeker") });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec CreateProfile because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }

        [IsValidURLRequest]
        public ActionResult Update(int? id, int? next_step)
        {
            JobSeekerUpdateProfileModel model = new JobSeekerUpdateProfileModel();
            model.next_step = Utils.ConvertToIntFromQuest(next_step);
            try
            {
                var jobSeekerId = Utils.ConvertToIntFromQuest(id);

                var apiModel = new ApiJobSeekerModel();
                apiModel.id = jobSeekerId;

                var apiReturned = A_JobSeekerServices.GetDetailForUpdateAsync(apiModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        IdentityJobSeeker info = JsonConvert.DeserializeObject<IdentityJobSeeker>(apiReturned.value.ToString());
                        if (info != null)
                        {
                            if (info.agency_id != GetCurrentAgencyId())
                                return RedirectToErrorPage();

                            model = ParseDataToForm(info);
                        }
                        else
                        {
                            return RedirectToErrorPage();
                        }
                    }
                }
                model.job_seeker_id = jobSeekerId;
                model.agency_id = GetCurrentAgencyId();
                model.hide_pf = (Request["hide_pf"] != null) ? Utils.ConvertToInt32(Request["hide_pf"]) : 0;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display Update Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return View(model);
        }

        [IsValidURLRequest]
        public ActionResult Detail(int? id)
        {
            JobSeekerUpdateProfileModel model = new JobSeekerUpdateProfileModel();
            try
            {
                var jobSeekerId = Utils.ConvertToIntFromQuest(id);

                var apiModel = new ApiJobSeekerModel();
                apiModel.id = jobSeekerId;

                var apiReturned = A_JobSeekerServices.GetDetailForUpdateAsync(apiModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        IdentityJobSeeker info = JsonConvert.DeserializeObject<IdentityJobSeeker>(apiReturned.value.ToString());
                        if (info != null)
                        {
                            if (info.agency_id != GetCurrentAgencyId())
                                return RedirectToErrorPage();

                            model = ParseDataToForm(info);
                        }
                        else
                        {
                            return RedirectToErrorPage();
                        }
                    }
                }

                model.job_seeker_id = jobSeekerId;
                model.agency_id = GetCurrentAgencyId();
                model.hide_pf = (Request["hide_pf"] != null) ? Utils.ConvertToInt32(Request["hide_pf"]) : 0;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display Update Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return View(model);
        }

        [AccessRoleChecker]
        public ActionResult DeleteItem(int id, int type)
        {
            JobSeekerDeleteModel model = new JobSeekerDeleteModel();
            model.id = id;
            model.type = type;
            model.tk = SecurityHelper.GenerateSecureLink("JobSeeker", "Delete", new { id = model.id, type = model.type });
            try
            {
                var jResult = A_JobSeekerServices.GetCounterForDeletionAsync(
                    new ApiJobSeekerDeleteModel { agency_id = GetCurrentAgencyId(), id = id }
                    ).Result;
                model.Counter = jResult.ConvertData<IdentityJobSeekerCounter>();
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display DeleteJobSeeker because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_DeleteItem", model);
        }

        [HttpPost]
        [ActionName("DeleteItem")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteItem()
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;

            try
            {
                var id = Request["id"];
                var type = Utils.ConvertToInt32(Request["type"]);
                var tk = Request["tk"];
                var tkSys = SecurityHelper.GenerateSecureLink("JobSeeker", "Delete", new { id = id, type = type });
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
                    var apiReturned = await A_JobSeekerServices.DeletesAsync(apiModel);
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

        //[IsValidURLRequest]
        [AccessRoleChecker]
        public ActionResult Delete(string ids, int type)
        {
            JobSeekerDeleteModel model = new JobSeekerDeleteModel();
            model.ids = ids;
            model.type = type;
            model.tk = SecurityHelper.GenerateSecureLink("JobSeeker", "Delete", new { ids = model.ids, type = model.type });
            try
            {
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display DeleteCvCertificate because: {0}", ex.ToString());
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
                var ids = Request["ids"];
                var type = Utils.ConvertToInt32(Request["type"]);
                var tk = Request["tk"];
                var tkSys = SecurityHelper.GenerateSecureLink("JobSeeker", "Delete", new { ids = ids, type = type });
                if (tk != tkSys)
                {
                    return Json(new { success = false, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_NOTIFICATION });
                }

                var apiModel = new ApiJobSeekerDeleteModel();
                apiModel.ids = ids;
                apiModel.type = type;
                apiModel.agency_id = GetCurrentAgencyId();

                if (!string.IsNullOrEmpty(apiModel.ids))
                {
                    var apiReturned = await A_JobSeekerServices.DeletesAsync(apiModel);
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

        #region Edu history

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> EduHistories(int? id, int is_detail = 0)
        {
            List<JobSeekerEduHistoryModel> myList = null;
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }

            try
            {
                var apiModel = new ApiGetListByPageModel();
                apiModel.job_seeker_id = Utils.ConvertToIntFromQuest(id);

                var apiReturn = await A_JobSeekerServices.GetEduHistoryAsync(apiModel);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<JobSeekerEduHistoryModel>>(apiReturn.value.ToString());

                        if (myList.HasData())
                        {
                            var qualifications = CommonHelpers.GetListQualifications();
                            var majors = CommonHelpers.GetListMajors();
                            foreach (var item in myList)
                            {
                                if (is_detail == 1)
                                {
                                    item.is_detail = true;
                                }

                                var qualification = qualifications.Where(x => x.id == item.qualification_id).FirstOrDefault();
                                if (qualification != null)
                                {
                                    item.qualification_label = qualification.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.qualification).FirstOrDefault();
                                    if (string.IsNullOrEmpty(item.qualification_label))
                                        item.qualification_label = qualification.qualification;
                                }

                                var major = majors.Where(x => x.id == item.major_id).FirstOrDefault();
                                if (major != null)
                                {
                                    item.major_label = major.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.major).FirstOrDefault();
                                    if (string.IsNullOrEmpty(item.major_label))
                                        item.major_label = major.major;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display EduHistories Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_EduHistories", myList);
        }

        public ActionResult UpdateEduHistory(int id = 0, int job_seeker_id = 0)
        {
            var model = new JobSeekerEduHistoryModel();
            try
            {
                model.id = id;
                model.Qualifications = CommonHelpers.GetListQualifications();
                model.Majors = CommonHelpers.GetListMajors();
                model.Majors.Add(new IdentityMajor() { id = 0, major = ManagerResource.LB_MAJOR_OTHER });

                if (model.id > 0)
                {
                    var apiModel = new ApiJobSeekerEduHistoryModel();
                    apiModel.id = model.id;
                    apiModel.job_seeker_id = job_seeker_id;

                    var apiReturned = A_EduHistoryServices.JobSeekerGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityJobSeekerEduHistory info = JsonConvert.DeserializeObject<IdentityJobSeekerEduHistory>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.job_seeker_id = info.job_seeker_id;
                                model.school = info.school;
                                model.start_date_str = info.start_date.DateTimeQuestToString(MONTH_YEAR_FORMAT);
                                model.end_date_str = info.end_date.DateTimeQuestToString(MONTH_YEAR_FORMAT);
                                model.status = Utils.ConvertToIntFromQuest(info.status);
                                model.address = info.address;
                                model.qualification_id = info.qualification_id;
                                model.major_id = info.major_id;
                                model.major_custom = info.major_custom;
                            }
                        }
                    }
                }
                else
                {
                    model.major_id = -1;
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display UpdateEduHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return PartialView("Partials/_UpdateEduHistory", model);
        }

        [HttpPost]
        [ActionName("UpdateEduHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateEduHistory(JobSeekerEduHistoryModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerEduHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = model.job_seeker_id;
                apiModel.school = model.school;
                apiModel.start_date = "01-" + model.start_date_str;
                apiModel.end_date = "01-" + model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;
                apiModel.qualification_id = model.qualification_id;
                apiModel.major_id = model.major_id;
                apiModel.major_custom = model.major_custom;

                var apiReturned = await A_EduHistoryServices.JobSeekerUpdateAsync(apiModel);
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ProfileGlobal.eduHistories();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateEduHistory because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }

        public ActionResult DeleteEduHistory(JobSeekerEduHistoryModel model)
        {
            return PartialView("Partials/_DeleteEduHistory", model);
        }

        [HttpPost]
        [ActionName("DeleteEduHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteEduHistory(JobSeekerEduHistoryModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerEduHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = model.job_seeker_id;

                var apiReturned = await A_EduHistoryServices.JobSeekerDeleteAsync(apiModel);
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ProfileGlobal.eduHistories();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmDeleteEduHistory because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }

        #endregion

        #region Work history

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> WorkHistories(int? id, int is_detail = 0)
        {
            List<JobSeekerWorkHistoryModel> myList = null;

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }
            try
            {
                var apiModel = new ApiGetListByPageModel();
                apiModel.job_seeker_id = Utils.ConvertToIntFromQuest(id);

                var apiReturn = await A_JobSeekerServices.GetWorkHistoryAsync(apiModel);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<JobSeekerWorkHistoryModel>>(apiReturn.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display WorkHistories Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            if (myList.HasData())
            {
                if (is_detail == 1)
                {
                    myList[0].is_detail = true;
                }
            }

            return PartialView("Sub/_WorkHistories", myList);
        }

        public ActionResult UpdateWorkHistory(int id = 0, int job_seeker_id = 0)
        {
            var model = new JobSeekerWorkHistoryModel();
            try
            {
                model.id = id;
                if (model.id > 0)
                {
                    var apiModel = new ApiJobSeekerWorkHistoryModel();
                    apiModel.id = model.id;
                    apiModel.job_seeker_id = job_seeker_id;

                    var apiReturned = A_WorkHistoryServices.JobSeekerGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityJobSeekerWorkHistory info = JsonConvert.DeserializeObject<IdentityJobSeekerWorkHistory>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.job_seeker_id = info.job_seeker_id;
                                model.company = info.company;
                                model.content_work = info.content_work;
                                model.form = info.form;
                                model.start_date_str = info.start_date.DateTimeQuestToString(DATE_FORMAT);
                                model.end_date_str = info.end_date.DateTimeQuestToString(DATE_FORMAT);
                                model.status = Utils.ConvertToIntFromQuest(info.status);
                                model.address = info.address;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display UpdateWorkHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return PartialView("Partials/_UpdateWorkHistory", model);
        }

        [HttpPost]
        [ActionName("UpdateWorkHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateWorkHistory(JobSeekerWorkHistoryModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerWorkHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = model.job_seeker_id;
                apiModel.company = model.company;
                apiModel.content_work = model.content_work;
                apiModel.form = model.form;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;

                var apiReturned = await A_WorkHistoryServices.JobSeekerUpdateAsync(apiModel);
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ProfileGlobal.workHistories();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateWorkHistory because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }

        public ActionResult DeleteWorkHistory(JobSeekerWorkHistoryModel model)
        {
            return PartialView("Partials/_DeleteWorkHistory", model);
        }

        [HttpPost]
        [ActionName("DeleteWorkHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteWorkHistory(JobSeekerWorkHistoryModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerWorkHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = model.job_seeker_id;

                var apiReturned = await A_WorkHistoryServices.JobSeekerDeleteAsync(apiModel);
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ProfileGlobal.workHistories();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmDeleteWorkHistory because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }

        #endregion

        #region Certificate

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> Certificates(int? id, int is_detail = 0)
        {
            List<JobSeekerCertificateModel> myList = null;

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }
            try
            {
                var apiModel = new ApiGetListByPageModel();
                apiModel.job_seeker_id = Utils.ConvertToIntFromQuest(id);

                var apiReturn = await A_JobSeekerServices.GetCertificateAsync(apiModel);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<JobSeekerCertificateModel>>(apiReturn.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display Certificates Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            if (myList.HasData())
            {
                if (is_detail == 1)
                {
                    myList[0].is_detail = true;
                }
            }

            return PartialView("Sub/_Certificates", myList);
        }

        public ActionResult UpdateCertificate(int id = 0, int job_seeker_id = 0)
        {
            var model = new JobSeekerCertificateModel();
            try
            {
                model.id = id;
                if (model.id > 0)
                {
                    var apiModel = new ApiJobSeekerCertificateModel();
                    apiModel.id = model.id;
                    apiModel.job_seeker_id = job_seeker_id;

                    var apiReturned = A_CertificateServices.JobSeekerGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityJobSeekerCertificate info = JsonConvert.DeserializeObject<IdentityJobSeekerCertificate>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.job_seeker_id = info.job_seeker_id;
                                model.name = info.name;
                                model.start_date_str = info.start_date.DateTimeQuestToString(MONTH_YEAR_FORMAT);
                                model.end_date_str = info.end_date.DateTimeQuestToString(MONTH_YEAR_FORMAT);
                                //model.status = Utils.ConvertToIntFromQuest(info.status);
                                model.point = info.point;
                                model.pass = Utils.ConvertToIntFromQuest(info.pass);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display UpdateCertificate because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return PartialView("Partials/_UpdateCertificate", model);
        }

        [HttpPost]
        [ActionName("UpdateCertificate")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateCertificate(JobSeekerCertificateModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerCertificateModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = model.job_seeker_id;
                apiModel.name = model.name;
                //apiModel.start_date = model.start_date_str;
                //apiModel.end_date = model.end_date_str;
                apiModel.start_date = "01-" + model.start_date_str;
                apiModel.end_date = "01-" + model.end_date_str;
                //apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.point = model.point;
                apiModel.pass = Utils.ConvertToIntFromQuest(model.pass);

                var apiReturned = await A_CertificateServices.JobSeekerUpdateAsync(apiModel);
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ProfileGlobal.certificates();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateCertificate because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }

        public ActionResult DeleteCertificate(JobSeekerCertificateModel model)
        {
            return PartialView("Partials/_DeleteCertificate", model);
        }

        [HttpPost]
        [ActionName("DeleteCertificate")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteCertificate(JobSeekerCertificateModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerCertificateModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = model.job_seeker_id;

                var apiReturned = await A_CertificateServices.JobSeekerDeleteAsync(apiModel);
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ProfileGlobal.certificates();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmDeleteCertificate because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }

        #endregion

        #region Helpers

        private ApiJobSeekerModel ExtractFormData(JobSeekerUpdateProfileModel model)
        {
            var info = new ApiJobSeekerModel();
            info.id = model.job_seeker_id;
            info.agency_id = GetCurrentAgencyId();
            info.staff_id = GetCurrentStaffId();
            info.job_seeker_id = info.user_id;
            info.email = model.email;
            info.phone = model.phone;
            info.marriage = model.marriage;
            info.dependent_num = model.dependent_num;
            info.fullname = model.fullname;
            info.fullname_furigana = model.fullname_furigana;
            info.display_name = model.display_name;
            info.image = model.image;
            info.birthday = model.birthday;
            info.gender = model.gender;
            info.id_card = model.id_card;
            info.note = model.note;
            info.video_path = model.video_path;
            info.expected_job_title = model.expected_job_title;
            info.expected_salary_min = model.expected_salary_min;
            info.expected_salary_max = model.expected_salary_max;
            info.work_status = model.work_status;
            info.qualification_id = model.qualification_id;
            info.job_seeking_status_id = model.job_seeking_status_id;
            info.salary_type_id = model.salary_type_id;
            info.japanese_level_number = model.japanese_level_number;
            info.pic_id = model.pic_id;
            info.nationality_id = model.nationality_id;
            info.visa_id = model.visa_id;
            info.duration_visa = model.duration_visa;
            info.religion = model.religion ? 1 : 0;
            info.religion_detail = model.religion_detail;

            info.Addresses = new List<IdentityJobSeekerAddress>();

            var address = new IdentityJobSeekerAddress();
            address.id = model.address.id;
            address.job_seeker_id = info.id;
            address.country_id = model.address.country_id;
            address.region_id = model.address.region_id;
            address.prefecture_id = model.address.prefecture_id;
            address.city_id = model.address.city_id;
            address.postal_code = model.address.postal_code;
            address.detail = model.address.detail;
            address.furigana = model.address.furigana;
            address.train_line_id = model.address.train_line_id;
            address.station_id = model.address.station_id;
            address.is_contact_address = true;

            var address_contact = new IdentityJobSeekerAddress();
            address_contact.id = model.address_contact.id;
            address_contact.job_seeker_id = info.id;
            address_contact.country_id = model.address_contact.country_id;
            address_contact.region_id = model.address_contact.region_id;
            address_contact.prefecture_id = model.address_contact.prefecture_id;
            address_contact.city_id = model.address_contact.city_id;
            address_contact.postal_code = model.address_contact.postal_code;
            address_contact.detail = model.address_contact.detail;
            address_contact.furigana = model.address_contact.furigana;
            address_contact.train_line_id = model.address_contact.train_line_id;
            address_contact.station_id = model.address_contact.station_id;

            info.Addresses.Add(address);
            info.Addresses.Add(address_contact);

            if (model.metadata.HasData())
            {
                info.metadata = new List<IdentityJobSeekerMetaItem>();
                foreach (var item in model.metadata)
                {
                    var f = new IdentityJobSeekerMetaItem();
                    if (!string.IsNullOrEmpty(item.Key))
                        f.key = item.Key.Replace(" ", string.Empty);

                    f.value = item.Value;
                    f.type = item.Type;
                    f.name = item.Name;

                    info.metadata.Add(f);
                }
            }
            //info.metadata = model.metadata;

            return info;
        }
        private List<ApiJobSeekerWishModel> ExtractWishFormData(JobSeekerUpdateProfileModel model)
        {
            var info = new List<ApiJobSeekerWishModel>();
            if (model.WishModel.HasData())
            {
                foreach (var item in model.WishModel)
                {
                    var f = new ApiJobSeekerWishModel();

                    f.id = item.id;
                    f.job_seeker_id = model.job_seeker_id;

                    f.employment_type_id = item.employment_type_id;
                    if (item.employment_type_ids.HasData())
                        f.employment_type_ids = string.Join(",", item.employment_type_ids);
                    if (item.prefecture_ids.HasData())
                        f.prefecture_ids = string.Join(",", item.prefecture_ids);
                    if (item.sub_field_ids.HasData())
                        f.sub_field_ids = string.Join(",", item.sub_field_ids);

                    f.salary_min = Utils.ConvertNumberCommaToInt32(item.salary_min);
                    f.salary_max = Utils.ConvertNumberCommaToInt32(item.salary_max);
                    f.start_date = item.start_date;

                    info.Add(f);
                }
            }

            return info;
        }

        private JobSeekerUpdateProfileModel ParseDataToForm(IdentityJobSeeker identity)
        {
            var model = new JobSeekerUpdateProfileModel();

            model.job_seeker_id = identity.id;
            model.email = identity.email;
            model.phone = identity.phone;
            model.marriage = identity.marriage;
            model.dependent_num = identity.dependent_num;
            model.fullname = identity.fullname;
            model.fullname_furigana = identity.fullname_furigana;
            model.display_name = identity.display_name;
            model.image = identity.image;
            model.birthday = identity.birthday.DateTimeQuestToString(DATE_FORMAT);
            model.gender = identity.gender;
            model.id_card = identity.id_card;
            model.note = identity.note;
            model.video_path = identity.video_path;
            model.expected_job_title = identity.expected_job_title;
            model.expected_salary_min = identity.expected_salary_min;
            model.expected_salary_max = identity.expected_salary_max;
            model.work_status = identity.work_status;
            model.qualification_id = identity.qualification_id;
            model.job_seeking_status_id = identity.job_seeking_status_id;
            model.salary_type_id = identity.salary_type_id;
            model.japanese_level_number = identity.japanese_level_number;
            model.pic_id = identity.pic_id;
            model.visa_id = identity.visa_id;
            model.nationality_id = identity.nationality_id;
            model.duration_visa = identity.duration_visa.DateTimeQuestToString("dd-MM-yyyy");
            model.religion_detail = identity.religion_detail;
            model.staff_id = identity.staff_id;
            if (!string.IsNullOrEmpty(identity.metadata))
            {
                try
                {
                    var customFields = JsonConvert.DeserializeObject<List<IdentityJobSeekerMetaItem>>(identity.metadata);
                    if (customFields.HasData())
                    {
                        model.metadata = new List<CommonCustomFieldModel>();
                        var count = 0;
                        foreach (var item in customFields)
                        {
                            var f = new CommonCustomFieldModel();
                            f.Key = item.key;
                            f.Value = item.value;
                            f.Type = item.type;
                            f.Name = item.name;
                            f.Idx = count;

                            model.metadata.Add(f);

                            count++;
                        }
                    }
                }
                catch
                {
                    logger.Error("Failed to ParseDataToForm. Rawdata: " + identity.metadata);
                }
            }

            model.Extensions = identity.Extensions;
            return model;
        }

        private ApiJobSeekerNoteUpdateModel ExtractJobSeekerNoteFormData(ManageJobSeekerNoteModel model)
        {
            var info = new ApiJobSeekerNoteUpdateModel();

            info.job_seeker_id = model.job_seeker_id;
            info.id = model.Id;
            info.type = model.type;
            info.note = model.note;
            info.type_job_seeker = model.type_job_seeker;

            return info;
        }
        private ApiCandidateInsertModel ExtractCandidateFormData(InterviewProcessInsertModel model)
        {
            var info = new ApiCandidateInsertModel();

            info.job_seeker_id = model.job_seeker_id;
            info.cv_id = model.cv_id;
            info.type = 1;
            info.job_id = model.job_id;
            info.company_id = model.company_id;
            info.agency_id = GetCurrentAgencyId();
            info.staff_id = GetCurrentStaffId();
            info.list_job_seeker_ids = model.list_job_seeker_ids;

            return info;
        }
        private ManageJobSeekerNoteModel ParseDataNoteToForm(IdentityJobSeekerNote info)
        {
            var model = new ManageJobSeekerNoteModel();
            if (info != null)
            {
                model.Id = info.id;
                model.job_seeker_id = info.job_seeker_id;
                model.note = info.note;
                model.type = info.type;
                model.type_job_seeker = info.type_job_seeker;
            }
            return model;
        }

        #endregion
        private JobSeekerWishModel ExtractJobSeekerWishData(IdentityJobSeekerWish info)
        {
            JobSeekerWishModel record = new JobSeekerWishModel();
            record.id = info.id;
            record.employment_type_ids = FrontendHelpers.ConvertStringToList(info.employment_type_ids);
            record.employment_type_id = info.employment_type_id;
            record.prefecture_ids = FrontendHelpers.ConvertStringToList(info.prefecture_ids);
            record.sub_field_ids = FrontendHelpers.ConvertStringToList(info.sub_field_ids);
            record.salary_min = string.Format("{0:N0}", info.salary_min);
            record.salary_max = string.Format("{0:N0}", info.salary_max);
            record.start_date = info.start_date.DateTimeQuestToStringNow("dd-MM-yyyy");

            return record;
        }
    }
}