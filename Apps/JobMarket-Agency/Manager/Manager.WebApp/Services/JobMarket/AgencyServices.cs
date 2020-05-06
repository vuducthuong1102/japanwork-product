using Manager.ShareLibs;

using Manager.SharedLibs.Logging;
using Manager.WebApp.Caching;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Settings;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Manager.WebApp.Helpers;
using Manager.SharedLibs;
using System.Web;

namespace Manager.WebApp.Services
{
    public class AgencyServices
    {
        private static readonly ILog logger = LogProvider.For<AgencyServices>();

        public static async Task<ApiResponseCommonModel> GetDetailAsync(ApiAgencyModel model)
        {
            var currentAction = "GetDetailAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/agencies/{1}", SystemSettings.MainApi, model.agency_id);

            try
            {
                //logger.Debug(string.Format("Begin PUT {0} request", currentAction));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                // Post to the Server and parse the response.
                response = client.GetAsync(_baseUrl).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        public static async Task<ApiResponseCommonModel> UpdateProfileAsync(ApiAgencyModel model)
        {
            var currentAction = "UpdateProfileAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/agencies/update");

            try
            {
                var rawData = JsonConvert.SerializeObject(model);
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        public static async Task<ApiResponseCommonModel> UploadLogoAsync(ApiAgencyModel model, HttpPostedFileBase image_file_upload = null)
        {
            var currentAction = "UploadImageAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/agencies/upload_logo");

            try
            {
                var rawData = JsonConvert.SerializeObject(model);
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));

                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        if (image_file_upload != null)
                        {
                            byte[] Bytes = new byte[image_file_upload.InputStream.Length + 1];
                            image_file_upload.InputStream.Read(Bytes, 0, Bytes.Length);
                            var fileContent = new ByteArrayContent(Bytes);
                            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = image_file_upload.FileName };
                            content.Add(fileContent);
                        }

                        content.Add(new StringContent(model.agency_id.ToString()), "agency_id");

                        client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                        // We want the response to be JSON.
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        //Authorization
                        client.AuthorizationHttpClientSocial();

                        //Begin calling
                        var response = new HttpResponseMessage();

                        // Post to the Server and parse the response.
                        response = client.PostAsync(_baseUrl, content).Result;

                        // Parsing the returned result                    
                        var responseString = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                        //Trace log
                        HttpStatusCodeTrace(response);
                    }
                }
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }


        public static async Task<ApiResponseCommonModel> GetCompaniesAsync(ApiCompanyModel model)
        {
            var currentAction = "GetCompaniesAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();

            var _baseUrl = string.Format("{0}/api/agencies/{1}/companies", SystemSettings.MainApi, model.agency_id);
            var myParams = new NameValueCollection
            {
                {"keyword", model.keyword},
                {"page_index", model.page_index.ToString() },
                {"page_size", model.page_size.ToString() },
                {"prefecture_id", model.prefecture_id.ToString() },
                {"ishiring", model.ishiring.ToString() },
                {"staff_id", model.staff_id.ToString() },
                {"sub_industry_id", model.sub_industry_id.ToString() }
            };

            var paramStr = Utility.AttachParameters(myParams);

            _baseUrl = _baseUrl + paramStr;

            try
            {
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                //StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.GetAsync(_baseUrl).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        public static async Task<ApiResponseCommonModel> GetApplicationsAsync(ApiApplicationModel model)
        {
            var currentAction = "GetApplicationsAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();

            var _baseUrl = string.Format("{0}/api/agencies/{1}/applications", SystemSettings.MainApi, model.agency_id);
            var myParams = new NameValueCollection
            {
                {"keyword", model.keyword},
                {"status", model.status.ToString() },
                {"company_id", model.company_id.ToString() },
                {"page_index", model.page_index.ToString() },
                {"page_size", model.page_size.ToString() },
                {"staff_id", model.staff_id.ToString() },
                {"cv_id", model.cv_id.ToString() },
                {"job_id", model.job_id.ToString() }
            };

            var paramStr = Utility.AttachParameters(myParams);

            _baseUrl = _baseUrl + paramStr;

            try
            {
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                //StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.GetAsync(_baseUrl).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        public static async Task<ApiResponseCommonModel> GetApplicationsByPageAsync(ApiApplicationModel model)
        {
            var currentAction = "GetApplicationsAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();

            var _baseUrl = string.Format("{0}/api/agencies/applications", SystemSettings.MainApi);


            try
            {
                var rawData = JsonConvert.SerializeObject(model);
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        public static async Task<ApiResponseCommonModel> GetApplicationInvitedAsync(ApiApplicationModel model)
        {
            var currentAction = "GetApplicationInvitedAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();

            var _baseUrl = string.Format("{0}/api/agencies/{1}/application_invited", SystemSettings.MainApi, model.agency_id);
            var myParams = new NameValueCollection
            {
                {"keyword", model.keyword},
                {"status", model.status.ToString() },
                {"company_id", model.company_id.ToString() },
                {"page_index", model.page_index.ToString() },
                {"page_size", model.page_size.ToString() },
                {"staff_id", model.staff_id.ToString() },
                {"cv_id", model.cv_id.ToString() },
                {"job_id", model.job_id.ToString() }
            };

            var paramStr = Utility.AttachParameters(myParams);

            _baseUrl = _baseUrl + paramStr;

            try
            {
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                //StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.GetAsync(_baseUrl).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        public static async Task<ApiResponseCommonModel> GetApplicationOfflineAsync(ApiApplicationModel model)
        {
            var currentAction = "GetApplicationOfflineAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();

            var _baseUrl = string.Format("{0}/api/agencies/{1}/application_offline", SystemSettings.MainApi, model.agency_id);
            var myParams = new NameValueCollection
            {
                {"keyword", model.keyword},
                {"status", model.status.ToString() },
                {"company_id", model.company_id.ToString() },
                {"page_index", model.page_index.ToString() },
                {"page_size", model.page_size.ToString() },
                {"staff_id", model.staff_id.ToString() },
                {"cv_id", model.cv_id.ToString() },
                {"job_id", model.job_id.ToString() }
            };

            var paramStr = Utility.AttachParameters(myParams);

            _baseUrl = _baseUrl + paramStr;

            try
            {
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                //StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.GetAsync(_baseUrl).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }
        public static async Task<ApiResponseCommonModel> GetApplicationRecruitedAsync(ApiApplicationModel model)
        {
            var currentAction = "GetApplicationOfflineAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();

            var _baseUrl = string.Format("{0}/api/agencies/{1}/application_recruited", SystemSettings.MainApi, model.agency_id);
            var myParams = new NameValueCollection
            {
                {"keyword", model.keyword},
                {"status", model.status.ToString() },
                {"company_id", model.company_id.ToString() },
                {"page_index", model.page_index.ToString() },
                {"page_size", model.page_size.ToString() },
                {"staff_id", model.staff_id.ToString() },
                {"cv_id", model.cv_id.ToString() },
                {"job_id", model.job_id.ToString() }
            };

            var paramStr = Utility.AttachParameters(myParams);

            _baseUrl = _baseUrl + paramStr;

            try
            {
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                //StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.GetAsync(_baseUrl).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }
        //public static async Task<ApiResponseCommonModel> GetCandidatesAsync(ApiApplicationModel model)
        //{
        //    var currentAction = "GetCandidatesAsync";
        //    var strError = string.Empty;
        //    var result = new ApiResponseCommonModel();

        //    var _baseUrl = string.Format("{0}/api/agencies/{1}/candidates", SystemSettings.MainApi, model.agency_id);
        //    var myParams = new NameValueCollection
        //    {
        //        {"keyword", model.keyword},
        //        {"status", model.status.ToString() },
        //        {"company_id", model.company_id.ToString() },
        //        {"page_index", model.page_index.ToString() },
        //        {"page_size", model.page_size.ToString() },
        //        {"type_job_seeker", model.type_job_seeker.ToString() },
        //        {"staff_id", model.staff_id.ToString() }
        //    };

        //    var paramStr = Utility.AttachParameters(myParams);

        //    _baseUrl = _baseUrl + paramStr;

        //    try
        //    {
        //        //logger.Debug(string.Format("Begin GET {0} request", currentAction));

        //        var client = new HttpClient();
        //        client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

        //        // We want the response to be JSON.
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        //Authorization
        //        client.AuthorizationHttpClientSocial();

        //        //Begin calling
        //        var response = new HttpResponseMessage();

        //        //StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

        //        // Post to the Server and parse the response.
        //        response = client.GetAsync(_baseUrl).Result;

        //        // Parsing the returned result                    
        //        var responseString = await response.Content.ReadAsStringAsync();
        //        result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

        //        //Trace log
        //        HttpStatusCodeTrace(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
        //        logger.Error(strError);

        //        throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
        //    }
        //    finally
        //    {
        //        //logger.Debug(string.Format("End {0} request", currentAction));
        //    }

        //    return result;
        //}
        public static async Task<ApiResponseCommonModel> GetCandidatesByJobIdAsync(ApiInterviewProcessSearchModel model)
        {
            var currentAction = "GetCandidatesByJobIdAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();

            var _baseUrl = string.Format("{0}/api/agencies/{1}/candidates/by_job_id", SystemSettings.MainApi, model.agency_id);
            var myParams = new NameValueCollection
            {
                {"keyword", model.keyword},
                {"company_id", model.company_id.ToString() },
                {"type_job_seeker", model.type_job_seeker.ToString() },
                {"staff_id", model.staff_id.ToString() },
                {"job_id", model.job_id.ToString() },
                {"page_index", model.page_index.ToString() },
                {"page_size", model.page_size.ToString() },
                {"status", model.status.ToString() }
            };

            var paramStr = Utility.AttachParameters(myParams);

            _baseUrl = _baseUrl + paramStr;

            try
            {
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                //StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.GetAsync(_baseUrl).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        public static async Task<ApiResponseCommonModel> GetInvitationsAsync(ApiCvInvitationModel model)
        {
            var currentAction = "GetInvitationsAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();

            var _baseUrl = string.Format("{0}/api/agencies/{1}/invitations", SystemSettings.MainApi, model.agency_id);
            var myParams = new NameValueCollection
            {
                {"keyword", model.keyword},
                {"status", model.status.ToString() },
                {"company_id", model.company_id.ToString() },
                {"staff_id", model.staff_id.ToString() },
                {"job_id", model.job_id.ToString() },
                {"page_index", model.page_index.ToString() },
                {"page_size", model.page_size.ToString() }
            };

            var paramStr = Utility.AttachParameters(myParams);

            _baseUrl = _baseUrl + paramStr;

            try
            {
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                //StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.GetAsync(_baseUrl).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        public static async Task<ApiResponseCommonModel> CreateAsync(ApiAgencyCreateModel model)
        {
            var currentAction = "CreateAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/agencies/create");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                //logger.Debug(string.Format("Begin POST {0} request", currentAction));
                //logger.Debug(string.Format("Raw data: {0}", rawData));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                //response = client.GetAsync(_baseUrl).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        public static async Task<ApiResponseCommonModel> GetSchedulesByStaffAsync(ApiScheduleByStaffModel model)
        {
            var currentAction = "GetSchedulesByStaffAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/A_agencies/schedules_by_staff");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                logger.Debug(string.Format("Begin {0} request", currentAction));
                //logger.Debug(string.Format("Raw data: {0}", rawData));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        public static async Task<ApiResponseCommonModel> GetTodaySchedulesByStaffAsync(ApiScheduleByStaffModel model)
        {
            var currentAction = "GetSchedulesByStaffAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/A_agencies/todayschedules_by_staff");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                logger.Debug(string.Format("Begin {0} request", currentAction));
                //logger.Debug(string.Format("Raw data: {0}", rawData));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ApiResponseCommonModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        private static void HttpStatusCodeTrace(HttpResponseMessage response)
        {
            var statusCode = (int)response.StatusCode;
            if (statusCode != (int)HttpStatusCode.OK && statusCode != 1)
            {
                logger.Debug(string.Format("Return code: {0}, message: {1}", response.StatusCode, response.RequestMessage.RequestUri));
            }
        }
    }
}