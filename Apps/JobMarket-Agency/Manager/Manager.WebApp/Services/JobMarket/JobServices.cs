using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Manager.WebApp.Models;
using System.Threading.Tasks;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Settings;
using Manager.WebApp.Caching;
using Manager.WebApp.Resources;
using Manager.ShareLibs;
using Manager.SharedLibs;
using System.Collections.Specialized;
using Manager.WebApp.Helpers;

namespace Manager.WebApp.Services
{
    public class JobServices
    {
        private static readonly ILog logger = LogProvider.For<JobServices>();

        public static async Task<ApiResponseCommonModel> Admin_GetDetailAsync(ApiJobModel model)
        {
            var currentAction = "GetDetailAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/admin/jobs/{1}", SystemSettings.MainApi, model.id);

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

        public static async Task<ApiResponseCommonModel> GetListProcessAsync(ApiJobModel model)
        {
            var currentAction = "GetListProcessAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/jobs/{1}/process", SystemSettings.MainApi, model.company_id);

            var myParams = new NameValueCollection
            {
                {"agency_id", model.agency_id.ToString() },
                {"sub_id", model.sub_id.ToString() },
                {"keyword", model.keyword},
                {"page_index", model.page_index.ToString() },
                {"page_size", model.page_size.ToString() },
                {"status", model.status.ToString() },
                {"staff_id", model.staff_id.ToString() },
                {"translate_status", model.translate_status.ToString() }
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
        public static async Task<ApiResponseCommonModel> GetDetailAsync(ApiJobModel model)
        {
            var currentAction = "GetDetailAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/jobs/{1}", SystemSettings.MainApi, model.id);

            try
            {
                //logger.Debug(string.Format("Begin PUT {0} request", currentAction));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                if(!string.IsNullOrEmpty(model.language_code))
                {
                    client.DefaultRequestHeaders.Add("Accept-Language", model.language_code);
                }
                else
                {
                    client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                }
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
        public static async Task<ApiResponseCommonModel> GetCounterForDeletionAsync(ApiJobDeleteModel model)
        {
            var currentAction = "GetCounterForDeletionAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/counter_for_deletion");

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
        public static async Task<ApiResponseCommonModel> UpdateTranslationAsync(ApiJobUpdateTranslationModel model)
        {
            var currentAction = "UpdateTranslationAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/update_translation");

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
        public static async Task<ApiResponseCommonModel> CreateAsync(ApiJobUpdateModel model)
        {
            var currentAction = "CreateAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs");

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

        public static async Task<ApiResponseCommonModel> UpdateAsync(ApiJobUpdateModel model)
        {
            var currentAction = "UpdateAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/update");

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

        public static async Task<ApiResponseCommonModel> DeleteAsync(ApiJobDeleteModel model)
        {
            var currentAction = "DeleteAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs");

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
        public static async Task<ApiResponseCommonModel> CloseAsync(int id)
        {
            var currentAction = "DeleteAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}/{2}/close", SystemSettings.MainApi, "api/jobs", id);

            try
            {
                var rawData = JsonConvert.SerializeObject(id);
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
        public static async Task<ApiResponseCommonModel> GetListAssignmentWorkByPageAsync(ApiJobModel model)
        {
            var currentAction = "GetJobPageAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}/get_list_assignment_work", SystemSettings.MainApi, "api/jobs");
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

        //public static async Task<ApiResponseCommonModel> GetRecentByPageAsync(ApiJobGetRecentModel model)
        //{
        //    var currentAction = "GetRecentByPageAsync";
        //    var strError = string.Empty;
        //    var result = new ApiResponseCommonModel();
        //    var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/recent_jobs");

        //    var rawData = JsonConvert.SerializeObject(model);
        //    try
        //    {
        //        //logger.Debug(string.Format("Begin GET {0} request", currentAction));
        //        logger.Debug(string.Format("Raw data: {0}", rawData));

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

        //        StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

        //        // Post to the Server and parse the response.
        //        response = client.PostAsync(_baseUrl, theContent).Result;

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

        //public static async Task<ApiResponseCommonModel> SaveJobAsync(ApiSaveJobModel model)
        //{
        //    var currentAction = "SaveJobAsync";
        //    var strError = string.Empty;
        //    var result = new ApiResponseCommonModel();
        //    var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/saved_jobs");

        //    var rawData = JsonConvert.SerializeObject(model);
        //    try
        //    {
        //        //logger.Debug(string.Format("Begin GET {0} request", currentAction));
        //        logger.Debug(string.Format("Raw data: {0}", rawData));

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

        //        StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

        //        // Post to the Server and parse the response.
        //        response = client.PostAsync(_baseUrl, theContent).Result;

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

        //public static async Task<ApiResponseCommonModel> UnSaveJobAsync(ApiSaveJobModel model)
        //{
        //    var currentAction = "UnSaveJobAsync";
        //    var strError = string.Empty;
        //    var result = new ApiResponseCommonModel();
        //    var _baseUrl = string.Format("{0}/{1}/{2}/{3}", SystemSettings.MainApi, "api/saved_jobs", model.job_id, model.job_seeker_id);

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
        //        response = client.DeleteAsync(_baseUrl).Result;

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

        //public static async Task<ApiResponseCommonModel> GetDetailAsync(ApiJobGetDetailModel model)
        //{
        //    var currentAction = "GetDetailAsync";
        //    var strError = string.Empty;
        //    var result = new ApiResponseCommonModel();
        //    var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/detail");

        //    var rawData = JsonConvert.SerializeObject(model);
        //    try
        //    {
        //        //logger.Debug(string.Format("Begin GET {0} request", currentAction));
        //        logger.Debug(string.Format("Raw data: {0}", rawData));

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

        //        StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

        //        // Post to the Server and parse the response.
        //        response = client.PostAsync(_baseUrl, theContent).Result;

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

        public static async Task<ApiResponseCommonModel> ApplicationInviteAsync(ApiJobInvitationModel model)
        {
            var currentAction = "ApplicationInviteAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/application_invite");

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

        public static async Task<ApiResponseCommonModel> ApplicationInviteAsync(ApiCvInvitationModel model)
        {
            var currentAction = "ApplicationInviteAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/application_invite");

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

        public static async Task<ApiResponseCommonModel> ApplicationInviteJobSeekerAsync(ApiCvInvitationModel model)
        {
            var currentAction = "ApplicationInviteAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/application_invite_job_seeker");

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

        public static async Task<ApiResponseCommonModel> ApplicationInviteMultiJobsAsync(ApiCvInvitationModel model)
        {
            var currentAction = "ApplicationInviteMultiJobsAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/application_invite_jobs");

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

        public static async Task<ApiResponseCommonModel> InvitationCheckingAsync(ApiJobInvitationModel model)
        {
            var currentAction = "ApplicationInviteAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/invitation_checking");

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

        public static async Task<ApiResponseCommonModel> InvitationCheckingAsync(ApiCvInvitationModel model)
        {
            var currentAction = "ApplicationInviteAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/invitation_checking");

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