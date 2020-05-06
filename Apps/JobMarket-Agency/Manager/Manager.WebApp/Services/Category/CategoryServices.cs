﻿//using Manager.ShareLibs;
//using Manager.SharedLibs.Logging;
//using Manager.WebApp.Models;
//using Manager.WebApp.Resources;
//using Manager.WebApp.Settings;
//using Newtonsoft.Json;
//using System;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;

//namespace Manager.WebApp.Services
//{
//    public class CategoryServices
//    {
//        private static readonly ILog logger = LogProvider.For<CategoryServices>();

//        public static ResponseApiModel ClearCategoryCache()
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.HaloSocialApi, "api/category/clearcache");
//            try
//            {
//                var client = new HttpClient();
//                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExtenalSeviceTimeOutInSeconds);

//                // We want the response to be JSON.
//                client.DefaultRequestHeaders.Accept.Clear();
//                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                //Authorization
//                client.AuthorizationHttpClientSocial();

//                //Begin calling
//                var response = new HttpResponseMessage();

//                //StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

//                // Post to the Server and parse the response.
//                response = client.GetAsync(_baseUrl).Result;

//                // Parsing the returned result                    
//                var responseString = response.Content.ReadAsStringAsync().Result;
//                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

//                //Trace log
//                HttpStatusCodeTrace(response);

//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to ClearCategoryCacheAsync - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }

//            return result;
//        }

//        private static void HttpStatusCodeTrace(HttpResponseMessage response)
//        {
//            var statusCode = (int)response.StatusCode;
//            if (statusCode != (int)HttpStatusCode.OK && statusCode != 1)
//            {
//                logger.Debug(string.Format("Return code: {0}, message: {1}", response.StatusCode, response.RequestMessage.RequestUri));
//            }
//        }
//    }
//}