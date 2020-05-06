using System;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using ApiJobMarket.Logging;
using ApiJobMarket.ActionResults;
using ApiJobMarket.Helpers;
using ApiJobMarket.Models;
using Newtonsoft.Json;
using ApiJobMarket.Helpers.Validation;
using ApiJobMarket.SharedLib.Extensions;
using System.Collections.Generic;
using ApiJobMarket.DB.Sql.Stores;
using Autofac;
using System.Web;
using System.Linq;
using ApiJobMarket.Services;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.Resources;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/certificates")]
    public class ApiCertificateController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiCertificateController>();

        [HttpPost]
        [Route("detail")]
        public async Task<IHttpActionResult> GetDetail(ApiJobSeekerCertificateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Certificates-GetDetail";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                var job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var info = storeCertificate.JobSeekerGetDetail(id, job_seeker_id);
                await Task.FromResult(info);

                returnModel.value = info;
                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> UpdateCertificate(ApiJobSeekerCertificateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Certificates-UpdateCertificate";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var updateInfo = ExtractCertificateFormData(model);

                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var result = storeCertificate.JobSeekerUpdate(updateInfo);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, model.job_seeker_id));

                returnModel.value = result;

                jsonString = JsonConvert.SerializeObject(returnModel);
                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return SuccessUpdatedResult(returnModel);
        }

        [HttpDelete]
        //[Route("job_seeker_delete")]
        [Route("")]
        public async Task<IHttpActionResult> Delete(ApiJobSeekerCertificateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Certificates-Delete";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var info = new IdentityJobSeekerCertificate();
                info.id = Utils.ConvertToIntFromQuest(model.id);
                info.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var result = storeCertificate.JobSeekerDelete(info);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, model.job_seeker_id));

                returnModel.value = result;

                jsonString = JsonConvert.SerializeObject(returnModel);
                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return SuccessDeletedResult(returnModel);
        }

        #region Cv

        [HttpPost]
        [Route("cv_detail")]
        public async Task<IHttpActionResult> CvGetDetail(ApiCvCertificateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Certificates-CvGetDetail";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                var cv_id = Utils.ConvertToIntFromQuest(model.cv_id);

                var info = storeCertificate.CvGetDetail(id, cv_id);
                await Task.FromResult(info);

                returnModel.value = info;
                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [HttpPost]
        [Route("cv_update")]
        public async Task<IHttpActionResult> CvUpdateCertificate(ApiCvCertificateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Certificates-CvUpdateCertificate";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var updateInfo = ExtractCvCertificateFormData(model);

                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var result = storeCertificate.CvUpdate(updateInfo);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cv, model.cv_id));

                returnModel.value = result;

                jsonString = JsonConvert.SerializeObject(returnModel);
                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return SuccessUpdatedResult(returnModel);
        }

        [HttpDelete]
        [Route("cv_delete")]
        public async Task<IHttpActionResult> CvDelete(ApiCvCertificateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Certificates-CvDelete";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var info = new IdentityCvCertificate();
                info.id = Utils.ConvertToIntFromQuest(model.id);
                info.cv_id = Utils.ConvertToIntFromQuest(model.cv_id);

                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var result = storeCertificate.CvDelete(info);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cv, model.cv_id));

                returnModel.value = result;

                jsonString = JsonConvert.SerializeObject(returnModel);
                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return SuccessDeletedResult(returnModel);
        }

        #endregion

        #region Cs

        [HttpPost]
        [Route("cs_detail")]
        public async Task<IHttpActionResult> CsGetDetail(ApiCsCertificateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Certificates-CsGetDetail";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                var cs_id = Utils.ConvertToIntFromQuest(model.cs_id);

                var info = storeCertificate.CsGetDetail(id, cs_id);
                await Task.FromResult(info);

                returnModel.value = info;
                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [HttpPost]
        [Route("cs_update")]
        public async Task<IHttpActionResult> CsUpdateCertificate(ApiCsCertificateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Certificates-CsUpdateCertificate";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var updateInfo = ExtractCsCertificateFormData(model);

                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var result = storeCertificate.CsUpdate(updateInfo);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cs, model.cs_id));

                returnModel.value = result;

                jsonString = JsonConvert.SerializeObject(returnModel);
                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return SuccessUpdatedResult(returnModel);
        }

        [HttpDelete]
        [Route("cs_delete")]
        public async Task<IHttpActionResult> CsDelete(ApiCsCertificateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Certificates-CsDelete";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var info = new IdentityCsCertificate();
                info.id = Utils.ConvertToIntFromQuest(model.id);
                info.cs_id = Utils.ConvertToIntFromQuest(model.cs_id);

                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var result = storeCertificate.CsDelete(info);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cs, model.cs_id));

                returnModel.value = result;

                jsonString = JsonConvert.SerializeObject(returnModel);
                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return SuccessDeletedResult(returnModel);
        }

        #endregion

        #region Helpers

        private IdentityJobSeekerCertificate ExtractCertificateFormData(ApiJobSeekerCertificateModel model)
        {
            var info = new IdentityJobSeekerCertificate();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);
            info.name = model.name;
            info.start_date = Utils.ConvertStringToDateTimeByFormat(model.start_date, "dd-MM-yyyy");
            info.end_date = Utils.ConvertStringToDateTimeByFormat(model.end_date, "dd-MM-yyyy");
            info.status = Utils.ConvertToIntFromQuest(model.status);
            info.point = model.point;
            info.pass = Utils.ConvertToIntFromQuest(model.pass);

            return info;
        }

        private IdentityCvCertificate ExtractCvCertificateFormData(ApiCvCertificateModel model)
        {
            var info = new IdentityCvCertificate();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.cv_id = Utils.ConvertToIntFromQuest(model.cv_id);
            info.name = model.name;
            info.start_date = Utils.ConvertStringToDateTimeByFormat(model.start_date, "dd-MM-yyyy");
            info.end_date = Utils.ConvertStringToDateTimeByFormat(model.end_date, "dd-MM-yyyy");
            info.status = Utils.ConvertToIntFromQuest(model.status);
            info.point = model.point;
            info.pass = Utils.ConvertToIntFromQuest(model.pass);

            return info;
        }

        private IdentityCsCertificate ExtractCsCertificateFormData(ApiCsCertificateModel model)
        {
            var info = new IdentityCsCertificate();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.cs_id = Utils.ConvertToIntFromQuest(model.cs_id);
            info.name = model.name;
            info.start_date = Utils.ConvertStringToDateTimeByFormat(model.start_date, "dd-MM-yyyy");
            info.end_date = Utils.ConvertStringToDateTimeByFormat(model.end_date, "dd-MM-yyyy");
            info.status = Utils.ConvertToIntFromQuest(model.status);
            info.point = model.point;
            info.pass = Utils.ConvertToIntFromQuest(model.pass);

            return info;
        }

        #endregion
    }
}
