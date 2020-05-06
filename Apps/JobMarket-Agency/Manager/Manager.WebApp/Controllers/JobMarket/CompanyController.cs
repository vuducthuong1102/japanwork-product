using System;
using System.Collections.Generic;
using System.Net;
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
using Autofac;
using MsSql.AspNet.Identity;

namespace Manager.WebApp.Controllers
{
    public class CompanyController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<CompanyController>();

        public CompanyController()
        {

        }
        
        public ActionResult Index(ManageCompanyModel model)
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

            var filter = new ApiCompanyModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = model.CurrentPage,
                page_size = model.PageSize,
                agency_id = GetCurrentAgencyId(),
                staff_id = model.staff_id,
                prefecture_id = model.prefecture_id,
                ishiring = model.ishiring == true ? 1 : 0,
                sub_industry_id = model.sub_industry_id
            };
            model.agency_id = GetCurrentAgencyId();

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
                model.Industries = CommonHelpers.GetListIndustries();
                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());

                var result = AgencyServices.GetCompaniesAsync(filter).Result;

                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCompany>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        model.TotalCount = Utils.ConvertToInt32(result.total);

                        var prefectureIds = model.SearchResults.Where(s => s.prefecture_id > 0).GroupBy(p => p.prefecture_id).Select(s => s.Key).ToList();
                        var cityIds = model.SearchResults.Where(s => s.city_id > 0).GroupBy(p => p.city_id).Select(s => s.Key).ToList();
                        var stationIds = new List<int>();

                        model.Prefectures = CommonHelpers.GetListPrefectures(prefectureIds);
                        model.Citys = CommonHelpers.GetListCities(cityIds);
                        foreach (var item in model.SearchResults)
                        {
                            item.address_info = new IdentityAddress();
                            if (model.Prefectures.HasData())
                            {
                                var prefecInfo = model.Prefectures.FirstOrDefault(s => s.id == item.prefecture_id);
                                if (prefecInfo != null) item.address_info.prefecture_info = prefecInfo;
                            }

                            if (model.Citys.HasData())
                            {
                                var cityInfo = model.Citys.FirstOrDefault(s => s.id == item.city_id);
                                if (cityInfo != null) item.address_info.city_info = cityInfo;
                            }
                        }
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
        public ActionResult SearchRender()
        {
            ManageCompanyModel model = new ManageCompanyModel();
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
                model.Industries = CommonHelpers.GetListIndustries();
                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
            }
            catch (Exception ex)
            {
                logger.Error("Failed to get PopupSearch: " + ex.ToString());
            }

            return PartialView("Partials/_PopupSearch", model);
        }

        [HttpPost]
        public ActionResult GetJobsByCompany()
        {
            ManageJobModel model = new ManageJobModel();
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (Request["CurrentJobListTabPageIdx"] != null)
                currentPage = Utils.ConvertToInt32(Request["CurrentJobListTabPageIdx"], 1);

            if (Request["id"] != null)
                model.company_id = Utils.ConvertToInt32(Request["id"], 1);
            var filter = new ApiJobModel
            {
                page_index = currentPage,
                page_size = pageSize,
                company_id = model.company_id,
                status = -99,
                agency_id = GetCurrentAgencyId(),
                sub_id = 0,
                translate_status = -1
            };

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
                                model.Status = companyInfo.status;
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
                        else
                        {
                            return RedirectToErrorPage();
                        }
                    }
                }
                var result = CompanyServices.GetJobsAsync(filter).Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJob>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        model.TotalCount = Utils.ConvertToInt32(result.total);
                        model.CurrentPage = currentPage;
                        model.PageSize = pageSize;
                        model.Industries = CommonHelpers.GetListIndustries();

                        //if (model.company_id <= 0)
                        //{
                        //    var listCompanyIds = model.SearchResults.Select(x => x.company_id).ToList();

                        //    var companyReturnApi = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCompanyIds }).Result;
                        //    if (companyReturnApi != null && companyReturnApi.value != null)
                        //        model.Companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companyReturnApi.value.ToString());
                        //}
                    }
                }
                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.SubFields = CommonHelpers.GetListSubFields();
            }
            catch (Exception ex)
            {
                logger.Error("Failed to get JobList: " + ex.ToString());
            }

            return PartialView("../Company/Partials/_JobList", model);
        }


        [HttpPost]
        [AccessRoleChecker]
        public ActionResult UpdateUser(List<int> selectUser, int company_id)
        {
            var strError = string.Empty;
            try
            {
                if (selectUser.HasData())
                    selectUser.Add(GetCurrentAgencyId());

                var model = new ApiCompanyUpdateAgencyModel
                {
                    company_id = company_id,
                    ListAgencyId = selectUser,
                    agency_parent_id = GetCurrentAgencyId()
                };
                var result = AgencyCompanyServices.UpdateAgencyAsync(model).Result;
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not UpdateUser because: {0}", ex.ToString());
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }
            this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);

            return RedirectToAction("Index", "Company");
        }

        [HttpGet]
        public JsonResult GetUserManagers(int id)
        {
            var myStore = GlobalContainer.IocContainer.Resolve<IIdentityStore>();
            var listUser = myStore.GetListByParentId(GetCurrentStaffId());
            var listResult = AgencyCompanyServices.GetListAgencysByIdAsync(id).Result;
            if (listResult != null && listResult.value != null)
            {
                var listSelected = JsonConvert.DeserializeObject<List<int>>(listResult.value.ToString());
                List<SelectListItem> listItem = new List<SelectListItem>();
                if (listUser.HasData())
                {

                    foreach (var record in listUser)
                    {
                        var item = new SelectListItem();
                        item.Value = record.StaffId.ToString();
                        item.Text = record.FullName;
                        if (listSelected != null && listSelected.Count > 0)
                        {
                            foreach (var select in listSelected)
                            {
                                if (record.StaffId == select)
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                        listItem.Add(item);
                    }
                }

                return Json(new { data = listItem }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { data = new List<SelectListItem>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [PreventCrossOrigin]
        [IsValidURLRequest]
        public ActionResult SendEmail()
        {
            ManageEmailSendingModel model = new ManageEmailSendingModel();
            try
            {
                model.company_id = Utils.ConvertToInt32(Request["company_id"]);
                model.receiver = Request["receiver"] != null ? Request["receiver"].ToString() : string.Empty;
                model.tk = Request["tk"];
                model.controller_name = "Company";
                model.action_name = "SendEmail";
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get SendEmail because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("../MyEmail/Sub/_PopupComposer", model);
        }

        [HttpGet]
        public JsonResult GetCompanyManagers(int id)
        {
            var model = new ApiCompanyModel
            {
                agency_id = GetCurrentAgencyId(),
                page_index = 1,
                page_size = 1000
            };
            var listDataCompany = AgencyServices.GetCompaniesAsync(model).Result;
            List<IdentityCompany> listCompany = JsonConvert.DeserializeObject<List<IdentityCompany>>(listDataCompany.value.ToString());
            var listResult = AgencyCompanyServices.GetListCompanysByIdsAsync(id).Result;
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
                        if (listSelected != null && listSelected.Count > 0)
                        {
                            foreach (var select in listSelected)
                            {
                                if (record.id == select)
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                        listItem.Add(item);
                    }
                }

                return Json(new { data = listItem }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { data = new List<SelectListItem>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [IsValidURLRequest]
        public ActionResult AssignToCompany(int id)
        {
            var apiModel = new ApiCompanyModel
            {
                agency_id = GetCurrentAgencyId(),
                page_index = 1,
                page_size = 1000
            };

            AssignStaffToCompanyModel model = new AssignStaffToCompanyModel();
            model.allCompanies = new List<SelectListItem>();
            model.staff_id = id;
            try
            {
                var listDataCompany = AgencyServices.GetCompaniesAsync(apiModel).Result;

                List<IdentityCompany> listCompany = JsonConvert.DeserializeObject<List<IdentityCompany>>(listDataCompany.value.ToString());
                var listResult = AgencyCompanyServices.GetListCompanysByIdsAsync(id).Result;

                if (listResult != null && listResult.value != null)
                {
                    var listSelected = JsonConvert.DeserializeObject<List<int>>(listResult.value.ToString());
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
                            if (listSelected != null && listSelected.Count > 0)
                            {
                                foreach (var select in listSelected)
                                {
                                    if (record.id == select)
                                    {
                                        item.Selected = true;
                                    }
                                }
                            }
                            model.allCompanies.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Could not show AssignToCompany because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_AssignToCompany", model);
        }

        [HttpPost]
        [ActionName("AssignToCompany")]
        [ValidateAntiForgeryToken]
        public ActionResult AssignToCompany_Post(AssignStaffToCompanyModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            try
            {
                var apiModel = new ApiCompanyUpdateCompanyModel
                {
                    agency_id = model.staff_id,
                    ListCompanyId = model.selectCompany,
                    agency_parent_id = GetCurrentAgencyId()
                };
                var result = AgencyCompanyServices.UpdateCompanyAsync(apiModel).Result;
                isSuccess = true;

                return Json(new { success = isSuccess, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
            }
            catch (Exception ex)
            {
                var strError = string.Format("Could not AssignToCompany because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, message = message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        [HttpPost]
        //[AccessRoleChecker]
        public ActionResult UpdateCompany(List<int> selectCompany, int agency_id)
        {
            var strError = string.Empty;
            try
            {
                var model = new ApiCompanyUpdateCompanyModel
                {
                    agency_id = agency_id,
                    ListCompanyId = selectCompany,
                    agency_parent_id = GetCurrentAgencyId()
                };
                var result = AgencyCompanyServices.UpdateCompanyAsync(model).Result;
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not UpdateCompany because: {0}", ex.ToString());
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }

            this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
            return RedirectToAction("Index", "UsersAdmin");
        }

        [AccessRoleChecker]
        public ActionResult Create()
        {
            var model = new CompanyEditModel();
            try
            {
                model.Countries = CommonHelpers.GetListCountries();
                if (model.Countries.HasData())
                {
                    model.Address.country_id = (int)EnumCountry.Japan;
                }
                model.agency_id = GetCurrentStaffId();
                model.Industries = CommonHelpers.GetListIndustries();
                model.CompanySizes = CommonHelpers.GetListCompanySizes();
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display CreateCompany because: {0}", ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Company(CompanyEditModel model)
        {
            try
            {
                //Begin create
                var apiCompanyModel = ExtractCompanyFormData(model);
                if (model.image_file_upload != null)
                {
                    var apiUploadReturned = CompanyServices.UploadImageAsync(apiCompanyModel, model.image_file_upload).Result;
                    if (apiUploadReturned != null)
                    {
                        if (apiUploadReturned.value != null)
                        {
                            List<FileUploadResponseModel> images = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiUploadReturned.value.ToString());
                            if (images.HasData())
                            {
                                apiCompanyModel.logo_path = images[0].Path;
                            }
                        }
                    }
                }

                var apiReturned = CompanyServices.UpdateAsync(apiCompanyModel).Result;
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

                logger.Error("Failed to CreateCompany because: " + ex.ToString());
            }

            //return RedirectToAction("Index");

            var returnUrl = "/Company" + GetPreviousPageQueryParams();
            return Redirect(returnUrl);
        }

        [IsValidURLRequest]
        public ActionResult Detail(int? id)
        {
            ModelState.Clear();
            var companyId = Utils.ConvertToIntFromQuest(id);
            var model = new CompanyEditModel();
            try
            {
                if (companyId <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var apiModel = new ApiCompanyModel();
                apiModel.id = Utils.ConvertToIntFromQuest(id);

                var result = CompanyServices.GetDetailAsync(apiModel).Result;
                if (result != null && result.value != null)
                {
                    IdentityCompany info = JsonConvert.DeserializeObject<IdentityCompany>(result.value.ToString());

                    model = ParseDataToForm(info);
                    model.address_full = FrontendHelpers.GenerateCompanyAddress(info, GetCurrentLanguageOrDefault());
                }
                model.agency_id = GetCurrentAgencyId();
                model.id = companyId;
                model.Countries = CommonHelpers.GetListCountries();
                model.Industries = CommonHelpers.GetListIndustries();
                model.CompanySizes = CommonHelpers.GetListCompanySizes();

            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display DetailCompany [{0}] because: {1}", id, ex.ToString()));
            }

            return View(model);
        }

        [IsValidURLRequest]
        public ActionResult Edit(int? id)
        {
            ModelState.Clear();
            var companyId = Utils.ConvertToIntFromQuest(id);
            var model = new CompanyEditModel();
            try
            {
                if (companyId <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var apiModel = new ApiCompanyModel();
                apiModel.id = Utils.ConvertToIntFromQuest(id);

                var result = CompanyServices.GetDetailAsync(apiModel).Result;
                if (result != null && result.value != null)
                {
                    var info = JsonConvert.DeserializeObject<IdentityCompany>(result.value.ToString());
                    model = ParseDataToForm(info);
                }
                model.agency_id = GetCurrentAgencyId();
                model.id = companyId;
                model.Countries = CommonHelpers.GetListCountries();
                model.Industries = CommonHelpers.GetListIndustries();
                model.CompanySizes = CommonHelpers.GetListCompanySizes();
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditCompany [{0}] because: {1}", id, ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Company(CompanyEditModel model)
        {
            try
            {
                //Begin update
                var apiCompanyModel = ExtractCompanyFormData(model);
                if (model.image_file_upload != null)
                {
                    var apiUploadReturned = CompanyServices.UploadImageAsync(apiCompanyModel, model.image_file_upload).Result;
                    if (apiUploadReturned != null)
                    {
                        if (apiUploadReturned.value != null)
                        {
                            List<FileUploadResponseModel> images = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiUploadReturned.value.ToString());
                            if (images.HasData())
                            {
                                apiCompanyModel.logo_path = images[0].Path;
                            }
                        }
                    }
                }

                var apiReturned = CompanyServices.UpdateAsync(apiCompanyModel).Result;
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
                            this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to EditCompany because: " + ex.ToString());
            }

            //var secureLink = SecurityHelper.GenerateSecureLink("Company", "Edit", new { id = model.id });
            //return Redirect(secureLink);

            var returnUrl = "/Company" + GetPreviousPageQueryParams();
            return Redirect(returnUrl);
        }


        //public ActionResult Delete(string ids)
        //{
        //    if (string.IsNullOrEmpty(ids))
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    var model = new CompanyDeleteModel();

        //    try
        //    {
        //        model.Ids = ids;
        //        model.tk = SecurityHelper.GenerateUrlToken("Company", "Delete", new { Ids = ids });
        //    }
        //    catch
        //    {
        //    }

        //    return PartialView("_PopupDelete", model);
        //}

        [IsValidURLRequest]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return RedirectToErrorPage();
            }

            var model = new CompanyClearDataModel();

            try
            {
                var agency_id = GetCurrentAgencyId();
                model.company_id = id;
                model.tk = SecurityHelper.GenerateUrlToken("Company", "Delete", new { company_id = id });

                var apiModel = new ApiCompanyModel();
                apiModel.id = id;

                var result = CompanyServices.GetDetailAsync(apiModel).Result;
                model.CompanyIno = result.ConvertData<IdentityCompany>();

                if (model.CompanyIno == null)
                {
                    return RedirectToErrorPage();
                }

                var cResult = CompanyServices.GetCounterForDeletionAsync(
                        new ApiCompanyDeleteModel { agency_id = agency_id, company_id = id }
                    ).Result;

                model.Counter = cResult.ConvertData<IdentityCompanyCounter>();

            }
            catch (Exception ex)
            {
                logger.Error("Failed to get Delete because: " + ex.ToString());

                return RedirectToErrorPage();
            }

            return View("ClearData", model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Confirm(CompanyClearDataModel model)
        {
            var strError = string.Empty;

            if (model.company_id <= 0)
            {
                return RedirectToErrorPage();
            }

            if (string.IsNullOrEmpty(model.tk))
            {
                return RedirectToErrorPage();
            }
            else
            {
                var tk = SecurityHelper.GenerateUrlToken("Company", "Delete", new { id = model.company_id });
                if (model.tk != tk)
                    return RedirectToErrorPage();
            }

            try
            {
                var result = CompanyServices.DeleteAsync(new ApiCompanyDeleteModel { ids = model.company_id.ToString(), staff_id = GetCurrentStaffId() }).Result;
                if (result != null)
                {
                    if (result.status == (int)HttpStatusCode.OK)
                    {
                        if (result.error != null && !string.IsNullOrEmpty(result.error.error_code))
                        {
                            this.AddNotification(result.error.message, NotificationType.ERROR);
                            return View("ClearData", model);
                        }
                        else
                        {
                            if (!Utils.ConvertToBoolean(result.value))
                            {
                                this.AddNotification(ManagerResource.COMPANY_DELETE_ERROR, NotificationType.ERROR);
                                return View("ClearData", model);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;
                logger.Error("Failed to get Delete Post because: " + ex.ToString());
                this.AddNotification(ManagerResource.COMPANY_DELETE_ERROR, NotificationType.ERROR);

                return View("ClearData", model);
            }

            this.AddNotification(ManagerResource.LB_DELETE_SUCCESS, NotificationType.SUCCESS);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult GetListForDeletes(int? current_page, int? company_id)
        {
            CompanyClearDataModel model = new CompanyClearDataModel();

            if (current_page == 0)
            {
                current_page = 1;
            }
            model.agency_id = GetCurrentAgencyId();
            model.CurrentPage = current_page ?? 1;
            model.company_id = company_id ?? 0;
            model.PageSize = SystemSettings.DefaultPageSize;

            var filter = new ApiJobModel
            {
                page_index = model.CurrentPage,
                page_size = model.PageSize,
                agency_id = GetCurrentAgencyId(),
                company_id = model.company_id
            };

            try
            {
                var result = CompanyServices.GetJobsForDeleteAsync(filter).Result;
                if (result != null && result.value != null)
                {
                    model.Jobs = JsonConvert.DeserializeObject<List<IdentityJob>>(result.value.ToString());
                    if (model.Jobs.HasData())
                    {
                        model.TotalCount = Utils.ConvertToInt32(result.total);
                    }
                }

                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.SubFields = CommonHelpers.GetListSubFields();
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return PartialView("Partials/_JobsItem", model);
            }

            return PartialView("Partials/_JobsItem", model);
        }

        [IsValidURLRequest]
        public ActionResult DeleteAllJobs(int id)
        {
            if (id <= 0)
            {
                return Content(ManagerResource.COMMON_ERROR_DATA_INVALID);
            }

            var model = new CompanyClearDataModel();

            try
            {
                model.company_id = id;
                model.tk = SecurityHelper.GenerateUrlToken("Company", "DeleteAllJobs", new { id = id });

                var apiModel = new ApiCompanyModel();
                apiModel.id = Utils.ConvertToIntFromQuest(id);

                var result = CompanyServices.GetDetailAsync(apiModel).Result;
                if (result != null && result.value != null)
                {
                    model.CompanyIno = JsonConvert.DeserializeObject<IdentityCompany>(result.value.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to get DeleteAllJobs because: " + ex.ToString());

                return RedirectToErrorPage();
            }

            return PartialView("_PopupDeleteAllJobs", model);
        }

        [HttpPost, ActionName("DeleteAllJobs")]
        [ValidateAntiForgeryToken]
        [IsValidURLRequest]
        public ActionResult DeleteAllJobs_Confirm(CompanyClearDataModel model)
        {
            var strError = string.Empty;

            if (model.company_id <= 0)
            {
                return Json(new { success = false, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_ERROR_OCCURED });
            }

            if (string.IsNullOrEmpty(model.tk))
            {
                return Json(new { success = false, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_ERROR_OCCURED });
            }
            else
            {
                var tk = SecurityHelper.GenerateUrlToken("Company", "DeleteAllJobs", new { id = model.company_id });
                if (model.tk != tk)
                    return Json(new { success = false, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_ERROR_OCCURED });
            }

            try
            {
                var agency_id = GetCurrentAgencyId();
                var result = CompanyServices.DeleteAllJobsAsync(new ApiCompanyDeleteModel { company_id = model.company_id, agency_id = agency_id }).Result;

                if (result != null)
                {
                    if (result.status == (int)HttpStatusCode.OK)
                    {
                        if (result.error != null && !string.IsNullOrEmpty(result.error.error_code))
                        {
                            strError = result.error.message;

                            return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Post because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload();" });
        }

        public ActionResult ClearData()
        {
            var model = new CompanyEditModel();
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
        public ActionResult ClearData_Confirm(CompanyEditModel model)
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

        [HttpPost, ActionName("AssignCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptAssignCategory(int id)
        {
            var msg = ManagerResource.LB_OPERATION_SUCCESS;
            var isSuccess = false;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                //var catId = Utils.ConvertToInt32(Request["cat"]);
                //if (catId > 0 && id > 0)
                //{
                //    isSuccess = _mainStore.AssignCategory(id, catId);

                //    //Clear cache
                //    //FrontendCachingHelpers.ClearCompanyCacheById(id);
                //}
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying AssignCategory: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = "AfterAssign()" });
        }
        #region Note
        public ActionResult ShowNote(int id)
        {
            ManageCompanyNoteModel model = new ManageCompanyNoteModel();
            model.company_id = id;
            model.staff_id = GetCurrentAgencyId();
            model.CurrentPage = 1;
            model.PageSize = SystemSettings.DefaultPageSize;
            var apiModel = new ApiCompanyNoteModel()
            {
                company_id = model.company_id,
                page_index = model.CurrentPage,
                page_size = model.PageSize,
                staff_id = model.staff_id,
                agency_id = GetCurrentAgencyId()
            };
            try
            {
                var resultList = CompanyNoteServices.GetByPageAsync(apiModel).Result;
                if (!string.IsNullOrEmpty(resultList.value.ToString()))
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCompanyNote>>(resultList.value.ToString());
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
            return PartialView("Partials/_Detail", model);
        }

        public ActionResult NoteSearch(ManageCompanyNoteModel model)
        {
            model.PageSize = SystemSettings.DefaultPageSize;
            model.staff_id = GetCurrentStaffId();
            var apiModel = new ApiCompanyNoteModel()
            {
                company_id = model.company_id,
                page_index = model.CurrentPage,
                page_size = model.PageSize,
                staff_id = model.staff_id,
                agency_id = GetCurrentAgencyId()
            };
            try
            {
                var resultList = CompanyNoteServices.GetByPageAsync(apiModel).Result;
                if (!string.IsNullOrEmpty(resultList.value.ToString()))
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCompanyNote>>(resultList.value.ToString());
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
            return PartialView("Partials/_DetailItem", model);
        }
        public ActionResult CreateNote(int company_id)
        {
            var model = new ManageCompanyNoteModel();
            try
            {
                model.company_id = company_id;
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display Create Interview Process because: {0}", ex.ToString()));
            }

            return PartialView("Partials/_CreateNote", model);
        }

        [HttpPost, ActionName("CreateNote")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNote_Post(ManageCompanyNoteModel model)
        {
            try
            {
                //Begin create
                var apiModel = ExtractCompanyNoteFormData(model);
                apiModel.staff_id = GetCurrentStaffId();
                apiModel.agency_id = GetCurrentAgencyId();
                var apiReturned = CompanyNoteServices.InsertAsync(apiModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            return Json(new { success = false, message = apiReturned.error.message, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowNote({0})", model.company_id) });
                        }
                        else
                        {
                            return Json(new { success = true, message = ManagerResource.LB_INSERT_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowNote({0})", model.company_id) });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to CreateCompanyNote because: " + ex.ToString());
            }
            return Json(new { success = true, message = ManagerResource.LB_INSERT_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowNote({0})", model.company_id) });
        }
        public ActionResult EditNote(int id)
        {
            ModelState.Clear();
            var model = new ManageCompanyNoteModel();
            try
            {
                if (id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var resultIvP = CompanyNoteServices.GetDetailAsync(id).Result;
                if (resultIvP != null && resultIvP.value != null)
                {
                    var info = JsonConvert.DeserializeObject<IdentityCompanyNote>(resultIvP.value.ToString());
                    model = ParseDataNoteToForm(info);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display Edit Interview Process [{0}] because: {1}", id, ex.ToString()));
            }

            return PartialView("Partials/_EditNote", model);
        }
        [HttpPost, ActionName("EditNote")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateNote(ManageCompanyNoteModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            try
            {
                //Begin update
                var apiModel = ExtractCompanyNoteFormData(model);
                apiModel.staff_id = GetCurrentStaffId();
                var apiReturned = CompanyNoteServices.UpdateAsync(apiModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowNote({0})", model.company_id) });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(apiReturned.message))
                            {
                                return Json(new { success = true, message = apiReturned.message, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowNote({0})", model.company_id) });
                            }
                            else
                            {
                                return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowNote({0})", model.company_id) });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec UpdateNote because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowNote({0})", model.company_id) });
        }

        public ActionResult DeleteNote(int id, int company_id)
        {
            CompanyNoteDeleteModel model = new CompanyNoteDeleteModel();
            model.id = id;
            model.company_id = company_id;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return PartialView("Partials/_DeleteNote", model);
        }

        [HttpPost, ActionName("DeleteNote")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_ConfirmNote(CompanyNoteDeleteModel model)
        {
            var strError = string.Empty;
            if (model.id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var result = CompanyNoteServices.DeleteAsync(model.id).Result;
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete CompanyNote because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format("ShowNote({0})", model.company_id) });
        }
        private ApiCompanyNoteUpdateModel ExtractCompanyNoteFormData(ManageCompanyNoteModel model)
        {
            var info = new ApiCompanyNoteUpdateModel();

            info.company_id = model.company_id;
            info.id = model.Id;
            info.type = model.type;
            info.note = model.note;

            return info;
        }

        private ManageCompanyNoteModel ParseDataNoteToForm(IdentityCompanyNote info)
        {
            var model = new ManageCompanyNoteModel();
            if (info != null)
            {
                model.Id = info.id;
                model.company_id = info.company_id;
                model.note = info.note;
                model.type = info.type;
            }
            return model;
        }
        #endregion

        #region Helpers

        private CompanyEditModel ParseDataToForm(IdentityCompany info)
        {
            var model = new CompanyEditModel();
            if (info != null)
            {
                model.id = model.id;
                model.company_name = info.company_name;
                model.description = info.description;
                model.email = info.email;
                model.website = info.website;
                model.phone = info.phone;
                model.fax = info.fax;
                model.lat = info.lat;
                model.lng = info.lng;
                model.staff_id = info.staff_id;
                model.address_detail = info.address_detail;
                model.created_at = info.created_at;
                model.address_furigana = info.address_furigana;
                if (info.establish_year != 0)
                {
                    model.establish_year = info.establish_year.ToString();
                }
                model.sub_industry_id = info.sub_industry_id;
                model.logo_path = info.logo_path;
                model.logo_full_path = info.logo_full_path;
                model.company_size_id = info.company_size_id;
                model.pic_id = info.pic_id;

                model.Address.country_id = info.country_id;
                model.Address.region_id = info.region_id;
                model.Address.prefecture_id = info.prefecture_id;
                model.Address.city_id = info.city_id;

                //if (info.LangList.HasData())
                //{
                //    var myLang = info.LangList.Where(x => x.language_code == _currentLanguage).FirstOrDefault();
                //    if (myLang != null)
                //    {
                //        model.company_name = myLang.company_name;
                //        model.description = myLang.description;
                //    }
                //}
            }

            return model;
        }

        private ApiCompanyUpdateModel ExtractCompanyFormData(CompanyEditModel model)
        {
            var info = new ApiCompanyUpdateModel();
            info.agency_id = GetCurrentAgencyId();
            info.id = model.id;
            info.company_name = model.company_name;
            info.description = model.description;
            info.email = model.email;
            info.phone = model.phone;
            info.website = model.website;
            info.fax = model.fax;
            info.address_detail = model.address_detail;
            info.address_furigana = model.address_furigana;
            info.company_size_id = model.company_size_id;
            info.establish_year = Utils.ConvertToInt32(model.establish_year);
            info.sub_industry_id = model.sub_industry_id;
            info.logo_path = model.logo_path;
            info.pic_id = model.pic_id;
            info.staff_id = GetCurrentStaffId();

            info.Address = new ApiAddressInputModel();

            info.Address.country_id = model.Address.country_id;
            info.Address.region_id = model.Address.region_id;
            info.Address.prefecture_id = model.Address.prefecture_id;
            info.Address.city_id = model.Address.city_id;


            return info;
        }

        //private void DeleteImageByUrl(string url)
        //{
        //    try
        //    {
        //        string fullPath = Request.MapPath(url);
        //        if (System.IO.File.Exists(fullPath))
        //        {
        //            System.IO.File.Delete(fullPath);
        //        }
        //    }
        //    catch
        //    {
        //        // Deliberately empty.
        //    }
        //}

        #endregion

    }
}