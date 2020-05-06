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
    public class InvitationController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<InvitationController>();

        public InvitationController()
        {

        }

        [AccessRoleChecker]
        public ActionResult History(ManageInvitationModel model)
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

            var filter = new ApiCvInvitationModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize,
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
                        model.CurrentPage = currentPage;
                        model.PageSize = pageSize;
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

        [IsValidURLRequest]
        [PreventCrossOrigin]
        public ActionResult GetReceivers(ManageInvitationModel model)
        {
            BeginSearchReceivers(model);

            return PartialView("Partials/_InvitationReceivers", model);
        }

        private void BeginSearchReceivers(ManageInvitationModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (Request["CurrentPage"] != null)
                currentPage = Utils.ConvertToInt32(Request["CurrentPage"], 1);

            var apiFilterModel = new ApiCvInvitationModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize,
                company_id = model.company_id,
                status = (model.Status == null) ? -99 : Utils.ConvertToIntFromQuest(model.Status),
                agency_id = GetCurrentAgencyId(),
                job_id = model.job_id,
                invite_id = model.invite_id
            };

            try
            {
                var listCompanyIds = new List<int>();
                var listJobSeekerIds = new List<int>();

                var result = InvitationServices.GetReceiversAsync(apiFilterModel).Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityInvitation>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        model.TotalCount = Utils.ConvertToInt32(result.total);
                        model.CurrentPage = currentPage;
                        model.PageSize = pageSize;

                        //var resultListCompanyIds = model.SearchResults.Where(x => x.job_info != null).Select(x => x.job_info.company_id).ToList();
                        //listCompanyIds.AddRange(resultListCompanyIds);

                        listJobSeekerIds = model.SearchResults.Where(x => x.job_seeker_id > 0).Select(x => x.job_seeker_id).ToList();
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

                if (listJobSeekerIds.HasData())
                {
                    var returApi = JobSeekerServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listJobSeekerIds }).Result;
                    //var cvReturnApi = CvServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCVs }).Result;
                    if (returApi != null && returApi.value != null)
                        model.JobSeekers = JsonConvert.DeserializeObject<List<IdentityJobSeeker>>(returApi.value.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to show Invitation GetReceivers form: " + ex.ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetReceiversSearch(ManageInvitationModel model)
        {
            BeginSearchReceivers(model);

            return PartialView("Partials/_ReceiversList", model);
        }

        #region Helpers

        #endregion

    }
}