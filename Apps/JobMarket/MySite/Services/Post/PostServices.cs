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
//using System.Diagnostics;

//namespace MySite.Services
//{
//    public class PostServices
//    {
//        private static readonly ILog logger = LogProvider.For<PostServices>();

//        public static async Task<ResponseApiModel> GetPostByPageAsync(ApiPostFilterModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/post/getbypage");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                logger.Debug("Begin GET posts request");
//                logger.Debug(string.Format("Raw data: {0}", rawData));

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

//                // Parsing the returned result                    
//                var responseString = await response.Content.ReadAsStringAsync();
//                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

//                //Trace log
//                HttpStatusCodeTrace(response);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to get post - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);

//                //result.Code = EnumCommonCode.Error;
//                //result.Msg = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                //return result;
//            }
//            finally
//            {
//                logger.Debug("End GET posts request");
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> GetDetailAsync(ApiPostDetailModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/post/detail");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                logger.Debug("Begin POST DETAIL request");
//                logger.Debug(string.Format("Raw data: {0}", rawData));

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
             
//                // Parsing the returned result                    
//                var responseString = await response.Content.ReadAsStringAsync();
//                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to get post - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);

//                //result.Code = EnumCommonCode.Error;
//                //result.Msg = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                //return result;
//            }
//            finally
//            {
//                logger.Debug("End POST DETAIL request");
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> GetDetailByCommentIdAsync(ApiPostDetailModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/post/detailbycommentid");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                logger.Debug("Begin detailbycomment request");
//                logger.Debug(string.Format("Raw data: {0}", rawData));

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

//                // Parsing the returned result                    
//                var responseString = await response.Content.ReadAsStringAsync();
//                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to detailbycommentid - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);

//                //result.Code = EnumCommonCode.Error;
//                //result.Msg = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                //return result;
//            }
//            finally
//            {
//                logger.Debug("End detailbycommentid request");
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> GetDetailByCommentReplyIdAsync(ApiPostDetailModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/post/detailbycommentreplyid");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                logger.Debug("Begin detailbycommentreplyid request");
//                logger.Debug(string.Format("Raw data: {0}", rawData));

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

//                // Parsing the returned result                    
//                var responseString = await response.Content.ReadAsStringAsync();
//                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to detailbycommentreplyid - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);

//                //result.Code = EnumCommonCode.Error;
//                //result.Msg = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                //return result;
//            }
//            finally
//            {
//                logger.Debug("End detailbycommentreplyid request");
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> GetBaseInfoAsync(ApiPostDetailModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/post/getbase");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                logger.Debug("Begin GetBaseInfoAsync request");
//                logger.Debug(string.Format("Raw data: {0}", rawData));

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

//                // Parsing the returned result                    
//                var responseString = await response.Content.ReadAsStringAsync();
//                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling GetBaseInfoAsync - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }
//            finally
//            {
//                logger.Debug("End GetBaseInfoAsync request");
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> GetFullInfoAsync(ApiPostDetailModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/post/getfullinfo");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                logger.Debug("Begin GetFullInfoAsync request");
//                logger.Debug(string.Format("Raw data: {0}", rawData));

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

//                // Parsing the returned result                    
//                var responseString = await response.Content.ReadAsStringAsync();
//                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling GetFullInfoAsync - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }
//            finally
//            {
//                logger.Debug("End GetFullInfoAsync request");
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> GetRecentPostAsync(ApiPostFilterModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/post/recent");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                logger.Debug("Begin GET recent posts request");
//                logger.Debug(string.Format("Raw data: {0}", rawData));

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
//                response = await client.PostAsync(_baseUrl, theContent);

//                // Parsing the returned result                    
//                var responseString =  response.Content.ReadAsStringAsync().Result;
//                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

//                //Trace log
//                HttpStatusCodeTrace(response);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to get recent posts - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }
//            finally
//            {
//                logger.Debug("End GET recent posts request");
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> GetTopTravellerAsync(ApiGetTopTravellerModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/user/toptraveller");

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
//                var time = new Stopwatch();
//                time.Start();
//                //Begin calling
//                var response = new HttpResponseMessage();

//                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");
//                // Post to the Server and parse the response.
//                response = client.PostAsync(_baseUrl, theContent).Result;

//                // Parsing the returned result                    
//                var responseString = await response.Content.ReadAsStringAsync();
//                time.Stop();
//                logger.Debug("time recentpost: " + time.ElapsedMilliseconds);
//                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
//                //Trace log
//                HttpStatusCodeTrace(response);

//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to GET TOP TRAVELLER - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }

//            return result;
//        }

//        public static async Task<ResponseApiModel> GetTopByPlaceAsync(ApiGetTopPostByPlaceModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/post/gettopbyplace");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                logger.Debug("Begin GetTopByPlaceAsync request");
//                logger.Debug(string.Format("Raw data: {0}", rawData));

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

//                // Parsing the returned result                    
//                var responseString = await response.Content.ReadAsStringAsync();
//                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

//                //Trace log
//                HttpStatusCodeTrace(response);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to GetTopByPlaceAsync - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }
//            finally
//            {
//                logger.Debug("End GetTopByPlaceAsync request");
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