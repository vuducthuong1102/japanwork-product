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
    public class CompanyNoteServices
    {
        private static readonly ILog logger = LogProvider.For<CompanyNoteServices>();

        public static async Task<ApiResponseCommonModel> GetDetailAsync(int id)
        {
            var currentAction = "GetDetailAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/company_note/{1}", SystemSettings.MainApi, id);

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

        public static async Task<ApiResponseCommonModel> GetByPageAsync(ApiCompanyNoteModel model)
        {
            var currentAction = "GetByPageAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/api/company_note/getbypage", SystemSettings.MainApi);

            try
            {
                var rawData = JsonConvert.SerializeObject(model);
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

       
        public static async Task<ApiResponseCommonModel> InsertAsync(ApiCompanyNoteUpdateModel model)
        {
            var currentAction = "InsertAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/company_note");

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

        public static async Task<ApiResponseCommonModel> UpdateAsync(ApiCompanyNoteUpdateModel model)
        {
            var currentAction = "UpdateAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/company_note");

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

        public static async Task<ApiResponseCommonModel> DeleteAsync(int id)
        {
            var currentAction = "DeleteAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}/{2}", SystemSettings.MainApi, "api/company_note",id);

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
                response = client.DeleteAsync(_baseUrl).Result;

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