//using System;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using Newtonsoft.Json;
//using MySite.Logging;
//using MySite.Settings;
//using MySite.Resources;
//using MySite.Models;
//using MySite.Caching;
//using MySite.ShareLibs.Exceptions;
//using System.Threading.Tasks;
//using MySite.ShareLibs;
//using MySite.Models.Account;

//namespace MySite.Services
//{
//    public class SearchServices
//    {
//        private static readonly ILog logger = LogProvider.For<SearchServices>();

//        public static async Task<ResponseApiModel> SearchUserAsycn(ApiFilterModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/search/user");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                var client = new HttpClient();
//                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

//                // We want the response to be JSON.
//                client.DefaultRequestHeaders.Accept.Clear();
//                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
//                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                //Authorization
//                client.AuthorizationHttpClientSocial();

//                //Begin calling
//                var response = new HttpResponseMessage();

//                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

//                // Post to the Server and parse the response.
//                response = client.PostAsync(_baseUrl, theContent).Result;
//                if (response.IsSuccessStatusCode)
//                {
//                    response.EnsureSuccessStatusCode();

//                    // Parsing the returned result                    
//                    var responseString = await response.Content.ReadAsStringAsync();

//                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
//                }

//                //Trace log
//                HttpStatusCodeTrace(response);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to SearchUserAsycn - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> SearchCategoryAsycn(ApiFilterModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/search/category");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                var client = new HttpClient();
//                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

//                // We want the response to be JSON.
//                client.DefaultRequestHeaders.Accept.Clear();
//                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
//                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                //Authorization
//                client.AuthorizationHttpClientSocial();

//                //Begin calling
//                var response = new HttpResponseMessage();

//                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

//                // Post to the Server and parse the response.
//                response = client.PostAsync(_baseUrl, theContent).Result;
//                if (response.IsSuccessStatusCode)
//                {
//                    response.EnsureSuccessStatusCode();

//                    // Parsing the returned result                    
//                    var responseString = await response.Content.ReadAsStringAsync();

//                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
//                }

//                //Trace log
//                HttpStatusCodeTrace(response);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to SearchCategoryAsycn - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> SearchPlaceAsycn(ApiFilterModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/search/place");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                var client = new HttpClient();
//                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

//                // We want the response to be JSON.
//                client.DefaultRequestHeaders.Accept.Clear();
//                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
//                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                //Authorization
//                client.AuthorizationHttpClientSocial();

//                //Begin calling
//                var response = new HttpResponseMessage();

//                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

//                // Post to the Server and parse the response.
//                response = client.PostAsync(_baseUrl, theContent).Result;
//                if (response.IsSuccessStatusCode)
//                {
//                    response.EnsureSuccessStatusCode();

//                    // Parsing the returned result                    
//                    var responseString = await response.Content.ReadAsStringAsync();

//                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
//                }

//                //Trace log
//                HttpStatusCodeTrace(response);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to SearchPlaceAsycn - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> SearchDestinationAsycn(ApiFilterModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/search/destination");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                var client = new HttpClient();
//                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

//                // We want the response to be JSON.
//                client.DefaultRequestHeaders.Accept.Clear();
//                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
//                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                //Authorization
//                client.AuthorizationHttpClientSocial();

//                //Begin calling
//                var response = new HttpResponseMessage();

//                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

//                // Post to the Server and parse the response.
//                response = client.PostAsync(_baseUrl, theContent).Result;
//                if (response.IsSuccessStatusCode)
//                {
//                    response.EnsureSuccessStatusCode();

//                    // Parsing the returned result                    
//                    var responseString = await response.Content.ReadAsStringAsync();

//                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
//                }

//                //Trace log
//                HttpStatusCodeTrace(response);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to SearchDestinationAsycn - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> SearchPostAsycn(ApiFilterModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/search/post");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                var client = new HttpClient(new HttpClientHandler
//                {
//                    UseProxy = false
//                });
//                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

//                // We want the response to be JSON.
//                client.DefaultRequestHeaders.Accept.Clear();
//                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
//                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                //Authorization
//                client.AuthorizationHttpClientSocial();

//                //Begin calling
//                var response = new HttpResponseMessage();

//                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

//                // Post to the Server and parse the response.
//                response = client.PostAsync(_baseUrl, theContent).Result;
//                if (response.IsSuccessStatusCode)
//                {
//                    response.EnsureSuccessStatusCode();

//                    // Parsing the returned result                    
//                    var responseString = await response.Content.ReadAsStringAsync();

//                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
//                }

//                //Trace log
//                HttpStatusCodeTrace(response);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to SearchPostAsycn - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }

//            return result;
//        }

//        private static void HttpStatusCodeTrace(HttpResponseMessage response)
//        {
//            var statusCode = Utils.ConvertToInt32(response.StatusCode);
//            if (statusCode != (int)HttpStatusCode.OK)
//            {
//                logger.Error(string.Format("Return code: {0}, message: {1}", response.StatusCode, response.RequestMessage.RequestUri));
//            }
//        }
//    }
//}