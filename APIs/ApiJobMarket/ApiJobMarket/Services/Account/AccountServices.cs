using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ApiJobMarket.Logging;
using ApiJobMarket.Models;
using ApiJobMarket.Settings;
using ApiJobMarket.ShareLibs.Exceptions;
using ApiJobMarket.Resources;
using ApiJobMarket.ShareLibs;
using System.Web;
using System.Web.Http;

namespace ApiJobMarket.Services
{
    public class AccountServices 
    {
        private static readonly ILog logger = LogProvider.For<AccountServices>();

        public static async Task<ResponseApiModel> GetUserProfileAsync(ApiUserModel model, int userId = 0)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.SingleSignOnApi, "api/user/getinfo");
            if (userId != 0)
            {
                model.UserId = userId;
            }
            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);

            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to GET PROFILE - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> SearchUserAsync(ApiFilterModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.SingleSignOnApi, "api/user/search");
            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);

            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to SearchUserAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> GetListUserProfileAsync(ApiListUserInfoModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.SingleSignOnApi, "api/user/getlistprofile");
           
            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);

            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to GetListUserProfileAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> CheckUserTokenAsync(ApiCheckUserTokenModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.SingleSignOnApi, "api/user/checkusertoken");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);

            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to CheckUserTokenAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> RegisterAsync(ApiRegisterModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.SingleSignOnApi, "api/user/register");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", GetCurrentRequestLang());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = await client.PostAsync(_baseUrl, theContent);

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to REGISTER - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseWebAuthLoginModel> LoginWithAsync(ApiAuthLoginWithModel model)
        {
            var strError = string.Empty;
            var result = new ResponseWebAuthLoginModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.SingleSignOnApi, "api/user/loginwith");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", GetCurrentRequestLang());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseWebAuthLoginModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to LOGIN - WITH - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }
        private static void HttpStatusCodeTrace(HttpResponseMessage response)
        {
            var statusCode = Utils.ConvertToInt32((int)response.StatusCode);

            if (statusCode != (int)HttpStatusCode.OK)
            {
                var strContent = response.Content.ReadAsAsync<HttpError>().Result.Message;
                logger.Debug(string.Format("Return code: {0}, URL: {1}, message: {2}", response.StatusCode, response.RequestMessage.RequestUri, strContent));
            }
        }

        public static string GetCurrentRequestLang()
        {
            var lang = "vi-VN";
            if (HttpContext.Current.Request.Headers["Accept-Language"] != null)
            {
                lang = HttpContext.Current.Request.Headers["Accept-Language"].ToString();
            }

            return lang;
        }
    }
}