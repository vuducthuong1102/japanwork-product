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

namespace Manager.WebApp.Services
{
    public class InterviewProcessServices
    {
        private static readonly ILog logger = LogProvider.For<InterviewProcessServices>();

        public static async Task<ApiResponseCommonModel> GetDetailAsync(int id)
        {
            var currentAction = "GetDetailAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/interviewprocess/{1}", SystemSettings.MainApi, id);

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

        public static async Task<ApiResponseCommonModel> GetByPageAsync(ApiCompanyModel model)
        {
            var currentAction = "GetByPageAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/interviewprocess/{1}", SystemSettings.MainApi, model.id);

            try
            {
                logger.Debug(string.Format("Begin Get {0} request", currentAction));

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

        public static async Task<ApiResponseCommonModel> GetListByCandidateAsync(int candidate_id)
        {
            var currentAction = "GetListByCandidateAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/interviewprocess/get_list_by_id/{1}", SystemSettings.MainApi,candidate_id);

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
        public static async Task<ApiResponseCommonModel> GetListByCandidateIdsAsync(string listcandidateid)
        {
            var currentAction = "GetListByCandidateIdAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/interviewprocess/get_list_candidate_ids/{1}", SystemSettings.MainApi, listcandidateid);

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
        public static async Task<ApiResponseCommonModel> InsertAsync(ApiInterviewProcessInsertModel model)
        {
            var currentAction = "InsertAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/interviewprocess");

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
                response = client.PutAsync(_baseUrl, theContent).Result;
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

        public static async Task<ApiResponseCommonModel> UpdateAsync(ApiInterviewProcessEditModel model)
        {
            var currentAction = "UpdateAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/interviewprocess");

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

        public static async Task<ApiResponseCommonModel> DeleteAsync(ApiInterviewProcessDeleteModel model)
        {
            var currentAction = "DeleteAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/interviewprocess");

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

        //public static async Task<ApiResponseCommonModel> SearchJobByPageAsync(ApiJobSearchModel model)
        //{
        //    var currentAction = "GetJobPageAsync";
        //    var strError = string.Empty;
        //    var result = new ApiResponseCommonModel();
        //    var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/jobs/search");

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