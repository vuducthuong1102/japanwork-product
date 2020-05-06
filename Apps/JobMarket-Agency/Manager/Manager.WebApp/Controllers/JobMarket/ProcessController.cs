using System;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Settings;
using Manager.SharedLibs;
using System.Collections.Generic;
using System.Linq;
using Manager.WebApp.Services;
using Newtonsoft.Json;
using ApiJobMarket.DB.Sql.Entities;
using Autofac;
using System.Globalization;
using Manager.WebApp.Caching;
using System.Dynamic;

namespace Manager.WebApp.Controllers
{
    public class ProcessController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<ProcessController>();
        private readonly string currentLang = GetCurrentLanguageOrDefault();
        public ProcessController()
        {

        }
        public ActionResult LoadDataJobSeeker()
        {
            ManageJobModel model = new ManageJobModel();
            model.Keyword = Request["query[generalSearch]"] != null ? Request["query[generalSearch]"].ToString() : string.Empty;
            int currentPage = 1;

            if (Request["pagination[page]"] != null)
            {
                model.CurrentPage = Utils.ConvertToInt32(Request["pagination[page]"], 1);
            }

            if (Request["query[type_job_seeker]"] != null)
            {
                model.type_job_seeker = Utils.ConvertToInt32(Request["query[type_job_seeker]"], 0);
            }

            if (Request["query[status_id]"] != null)
            {
                model.status_id = Utils.ConvertToInt32(Request["query[status_id]"], 0);
            }

            if (Request["query[staff_id]"] != null)
            {
                model.staff_id = Utils.ConvertToInt32(Request["query[staff_id]"], 0);
            }

            if (Request["query[japanese_level_number]"] != null)
            {
                model.japanese_level_number = Utils.ConvertToInt32(Request["query[japanese_level_number]"], 0);
            }
            var filter = new ApiGetListByPageModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = model.CurrentPage,
                page_size = Utils.ConvertToInt32(Request["pagination[perpage]"]),
                agency_id = GetCurrentAgencyId(),
                type_job_seeker = model.type_job_seeker,
                staff_id = model.staff_id,
                status = model.status_id,
                japanese_level = model.japanese_level_number
            };
            PagingMeta meta = new PagingMeta();
            meta.page = Utils.ConvertToInt32(Request["pagination[page]"], 1);
            meta.pages = 2;
            meta.perpage = Utils.ConvertToInt32(Request["pagination[perpage]"]);
            List<JobSeekerViewModel> JobSeekers = new List<JobSeekerViewModel>();
            var listVisas = CommonHelpers.GetListVisas();
            var listJapanese_Levels = CommonHelpers.GetListJapaneseLevels();
            model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
            try
            {
                var result = JobSeekerServices.GetByAgencyAsync(filter).Result;

                if (result != null && result.value != null)
                {
                    var listStatus = CommonHelpers.GetListProcessStatus(GetCurrentAgencyId());
                    bool hasStatus = false;
                    if (listStatus.HasData()) hasStatus = true;

                    List<IdentityJobSeeker> listResults = JsonConvert.DeserializeObject<List<IdentityJobSeeker>>(result.value.ToString());
                    if (listResults.HasData())
                    {
                        meta.total = Utils.ConvertToInt32(result.total);
                        var index = 0;
                        foreach (var item in listResults)
                        {
                            index++;
                            var dtJobSeeker = new JobSeekerViewModel();
                            dtJobSeeker.index = (index + (meta.page - 1) * meta.perpage);
                            dtJobSeeker.full_name = item.fullname;
                            dtJobSeeker.id = item.id;
                            dtJobSeeker.code = item.code;
                            dtJobSeeker.company_count = item.company_count;

                            if (hasStatus)
                            {
                                var processInfo = listStatus.FirstOrDefault(s => s.id == item.process_lastest);
                                if (processInfo != null)
                                {
                                    dtJobSeeker.process_lastest = processInfo.status_name;
                                }
                            }

                            dtJobSeeker.StatusList = CommonHelpers.GetListProcessStatusActive(GetCurrentAgencyId());

                            if (model.Staffs.HasData())
                            {
                                var staffInfo = model.Staffs.FirstOrDefault(s => s.StaffId == item.pic_id);
                                if (staffInfo != null) dtJobSeeker.staff_name = staffInfo.FullName;
                            }
                            var visaName = string.Empty;
                            if (listVisas.HasData())
                            {
                                var visaInfo = listVisas.FirstOrDefault(s => s.id == item.visa_id);
                                if (visaInfo != null)
                                {
                                    if (visaInfo.LangList.HasData())
                                    {
                                        var visaLang = visaInfo.LangList.Where(x => x.language_code == currentLang).FirstOrDefault();
                                        if (visaLang != null)
                                        {
                                            visaName = visaLang.visa;
                                        }
                                    }

                                    if (string.IsNullOrEmpty(visaName))
                                    {
                                        visaName = visaInfo.visa;
                                    }
                                }
                            }
                            dtJobSeeker.visa_name = visaName;

                            if (listJapanese_Levels.HasData())
                            {
                                var japaneseInfo = listJapanese_Levels.FirstOrDefault(s => s.id == item.japanese_level_number);
                                if (japaneseInfo != null)
                                {
                                    dtJobSeeker.japanese_level = japaneseInfo.level;
                                }
                            }

                            JobSeekers.Add(dtJobSeeker);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get LoadData because: " + ex.ToString());
            }
            return Json(new { data = JobSeekers, meta = meta });
        }

        [HttpPost]
        public ActionResult GetDetailJobSeekers()
        {

            List<dynamic> detailProcesses = new List<dynamic>();
            var id = Utils.ConvertToInt32(Request["query[id]"]);
            var staff_id = GetCurrentStaffId();
            var agency_id = GetCurrentAgencyId();

            ManageInterviewProcessModel model = new ManageInterviewProcessModel();
            PagingMeta meta = new PagingMeta();

            try
            {
                model.Keyword = Request["query[generalSearch]"] != null ? Request["query[generalSearch]"].ToString() : string.Empty;
                var currentPage = 1;
                if (Request["pagination[page]"] != null)
                {
                    currentPage = Utils.ConvertToInt32(Request["pagination[page]"], 1);
                }

                int type_job_seeker = 0;
                if (Request["query[type_job_seeker]"] != null)
                {
                    type_job_seeker = Utils.ConvertToInt32(Request["query[type_job_seeker]"], 0);
                }

                int status_id = 0;
                if (Request["query[status_id]"] != null)
                {
                    status_id = Utils.ConvertToInt32(Request["query[status_id]"], 0);
                }

                model.agency_id = agency_id;
                var filter = new ApiGetListByPageModel
                {
                    keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                    page_index = currentPage,
                    page_size = Utils.ConvertToInt32(Request["pagination[perpage]"]),
                    job_seeker_id = id,
                    staff_id = staff_id,
                    agency_id = agency_id,
                    type_job_seeker = type_job_seeker,
                    status = status_id
                };
                var statusList = CommonHelpers.GetListProcessStatusActive(agency_id);
                var listCompanyIds = new List<int>();
                var listCVs = new List<int>();

                meta.page = Utils.ConvertToInt32(Request["pagination[page]"], 1);
                meta.pages = 2;
                meta.perpage = Utils.ConvertToInt32(Request["pagination[perpage]"]);
                if (meta.perpage == 0) meta.perpage = SystemSettings.DefaultPageSize;

                var result = CandidateServices.GetByJobSeekerAsync(filter).Result;


                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCandidate>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        meta.total = Utils.ConvertToInt32(result.total);

                        listCompanyIds = model.SearchResults.Select(x => x.job_info.company_id).ToList();
                        if (listCompanyIds.HasData())
                        {
                            var companyReturnApi = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCompanyIds }).Result;
                            if (companyReturnApi != null && companyReturnApi.value != null)
                                model.Companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companyReturnApi.value.ToString());
                        }

                        listCVs = model.SearchResults.Where(x => x.cv_id > 0).Select(x => x.cv_id).ToList();
                        var listCandidateIds = model.SearchResults.Select(s => s.id).ToList();
                        if (listCandidateIds.HasData())
                        {
                            var listStrId = string.Join(",", listCandidateIds);
                            var listResult = InterviewProcessServices.GetListByCandidateIdsAsync(listStrId).Result;
                            if (!string.IsNullOrEmpty(listResult.value.ToString()))
                            {
                                List<IdentityInterviewProcess> listInterviews = JsonConvert.DeserializeObject<List<IdentityInterviewProcess>>(listResult.value.ToString());
                                foreach (var item in model.SearchResults)
                                {
                                    var listInterviewDetails = listInterviews.Where(s => s.candidate_id == item.id).ToList();
                                    item.ListInterviewProcess = new List<IdentityInterviewProcess>();
                                    item.ListInterviewProcess = listInterviewDetails;
                                }
                            }
                        }
                    }
                }
                if (listCVs.HasData())
                {
                    var cvReturnApi = CvServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCVs }).Result;
                    if (cvReturnApi != null && cvReturnApi.value != null)
                        model.CVs = JsonConvert.DeserializeObject<List<IdentityCv>>(cvReturnApi.value.ToString());
                }
                model.ProcessStatuses = CommonHelpers.GetListProcessStatus(agency_id);
                model.SubFields = CommonHelpers.GetListSubFields();
                model.Staffs = CommonHelpers.GetListUser(agency_id);

                if (model.SearchResults.HasData())
                {
                    foreach (var record in model.SearchResults)
                    {
                        int fc_create = 0;
                        var dtProcess = new ExpandoObject() as IDictionary<string, Object>;
                        var jobName = "";

                        if (record.job_info != null)
                        {
                            dtProcess.Add("job_code", record.job_info.job_code);
                            if (record.job_info.Job_translations.HasData())
                            {
                                jobName = record.job_info.Job_translations.Where(x => x.language_code == currentLang).Select(x => x.title).FirstOrDefault();
                                if (string.IsNullOrEmpty(jobName))
                                {
                                    jobName = record.job_info.Job_translations[0].title;
                                }

                            }
                        }
                        dtProcess.Add("job_name", jobName);

                        var subFieldName = string.Empty;
                        var subFieldInfo = model.SubFields.Where(x => x.id == record.job_info.sub_field_id).FirstOrDefault();
                        if (subFieldInfo != null)
                        {
                            if (subFieldInfo.LangList.HasData())
                            {
                                var subFieldLang = subFieldInfo.LangList.Where(x => x.language_code == currentLang).FirstOrDefault();
                                if (subFieldLang != null)
                                {
                                    subFieldName = subFieldLang.sub_field;
                                }
                            }
                            if (string.IsNullOrEmpty(subFieldName))
                            {
                                subFieldName = subFieldInfo.sub_field;
                            }
                        }
                        dtProcess.Add("sub_field_name", subFieldName);

                        var companyName = string.Empty;
                        var companyInfo = model.Companies.Where(x => x.id == record.job_info.company_id).FirstOrDefault();
                        if (companyInfo != null)
                        {
                            companyName = companyInfo.company_name;
                            if (companyInfo.LangList.HasData())
                            {
                                var companyLang = companyInfo.LangList.Where(x => x.language_code == currentLang).FirstOrDefault();
                                if (companyLang == null)
                                {
                                    companyLang = companyInfo.LangList[0];
                                }

                                if (companyLang != null)
                                {
                                    companyName = companyLang.company_name;
                                }
                            }
                        }
                        dtProcess.Add("company_name", companyName);

                        var picName = string.Empty;
                        var staffInfo = model.Staffs.Where(x => x.StaffId == record.pic_id).FirstOrDefault();
                        if (staffInfo != null)
                        {
                            picName = staffInfo.FullName;
                        }
                        dtProcess.Add("pic_name", picName);


                        if (statusList.HasData())
                        {
                            IdentityInterviewProcess lastStatus = null;
                            if (record.ListInterviewProcess.HasData())
                            {
                                lastStatus = record.ListInterviewProcess.FirstOrDefault(s => s.status_id == record.interview_status_id);
                                var itemStatus = statusList.FirstOrDefault(s => s.id == record.interview_status_id);
                                if (itemStatus != null)
                                {
                                    dtProcess.Add("last_status", itemStatus.status_name);
                                }
                                else
                                {
                                    dtProcess.Add("last_status", string.Empty);
                                }
                                if (lastStatus != null)
                                {
                                    dtProcess.Add("last_time", lastStatus.modified_at.DateTimeQuestToLocaleString(currentLang));
                                }
                                else
                                {
                                    dtProcess.Add("last_time", string.Empty);
                                }
                            }
                            foreach (var st in statusList)
                            {
                                var stValue = string.Empty;
                                if (record.ListInterviewProcess.HasData())
                                {
                                    var lastModified = record.ListInterviewProcess.Where(x => x.status_id == st.id).Select(x => x.modified_at).FirstOrDefault();
                                    if (lastModified != null)
                                        stValue = lastModified.DateTimeQuestToLocaleString(currentLang);
                                }
                                dtProcess.Add("status_" + st.id, stValue);
                            }

                        }

                        if (PermissionHelper.CheckPermission("Create", "Process")
                            && (PermissionHelper.CheckIsPic(record.pic_id) || PermissionHelper.CheckIsPic(record.pic_job_id)))
                        {
                            if (record.interview_status_id != 27)
                            {
                                fc_create = 1;
                            }
                            else
                            {
                                fc_create = -1;
                            }
                        }

                        dtProcess.Add("fc_create", fc_create);
                        dtProcess.Add("fc_show", 1);
                        dtProcess.Add("id", record.id);

                        detailProcesses.Add(dtProcess);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetDetails because: " + ex.ToString());
            }

            return Json(new { data = detailProcesses, meta = meta });
        }

        public ActionResult JobSeeker()
        {
            ManageInterviewProcessModel model = new ManageInterviewProcessModel();

            model = GetDefaultFilterModel(model, 2);

            model.type_job_seeker = -1;
            model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
            model.ProcessStatuses = CommonHelpers.GetListProcessStatus(GetCurrentAgencyId());
            model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
            model.SubFields = CommonHelpers.GetListSubFields();
            model.agency_id = GetCurrentAgencyId();
            model.type = 1;
            model.status_id = -1;

            return View(model);
        }


        public ActionResult LoadDataJob()
        {
            ManageJobModel model = new ManageJobModel();
            model.Keyword = Request["query[generalSearch]"] != null ? Request["query[generalSearch]"].ToString() : string.Empty;
            var models = new List<JobViewModel>();
            var currentLang = UserCookieManager.GetCurrentLanguageOrDefault();
            model = GetDefaultFilterModel(model, 2);

            if (Request["pagination[page]"] != null)
            {
                model.CurrentPage = Utils.ConvertToInt32(Request["pagination[page]"], 1);
            }

            if (Request["query[sub_id]"] != null)
            {
                model.sub_id = Utils.ConvertToInt32(Request["query[sub_id]"], 0);
            }

            if (Request["query[status_id]"] != null)
            {
                model.status_id = Utils.ConvertToInt32(Request["query[status_id]"], 0);
            }

            if (Request["query[staff_id]"] != null)
            {
                model.staff_id = Utils.ConvertToInt32(Request["query[staff_id]"], 0);
            }

            var filter = new ApiJobModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = model.CurrentPage,
                page_size = Utils.ConvertToInt32(Request["pagination[perpage]"]),
                status = model.status_id,
                agency_id = GetCurrentAgencyId(),
                translate_status = -1,
                staff_id = model.staff_id,
                sub_id = model.sub_id
            };

            PagingMeta meta = new PagingMeta();
            meta.page = Utils.ConvertToInt32(Request["pagination[page]"], 1);
            meta.pages = 2;
            meta.perpage = Utils.ConvertToInt32(Request["pagination[perpage]"]);

            try
            {
                if (model.company_id > 0)
                {
                    var companyResult = CompanyServices.GetDetailAsync(new ApiCompanyModel { id = model.company_id }).Result;
                    if (companyResult != null)
                    {
                        if (companyResult.value != null)
                        {
                            IdentityCompany companyInfo = JsonConvert.DeserializeObject<IdentityCompany>(companyResult.value.ToString());
                            if (companyInfo != null)
                            {
                                model.CompanyInfo = companyInfo;
                                if (companyInfo.LangList.HasData())
                                {
                                    model.company_name = companyInfo.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.company_name).FirstOrDefault();
                                }

                                if (string.IsNullOrEmpty(model.company_name))
                                    model.company_name = companyInfo.company_name;
                            }
                        }
                    }
                }

                var statusList = CommonHelpers.GetListProcessStatusActive(GetCurrentAgencyId());
                var listStatus = CommonHelpers.GetListProcessStatus(GetCurrentAgencyId());
                bool hasStatus = false;
                if (listStatus.HasData()) { hasStatus = true; }
                var result = JobServices.GetListProcessAsync(filter).Result;
                model.SubFields = CommonHelpers.GetListSubFields();
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJob>>(result.value.ToString());
                    model.Industries = CommonHelpers.GetListIndustries();
                    model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                    if (model.company_id <= 0)
                    {
                        var listCompanyIds = model.SearchResults.Select(x => x.company_id).ToList();

                        var companyReturnApi = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCompanyIds }).Result;
                        if (companyReturnApi != null && companyReturnApi.value != null)
                            model.Companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companyReturnApi.value.ToString());
                    }
                    if (model.SearchResults.HasData())
                    {
                        meta.total = Utils.ConvertToInt32(result.total);
                        foreach (var record in model.SearchResults)
                        {
                            var jobName = string.Empty;
                            if (record.Job_translations.HasData())
                            {
                                jobName = record.Job_translations.Where(x => x.language_code == currentLang).Select(x => x.title).FirstOrDefault();
                                if (string.IsNullOrEmpty(jobName))
                                {
                                    jobName = record.Job_translations.Where(x => x.language_code != currentLang).Select(x => x.title).FirstOrDefault();
                                }
                            }

                            var item = new JobViewModel();
                            //item.application_count = ((int)record.Extensions.application_count).FormatWithComma();
                            item.candidate_count = ((int)record.Extensions.candidate_count).FormatWithComma();
                            item.quantity = record.quantity.FormatWithComma();
                            item.created_at = record.created_at.DateTimeQuestToString();
                            item.job_name = jobName;
                            item.job_id = record.id;
                            item.job_code = record.job_code;
                            item.pic_id = record.pic_id;
                            if (hasStatus)
                            {
                                var processInfo = listStatus.FirstOrDefault(s => s.id == ((int)record.Extensions.process_lastest));
                                if (processInfo != null)
                                {
                                    item.process_lastest = processInfo.status_name;
                                }
                            }


                            var subFieldName = string.Empty;
                            if (model.SubFields.HasData())
                            {
                                var subField = model.SubFields.FirstOrDefault(s => s.id == record.sub_field_id);
                                if (subField != null)
                                {
                                    if (subField.LangList.HasData())
                                    {
                                        subFieldName = subField.LangList.Where(x => x.language_code == currentLang).Select(x => x.sub_field).FirstOrDefault();
                                        if (string.IsNullOrEmpty(subFieldName))
                                        {
                                            subFieldName = subField.LangList[0].sub_field;
                                        }
                                    }

                                    if (string.IsNullOrEmpty(subFieldName))
                                    {
                                        subFieldName = subField.sub_field;
                                    }
                                }
                            }
                            item.sub_field = subFieldName;
                            if (model.Staffs.HasData())
                            {
                                var staffInfo = model.Staffs.FirstOrDefault(s => s.StaffId == record.pic_id);
                                if (staffInfo != null)
                                {
                                    item.person_charge_application = staffInfo.FullName;
                                }
                            }
                            var companyName = string.Empty;
                            var companyInfo = model.Companies.Where(x => x.id == record.company_id).FirstOrDefault();
                            if (companyInfo != null)
                            {
                                companyName = companyInfo.company_name;
                                if (companyInfo.LangList.HasData())
                                {
                                    var companyLang = companyInfo.LangList.Where(x => x.language_code == currentLang).FirstOrDefault();
                                    if (companyLang == null)
                                    {
                                        companyLang = companyInfo.LangList[0];
                                    }

                                    if (companyLang != null)
                                    {
                                        companyName = companyLang.company_name;
                                    }
                                }
                            }

                            item.company_name = companyName;


                            item.StatusList = statusList;

                            models.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to LoadData because: " + ex.ToString());
            }

            return Json(new { data = models, meta = meta });
        }
        [HttpPost]
        public ActionResult GetDetailJobs()
        {
            List<dynamic> detailProcesses = new List<dynamic>();
            var job_id = Utils.ConvertToInt32(Request["query[job_id]"]);
            var staff_id = GetCurrentStaffId();
            var agency_id = GetCurrentAgencyId();

            ManageInterviewProcessModel model = new ManageInterviewProcessModel();
            PagingMeta meta = new PagingMeta();

            try
            {
                model.Keyword = Request["query[generalSearch]"] != null ? Request["query[generalSearch]"].ToString() : string.Empty;
                var currentPage = 1;
                if (Request["Page"] != null)
                {
                    currentPage = Utils.ConvertToInt32(Request["Page"], 1);
                }
                int type_job_seeker = 0;
                if (Request["query[type_job_seeker]"] != null)
                {
                    type_job_seeker = Utils.ConvertToInt32(Request["query[type_job_seeker]"], 0);
                }

                int status = 0;
                if (Request["query[status_id]"] != null)
                {
                    status = Utils.ConvertToInt32(Request["query[status_id]"], 0);
                }

                var filter = new ApiInterviewProcessSearchModel
                {
                    keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                    page_index = currentPage,
                    page_size = Utils.ConvertToInt32(Request["pagination[perpage]"]),
                    job_id = job_id,
                    agency_id = agency_id,
                    type_job_seeker = type_job_seeker,
                    staff_id = staff_id,
                    status = status
                };
                var statusList = CommonHelpers.GetListProcessStatusActive(agency_id);
                var listCompanyIds = new List<int>();
                var listJobSeekers = new List<int>();

                meta.page = Utils.ConvertToInt32(Request["pagination[page]"], 1);
                meta.pages = 2;
                meta.perpage = Utils.ConvertToInt32(Request["pagination[perpage]"]);
                if (meta.perpage == 0) meta.perpage = SystemSettings.DefaultPageSize;
                model.Staffs = CommonHelpers.GetListUser(agency_id);
                var result = AgencyServices.GetCandidatesByJobIdAsync(filter).Result;

                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCandidate>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        meta.total = Utils.ConvertToInt32(result.total);
                        listJobSeekers = model.SearchResults.Where(x => x.job_seeker_id > 0).Select(x => x.job_seeker_id).ToList();
                        var listCandidateIds = model.SearchResults.Select(s => s.id).ToList();
                        if (listCandidateIds.HasData())
                        {
                            var listStrId = string.Join(",", listCandidateIds);
                            var listResult = InterviewProcessServices.GetListByCandidateIdsAsync(listStrId).Result;
                            if (!string.IsNullOrEmpty(listResult.value.ToString()))
                            {
                                List<IdentityInterviewProcess> listInterviews = JsonConvert.DeserializeObject<List<IdentityInterviewProcess>>(listResult.value.ToString());
                                foreach (var item in model.SearchResults)
                                {
                                    var listInterviewDetails = listInterviews.Where(s => s.candidate_id == item.id).ToList();
                                    item.ListInterviewProcess = new List<IdentityInterviewProcess>();
                                    item.ListInterviewProcess = listInterviewDetails;
                                }
                            }
                        }
                    }
                }
                model.ProcessStatuses = CommonHelpers.GetListProcessStatus(agency_id);


                if (model.SearchResults.HasData())
                {
                    var index = 0;

                    foreach (var record in model.SearchResults)
                    {
                        index++;
                        int fc_create = 0;
                        var dtProcess = new ExpandoObject() as IDictionary<string, Object>;

                        if (statusList.HasData())
                        {
                            IdentityInterviewProcess lastStatus = null;
                            if (record.ListInterviewProcess.HasData())
                            {
                                lastStatus = record.ListInterviewProcess.OrderByDescending(s => s.created_at).FirstOrDefault();
                                var itemStatus = statusList.FirstOrDefault(s => s.id == lastStatus.status_id);
                                if (itemStatus != null)
                                {
                                    dtProcess.Add("last_status", itemStatus.status_name);
                                }
                                else
                                {
                                    dtProcess.Add("last_status", string.Empty);
                                }

                                dtProcess.Add("last_time", lastStatus.modified_at.DateTimeQuestToLocaleString(currentLang));
                            }

                            foreach (var st in statusList)
                            {
                                var stValue = string.Empty;
                                if (record.ListInterviewProcess.HasData())
                                {
                                    var lastModified = record.ListInterviewProcess.Where(x => x.status_id == st.id).Select(x => x.modified_at).FirstOrDefault();
                                    stValue = lastModified.DateTimeQuestToLocaleString(currentLang);
                                }
                                dtProcess.Add("status_" + st.id, stValue);
                            }

                        }

                        if (PermissionHelper.CheckPermission("Create", "Process")
                            && (PermissionHelper.CheckIsPic(record.pic_id) || PermissionHelper.CheckIsPic(record.pic_job_id)))
                        {
                            if (record.interview_status_id != 27)
                            {
                                fc_create = 1;
                            }
                            else
                            {
                                fc_create = -1;
                            }
                        }

                        dtProcess.Add("fc_create", fc_create);
                        dtProcess.Add("fc_show", 1);

                        var staffName = "";
                        if (model.Staffs.HasData())
                        {

                            var staffInfo = model.Staffs.FirstOrDefault(s => s.StaffId == record.pic_id);

                            if (staffInfo != null)
                            {
                                staffName = staffInfo.FullName;
                            }
                        }

                        if (record.job_seeker_info != null)
                        {
                            dtProcess.Add("full_name", record.job_seeker_info.fullname);
                            dtProcess.Add("code", record.job_seeker_info.code);
                        }
                        else
                        {
                            dtProcess.Add("full_name", string.Empty);
                            dtProcess.Add("code", string.Empty);
                        }
                        dtProcess.Add("staff_name", staffName);
                        dtProcess.Add("id", record.id);
                        dtProcess.Add("index", (index + (meta.page - 1) * meta.perpage));

                        detailProcesses.Add(dtProcess);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetDetails because: " + ex.ToString());
            }

            return Json(new { data = detailProcesses, meta = meta });
        }

        public ActionResult Job()
        {
            ManageJobProcessModel model = new ManageJobProcessModel();
            try
            {
                model = GetDefaultFilterModel(model, 2);
                model.type = 2;
                model.type_job_seeker = -1;
                model.SubFields = CommonHelpers.GetListSubFields();
                model.ProcessStatuses = CommonHelpers.GetListProcessStatus(GetCurrentAgencyId());
                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.agency_id = GetCurrentAgencyId();
                model.status_id = (int)EnumJobStatus.Published;
            }
            catch (Exception ex)
            {
                logger.Error("Failed to get data because: " + ex.ToString());
            }

            return View(model);
        }

        [AccessRoleChecker]
        public ActionResult Create(int candidate_id, int status_id)
        {
            var model = new InterviewProcessInsertModel();
            try
            {
                model.modified_at_time = DateTime.Now;
                model.modified_at = model.modified_at_time.Value.ToString("yyyy/MM/dd");
                model.ProcessStatuses = CommonHelpers.GetListProcessStatus(GetCurrentAgencyId());
                model.status_id = status_id;
                model.candidate_id = candidate_id;
                model.ProcessStatuses = CommonHelpers.GetListProcessStatus(GetCurrentAgencyId());
                model.agency_id = GetCurrentAgencyId();
                var resultList = InterviewProcessServices.GetListByCandidateAsync(candidate_id).Result;
                if (!string.IsNullOrEmpty(resultList.value.ToString()))
                {
                    model.ListInterviewProcess = JsonConvert.DeserializeObject<List<IdentityInterviewProcess>>(resultList.value.ToString());
                }

                var candidateInfo = CandidateServices.GetDetailAsync(candidate_id).Result;
                if (!string.IsNullOrEmpty(candidateInfo.value.ToString()))
                {
                    IdentityCandidate result = JsonConvert.DeserializeObject<IdentityCandidate>(candidateInfo.value.ToString());
                    if (result != null)
                    {
                        if (result.job_info != null)
                        {
                            if (result.job_info.company_info != null)

                                model.company_name = result.job_info.company_info.company_name;

                            if (result.job_info.Job_translations.HasData())
                            {
                                model.job_name = result.job_info.Job_translations.Where(x => x.language_code == currentLang).Select(x => x.title).FirstOrDefault();
                                if (string.IsNullOrEmpty(model.job_name))
                                {
                                    model.job_name = result.job_info.Job_translations[0].title;
                                }
                            }
                            model.type_job_seeker = result.type;
                            model.job_seeker_info = result.job_seeker_info;
                            if (result.cv_id > 0 && result.type == 0)
                            {
                                var cvReturnApi = CvServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = new List<int> { result.cv_id } }).Result;
                                if (cvReturnApi != null && cvReturnApi.value != null)
                                {
                                    List<IdentityCv> listCVs = JsonConvert.DeserializeObject<List<IdentityCv>>(cvReturnApi.value.ToString());
                                    if (listCVs.HasData()) model.full_name = listCVs[0].fullname;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display Create Interview Process because: {0}", ex.ToString()));
            }

            return PartialView("Partials/_Create", model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Post(InterviewProcessInsertModel model)
        {
            DateTime? ModifiedAt = DateTime.Now;
            var statusName = string.Empty;
            try
            {
                //Begin create
                var apiModel = ExtractInterviewProcessFormData(model);
                ModifiedAt = apiModel.modified_at;
                apiModel.agency_id = GetCurrentAgencyId();

                var apiReturned = InterviewProcessServices.InsertAsync(apiModel).Result;
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
                var listStatus = CommonHelpers.GetListProcessStatus(GetCurrentAgencyId());
                if (listStatus.HasData())
                {
                    var statusInfo = listStatus.FirstOrDefault(s => s.id == model.status_id);
                    if (statusInfo != null) statusName = statusInfo.status_name;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to InterviewProcess because: " + ex.ToString());
            }
            return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ProcessDetail({0},'{1}',{2},'{3}')", model.candidate_id, ModifiedAt.Value.ToString("yyyy/MM/dd"), model.status_id, statusName) });
            //return RedirectToAction("Index");
        }

        [AccessRoleChecker]
        public ActionResult CreateCandidate()
        {
            var model = new InterviewProcessInsertModel();
            try
            {
                model.modified_at_time = DateTime.Now;
                model.modified_at = model.modified_at_time.DateTimeQuestToLocaleString("yyyy/MM/dd HH:mm");
                model.ProcessStatuses = CommonHelpers.GetListProcessStatus(GetCurrentAgencyId());
                if (model.ProcessStatuses.HasData())
                    model.status_id = model.ProcessStatuses[0].id;
                int currentPage = 1;
                int pageSize = SystemSettings.DefaultPageSize;


                var filterCompany = new ApiCompanyModel
                {
                    page_index = currentPage,
                    page_size = pageSize,
                    keyword = null,
                    agency_id = GetCurrentAgencyId()
                };

                var resultCompany = AgencyServices.GetCompaniesAsync(filterCompany).Result;
                if (resultCompany != null && resultCompany.value != null)
                {
                    model.JobSeekers = JsonConvert.DeserializeObject<List<IdentityCompany>>(resultCompany.value.ToString());
                }

                var filter = new ApiJobSeekerByPageModel
                {
                    page_index = currentPage,
                    page_size = pageSize,
                    status = -1,
                    agency_id = GetCurrentAgencyId()
                };

                var result = A_JobSeekerServices.GetListByPageAsync(filter).Result;
                if (result != null && result.value != null)
                {
                    model.JobSeekers = JsonConvert.DeserializeObject<List<IdentityJobSeeker>>(result.value.ToString());
                }

                //var resultList = InterviewProcessServices.GetListByCandidateAsync(candidate_id).Result;
                //if (!string.IsNullOrEmpty(resultList.value.ToString()))
                //{
                //    model.ListInterviewProcess = JsonConvert.DeserializeObject<List<IdentityInterviewProcess>>(resultList.value.ToString());
                //}
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display Create Candidate because: {0}", ex.ToString()));
            }

            return PartialView("Partials/_CreateCandidate", model);
        }

        [HttpPost, ActionName("CreateCandidate")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Candidate(InterviewProcessInsertModel model)
        {
            try
            {
                //Begin create
                var apiModel = ExtractInterviewProcessFormData(model);
                apiModel.agency_id = GetCurrentStaffId();
                var apiReturned = InterviewProcessServices.InsertAsync(apiModel).Result;
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

                logger.Error("Failed to CreateCandidate because: " + ex.ToString());
            }
            return RedirectToAction("Index");
        }
        [AccessRoleChecker]
        public ActionResult Edit(int id)
        {
            ModelState.Clear();
            var interviewProcessId = Utils.ConvertToIntFromQuest(id);
            var model = new InterviewProcessEditModel();
            try
            {
                if (interviewProcessId <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var resultIvP = InterviewProcessServices.GetDetailAsync(interviewProcessId).Result;
                if (resultIvP != null && resultIvP.value != null)
                {
                    var info = JsonConvert.DeserializeObject<IdentityInterviewProcess>(resultIvP.value.ToString());
                    model = ParseDataToForm(info);
                }

                var candidateInfo = CandidateServices.GetDetailAsync(model.candidate_id).Result;
                if (!string.IsNullOrEmpty(candidateInfo.value.ToString()))
                {
                    IdentityCandidate result = JsonConvert.DeserializeObject<IdentityCandidate>(candidateInfo.value.ToString());
                    if (result != null)
                    {
                        if (result.job_info != null)
                        {
                            if (result.job_info.company_info != null)

                                model.company_name = result.job_info.company_info.company_name;

                            if (result.job_info.Job_translations.HasData())
                            {
                                model.job_name = result.job_info.Job_translations.Where(x => x.language_code == currentLang).Select(x => x.title).FirstOrDefault();
                                if (string.IsNullOrEmpty(model.job_name))
                                {
                                    model.job_name = result.job_info.Job_translations.Where(x => x.language_code != currentLang).Select(x => x.title).FirstOrDefault();
                                }
                            }
                            if (result.cv_id > 0)
                            {
                                var cvReturnApi = CvServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = new List<int> { result.cv_id } }).Result;
                                if (cvReturnApi != null && cvReturnApi.value != null)
                                {
                                    List<IdentityCv> listCVs = JsonConvert.DeserializeObject<List<IdentityCv>>(cvReturnApi.value.ToString());
                                    if (listCVs.HasData()) model.full_name = listCVs[0].fullname;
                                }
                            }

                            if (result.type == 1 && result.job_seeker_id > 0)
                            {
                                var apiModel = new ApiJobSeekerModel();
                                apiModel.id = result.job_seeker_id;
                                var returnApi = A_JobSeekerServices.GetDetailForUpdateAsync(apiModel).Result;
                                if (returnApi != null && returnApi.value != null)
                                {
                                    IdentityJobSeeker info = JsonConvert.DeserializeObject<IdentityJobSeeker>(returnApi.value.ToString());
                                    if (info != null) model.full_name = info.fullname;
                                }
                            }
                        }

                    }
                }

                model.agency_id = GetCurrentAgencyId();
                model.ProcessStatuses = CommonHelpers.GetListProcessStatus(GetCurrentAgencyId());
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display Edit Interview Process [{0}] because: {1}", id, ex.ToString()));
            }

            return PartialView("Partials/_Edit", model);
        }

        public ActionResult ShowInterviewProcess(int id)
        {
            ManageInterviewProcessModel model = new ManageInterviewProcessModel();
            model.Id = id;
            model.staff_id = GetCurrentStaffId();
            model.agency_id = GetCurrentAgencyId();
            var candidate_id = 0;
            try
            {
                var resultList = InterviewProcessServices.GetListByCandidateAsync(id).Result;
                if (resultList != null && resultList.value != null)
                {
                    model.ListInterviewProcess = JsonConvert.DeserializeObject<List<IdentityInterviewProcess>>(resultList.value.ToString());
                    if (model.ListInterviewProcess.HasData()) candidate_id = model.ListInterviewProcess[0].candidate_id;
                }

                var resultCandidate = CandidateServices.GetDetailAsync(id).Result;
                var job_id = 0;
                if (resultCandidate != null && resultCandidate.value != null)
                {
                    var candidateInfo = JsonConvert.DeserializeObject<IdentityCandidate>(resultCandidate.value.ToString());
                    if (candidateInfo != null)
                    {
                        model.pic_id = candidateInfo.pic_id;
                        model.interview_status_id = candidateInfo.interview_status_id;
                        job_id = candidateInfo.job_id;
                    }
                }

                if (job_id > 0)
                {
                    var apiJobModel = new ApiJobModel();
                    apiJobModel.job_id = job_id;
                    var resultJob = JobServices.GetDetailAsync(apiJobModel).Result;
                    if (resultJob != null && resultJob.value != null)
                    {
                        var jobInfo = JsonConvert.DeserializeObject<IdentityJob>(resultJob.value.ToString());
                        if (jobInfo != null)
                        {
                            model.pic_job_id = jobInfo.pic_id;
                        }
                    }
                }
                var listAgency = model.ListInterviewProcess.Select(s => s.agency_id).ToList();

                model.ProcessStatuses = CommonHelpers.GetListProcessStatus(model.agency_id);
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                PartialView("Partials/_Detail", model);
            }
            return PartialView("Partials/_Detail", model);
        }
        private ApiInterviewProcessEditModel ExtractInterviewProcessFormData(InterviewProcessEditModel model)
        {
            var info = new ApiInterviewProcessEditModel();

            info.job_id = model.job_id;
            if (!string.IsNullOrEmpty(model.modified_at))
            {
                info.modified_at = DateTime.ParseExact(model.modified_at, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            }
            info.status_id = model.status_id;
            info.cv_id = model.cv_id;
            info.candidate_id = model.candidate_id;
            info.note = model.note;

            return info;
        }

        private ApiInterviewProcessInsertModel ExtractInterviewProcessFormData(InterviewProcessInsertModel model)
        {
            var info = new ApiInterviewProcessInsertModel();

            info.job_id = model.job_id;
            info.modified_at = DateTime.ParseExact(model.modified_at, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            //info.modified_at = DateTime.Now;
            info.status_id = model.status_id;
            info.cv_id = model.cv_id;
            info.candidate_id = model.candidate_id;
            info.note = model.note;
            info.staff_id = GetCurrentStaffId();

            return info;
        }

        private InterviewProcessEditModel ParseDataToForm(IdentityInterviewProcess info)
        {
            var model = new InterviewProcessEditModel();
            if (info != null)
            {
                model.Id = info.id;
                model.job_id = info.job_id;
                model.modified_at = info.modified_at.Value.ToString("yyyy/MM/dd");
                model.note = info.note;
                model.status_id = info.status_id;
                model.candidate_id = info.candidate_id;
                model.agency_id = info.agency_id;
            }
            return model;
        }

        private ApiInterviewProcessEditModel ParseDataToForm(InterviewProcessEditModel info)
        {
            var model = new ApiInterviewProcessEditModel();
            if (info != null)
            {
                model.id = info.Id;
                model.job_id = info.job_id;
                if (!string.IsNullOrEmpty(info.modified_at))
                {
                    model.modified_at = DateTime.ParseExact(info.modified_at, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                }
                model.note = info.note;
                model.status_id = info.status_id;
                model.candidate_id = info.candidate_id;
                model.agency_id = info.agency_id;
                model.staff_id = GetCurrentStaffId();
            }

            return model;
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProcess(InterviewProcessEditModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            try
            {
                //Begin update
                var apiModel = ParseDataToForm(model);
                if (!string.IsNullOrEmpty(model.modified_at))
                {
                    apiModel.modified_at = DateTime.ParseExact(model.modified_at, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                }
                var apiReturned = InterviewProcessServices.UpdateAsync(apiModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowInterviewProcess({0})", model.candidate_id) });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(apiReturned.message))
                            {
                                return Json(new { success = true, message = apiReturned.message, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowInterviewProcess({0})", model.candidate_id) });
                            }
                            else
                            {
                                return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowInterviewProcess({0})", model.candidate_id) });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec UpdateProcess because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowInterviewProcess({0})", model.candidate_id) });
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
                var result = InterviewProcessServices.DeleteAsync(new ApiInterviewProcessDeleteModel { id = id }).Result;
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Interview Process because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
        }
    }
}