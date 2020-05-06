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
    public class InvitationHistoryController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<InvitationHistoryController>();

        public InvitationHistoryController()
        {

        }
        
        public ActionResult Index(ManageInvitationModel model)
        {
            model = GetDefaultFilterModel(model);

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }
            var filter = new ApiCvInvitationModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = model.CurrentPage,
                page_size = model.PageSize,
                company_id = model.company_id,
                status = (model.Status == null) ? -99 : Utils.ConvertToIntFromQuest(model.Status),
                agency_id = GetCurrentAgencyId()
            };

            var listCompanyIds = new List<int>();
            var listCVs = new List<int>();
            try
            {
                if (model.company_id > 0)
                {
                    listCompanyIds.Add(model.company_id);

                    var companyResult = CompanyServices.GetDetailAsync(new ApiCompanyModel { id = model.company_id }).Result;
                    if (companyResult != null)
                    {
                        if (companyResult.value != null)
                        {
                            IdentityCompany companyInfo = JsonConvert.DeserializeObject<IdentityCompany>(companyResult.value.ToString());
                            if (companyInfo != null)
                            {
                                if (companyInfo.agency_id != GetCurrentAgencyId())
                                {
                                    return RedirectToErrorPage();
                                }

                                model.CompanyInfo = companyInfo;
                                if (companyInfo.LangList.HasData())
                                {
                                    model.company_name = companyInfo.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.company_name).FirstOrDefault();
                                }

                                if (string.IsNullOrEmpty(model.company_name))
                                    model.company_name = companyInfo.company_name;
                            }
                            else
                            {
                                return RedirectToErrorPage();
                            }
                        }
                    }
                }

                var result = AgencyServices.GetInvitationsAsync(filter).Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityInvitation>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        model.TotalCount = Utils.ConvertToInt32(result.total);
                        //model.Industries = CommonHelpers.GetListIndustries();
                        if (model.company_id <= 0)
                        {
                            var resultListCompanyIds = model.SearchResults.Where(x => x.job_info != null).Select(x => x.job_info.company_id).ToList();
                            listCompanyIds.AddRange(resultListCompanyIds);
                        }

                        listCVs = model.SearchResults.Where(x => x.cv_id > 0).Select(x => x.cv_id).ToList();
                    }
                }

                if (listCompanyIds.HasData())
                {
                    var companyReturnApi = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCompanyIds }).Result;
                    if (companyReturnApi != null && companyReturnApi.value != null)
                        model.Companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companyReturnApi.value.ToString());

                    if (model.company_id > 0 && model.Companies.HasData())
                    {
                        model.CompanyInfo = model.Companies.Where(x => x.id == model.company_id).FirstOrDefault();
                    }
                }

                if (listCVs.HasData())
                {
                    var cvReturnApi = CvServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCVs }).Result;
                    if (cvReturnApi != null && cvReturnApi.value != null)
                        model.CVs = JsonConvert.DeserializeObject<List<IdentityCv>>(cvReturnApi.value.ToString());
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