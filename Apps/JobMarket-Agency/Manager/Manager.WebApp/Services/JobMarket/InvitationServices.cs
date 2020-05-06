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

namespace Manager.WebApp.Services
{
    public class InvitationServices
    {
        private static readonly ILog logger = LogProvider.For<InvitationServices>();

        public static async Task<ApiResponseCommonModel> GetReceiversAsync(ApiCvInvitationModel model)
        {
            var currentAction = "GetReceiversAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();

            var _baseUrl = string.Format("{0}/api/invitations/{1}/receivers", SystemSettings.MainApi, model.invite_id);
            var myParams = new NameValueCollection
            {
                {"keyword", model.keyword},
                {"status", model.status.ToString() },
                {"agency_id", model.agency_id.ToString() },
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