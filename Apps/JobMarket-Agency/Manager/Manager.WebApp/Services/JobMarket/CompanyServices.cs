﻿using Manager.ShareLibs;

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
using System.Web;
using Manager.WebApp.Helpers;
using System.Collections.Specialized;
using Manager.SharedLibs;

namespace Manager.WebApp.Services
{
    public class CompanyServices
    {
        private static readonly ILog logger = LogProvider.For<CompanyServices>();

        public static async Task<ApiResponseCommonModel> GetJobsAsync(ApiJobModel model)
        {
            var currentAction = "GetJobsAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/companies/{1}/jobs", SystemSettings.MainApi, model.company_id);

            var myParams = new NameValueCollection
            {
                {"agency_id", model.agency_id.ToString() },
                {"sub_id", model.sub_id.ToString() },
                {"keyword", model.keyword},
                {"page_index", model.page_index.ToString() },
                {"page_size", model.page_size.ToString() },
                {"status", model.status.ToString() },
                {"translate_status", model.translate_status.ToString() },
                {"staff_id", model.staff_id.ToString() },
                {"employment_type_id", model.employment_type_id.ToString() },
                {"prefecture_id", model.prefecture_id.ToString() },
                {"japanese_level_number", model.japanese_level_number.ToString() },
                {"salary_min", model.salary_min.ToString() },
                {"salary_max", model.salary_max.ToString() }
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

        public static async Task<ApiResponseCommonModel> GetJobsForDeleteAsync(ApiJobModel model)
        {
            var currentAction = "GetJobsForDeleteAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/companies/{1}/jobs_for_delete", SystemSettings.MainApi, model.company_id);

            var myParams = new NameValueCollection
            {
                {"agency_id", model.agency_id.ToString() },
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
        public static async Task<ApiResponseCommonModel> DeleteAsync(ApiCompanyDeleteModel model)
        {
            var currentAction = "DeleteAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/companies");

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

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(_baseUrl),
                    Content = theContent
                };

                // Post to the Server and parse the response.
                response = client.SendAsync(request).Result;

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

        public static async Task<ApiResponseCommonModel> DeleteAllJobsAsync(ApiCompanyDeleteModel model)
        {
            var currentAction = "DeleteAllJobsAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/companies/delete_all_jobs");

            try
            {
                var rawData = JsonConvert.SerializeObject(model);
                //logger.Debug(string.Format("Begin DELETE {0} request", currentAction));

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

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(_baseUrl),
                    Content = theContent
                };

                // Post to the Server and parse the response.
                response = client.SendAsync(request).Result;

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

        public static async Task<ApiResponseCommonModel> GetDetailAsync(ApiCompanyModel model)
        {
            var currentAction = "GetDetailAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/companies/{1}", SystemSettings.MainApi, model.id);

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

        public static async Task<ApiResponseCommonModel> GetListByIdsAsync(ApiGetListByIdsModel model)
        {
            var currentAction = "GetListByIdsAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/companies/get_list_by_ids");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
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

            return result;
        }

        public static async Task<ApiResponseCommonModel> GetListActiveByIdsAsync(ApiGetListByIdsModel model)
        {
            var currentAction = "GetListActiveByIdsAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/companies/get_list_active_by_ids");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
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

            return result;
        }

        public static async Task<ApiResponseCommonModel> UploadImageAsync(ApiCompanyUpdateModel model, HttpPostedFileBase image_file_upload = null)
        {
            var currentAction = "UploadImageAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/companies/upload_logo");

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

                        content.Add(new StringContent(model.id.ToString()), "company_id");
                        content.Add(new StringContent(model.agency_id.ToString()), "agency_id");

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

        public static async Task<ApiResponseCommonModel> UpdateAsync(ApiCompanyUpdateModel model)
        {
            var currentAction = "UpdateAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/companies");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                //logger.Debug(string.Format("Begin PUT {0} request", currentAction));
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

        public static async Task<ApiResponseCommonModel> GetCounterForDeletionAsync(ApiCompanyDeleteModel model)
        {
            var currentAction = "GetCounterForDeletionAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/companies/counter_for_deletion");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
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