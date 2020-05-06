//using System;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using Newtonsoft.Json;
//using Halo.Logging;
//using Halo.Settings;
//using Halo.Resources;
//using Halo.Models;
//using System.Threading.Tasks;
//using MyCloud.SharedLib.Logging;
//using MyCloud.Models;
//using MyCloud.Settings;

//namespace Halo.Services
//{
//    public class MemberServices
//    {
//        private static readonly ILog logger = LogProvider.For<MemberServices>();

//        public static async Task<ResponseApiModel> ActionFollow(NotificationModel model)
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.HaloSocialApi, "api/user/actionfollow");

//            var rawData = JsonConvert.SerializeObject(model);
//            try
//            {
//                var client = new HttpClient();
//                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

//                // We want the response to be JSON.
//                client.DefaultRequestHeaders.Accept.Clear();
//                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
//                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
//                strError = string.Format("Failed when calling api to Action-Follow - {0} because: {1}", _baseUrl, ex.ToString());
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
//                logger.Debug(string.Format("Return code: {0}, message: {1}", response.StatusCode, response.RequestMessage.RequestUri));
//            }
//        }
//    }
//}