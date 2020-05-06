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
using Manager.SharedLibs;
using System.Linq;

namespace Manager.WebApp.Controllers
{
    public class CompanyController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<CompanyController>();

        public CompanyController()
        {

        }

        [AccessRoleChecker]
        public ActionResult Index(ManageCompanyModel model)
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

            var filter = new ApiCompanyModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize
            };

            try
            {
                var result = CompanyServices.GetByPageAsync(filter).Result;
                if(result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCompany>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        model.TotalCount = Utils.ConvertToInt32(result.total);
                        model.CurrentPage = currentPage;
                        model.PageSize = pageSize;
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

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            ModelState.Clear();
            var companyId = Utils.ConvertToIntFromQuest(id);
            var model = new CompanyEditModel();
            try
            {                
                if(companyId <= 0)
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

            return RedirectToAction("Edit/" + model.id);
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
                //var info = _mainStore.GetById(id, UserCookieManager.GetCurrentLanguageOrDefault());

                //if(info != null)
                //{
                //    _mainStore.Delete(id);

                //    //Clear cache
                //    FrontendCachingHelpers.ClearCompanyCateById(id);
                //}                                
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Company because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
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

        //[HttpGet]
        //public ActionResult AssignCategory(int id)
        //{
        //    if (id <= 0)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    AssignCategoryModel model = new AssignCategoryModel();
        //    try
        //    {
        //        model.Id = id;
        //        model.Categories = CommonHelpers.GetListCategory();
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed when trying load AssignCategory: {0}", ex.ToString());
        //        logger.Error(strError);
        //    }

        //    return PartialView("../Company/_AssignCategory", model);
        //}

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

        //[HttpPost]
        //public ActionResult Upload(IEnumerable<HttpPostedFileBase> file, int Id)
        //{
        //    var fileName = string.Empty;
        //    var filePath = string.Empty;
        //    try
        //    {
        //        var imgUrl = string.Empty;
        //        if(Request.Files != null && Request.Files.Count > 0)
        //            fileName = Request.Files[0].FileName;

        //        if (file != null)
        //        {
        //            var uploadedUrls = UploadImages("Company", "0");
        //            if (uploadedUrls.HasData())
        //            {
        //                imgUrl = uploadedUrls[0];
        //            }
        //        }

        //        if (string.IsNullOrEmpty(imgUrl))
        //        {
        //            if (Request["Url"] != null)
        //                imgUrl = Request["Url"].ToString();

        //            imgUrl = RemoveServerUrl(imgUrl);                                      
        //        }

        //        //var imageIdentity = new IdentityCompanyImage();
        //        //var hashFileName = Utility.Md5HashingData(DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetFileNameWithoutExtension(fileName));
        //        //imageIdentity.Id = string.Format("{0}_{1}", Id, hashFileName);
        //        //imageIdentity.CompanyId = Id;
        //        //imageIdentity.Name = System.IO.Path.GetFileName(fileName);
        //        //imageIdentity.Url = imgUrl;

        //        ////Storage to db
        //        //_mainStore.AddNewImage(imageIdentity);

        //        //Clear cache
        //        //FrontendCachingHelpers.ClearCompanyCateById(Id);

        //        //return Json(new { success = true, fileId = imageIdentity.Id }); // success  

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Failed to Upload because: " + ex.ToString());

        //        return Json(new { success = false, errorMessage = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT });
        //    }
        //}

        //[HttpPost]
        //public ActionResult RemoveImage(string Id, string Url, int CompanyId)
        //{
        //    try
        //    {
        //        //var hasRemoved = _mainStore.RemoveImage(Id);

        //        //if (hasRemoved)
        //        //{
        //        //    //Remove physical image
        //        //    //DeleteImageByUrl(Url);

        //        //    //Clear cache
        //        //    FrontendCachingHelpers.ClearCompanyCateById(CompanyId);


        //        //    return Json(new { success = true }); // success
        //        //}
        //        //else
        //        //{
        //        //    logger.Error("Failed to RemoveImage because the SQL Execution has an error");
        //        //    return Json(new { success = false, errorMessage = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT });
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Failed to RemoveImage because: " + ex.ToString());

        //        return Json(new { success = false, errorMessage = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT });
        //    }
        //}

        //[HttpPost]
        //public ActionResult RefreshImageList(int Id)
        //{
        //    //List<IdentityCompanyImage> myImages = null;
        //    //try
        //    //{
        //    //    myImages = _mainStore.GetListImage(Id);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    logger.Error("Failed to RefreshImageList because: " + ex.ToString());
        //    //}

        //    return PartialView("~/Views/Company/Partials/_Images.cshtml", myImages);
        //}

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
                model.address_detail = info.address_detail;
                model.address_furigana = info.address_furigana;
                model.establish_year = info.establish_year;
                model.sub_industry_id = info.sub_industry_id;
                model.logo_path = info.logo_path;
                model.logo_full_path = info.logo_full_path;

                model.Address.country_id = info.country_id;
                model.Address.region_id = info.region_id;
                model.Address.prefecture_id = info.prefecture_id;
                model.Address.city_id = info.city_id;                

                if (info.LangList.HasData())
                {
                    var myLang = info.LangList.Where(x => x.language_code == _currentLanguage).FirstOrDefault();
                    if(myLang != null)
                    {
                        model.company_name = myLang.company_name;
                        model.description = myLang.description;
                    }
                }
            }

            return model;
        }

        private ApiCompanyUpdateModel ExtractCompanyFormData(CompanyEditModel model)
        {
            var info = new ApiCompanyUpdateModel();
            info.agency_id = GetCurrentStaffId();
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
            info.establish_year = model.establish_year;
            info.sub_industry_id = model.sub_industry_id;
            info.logo_path = model.logo_path;

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