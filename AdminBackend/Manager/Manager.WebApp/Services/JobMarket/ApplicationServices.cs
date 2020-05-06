using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Models;
using Manager.WebApp.Settings;
using Manager.WebApp.Caching;
using Manager.WebApp.Services;
using Manager.ShareLibs;
using Manager.WebApp.Resources;
using Manager.SharedLibs;

namespace Manager.WebApp.Services
{
    public class ApplicationServices
    {
        private static readonly ILog logger = LogProvider.For<ApplicationServices>();

        //public static async Task<ApiResponseCommonModel> ApplyJobAsync(ApiJobActionApplyModel model)
        //{
        //    var currentAction = "ApplyJobAsync";
        //    var strError = string.Empty;
        //    var result = new ApiResponseCommonModel();
        //    var _baseUrl = string.Format("{0}/{1}", SystemSettings.MainApi, "api/applications");

        //    var rawData = JsonConvert.SerializeObject(model);
        //    try
        //    {
        //        logger.Debug(string.Format("Begin GET {0} request", currentAction));
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
        //        logger.Debug(string.Format("End {0} request", currentAction));
        //    }

        //    return result;
        //}

        //public static async Task<ApiResponseCommonModel> CancelAsync(ApiApplicationCancelModel model)
        //{
        //    var currentAction = "CancelAsync";
        //    var strError = string.Empty;
        //    var result = new ApiResponseCommonModel();
        //    var _baseUrl = string.Format("{0}/{1}/{2}/cancels", SystemSettings.MainApi, "api/applications", model.id);

        //    var rawData = JsonConvert.SerializeObject(model);
        //    try
        //    {
        //        logger.Debug(string.Format("Begin GET {0} request", currentAction));
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
        //        logger.Debug(string.Format("End {0} request", currentAction));
        //    }

        //    return result;
        //}

        private static void HttpStatusCodeTrace(HttpResponseMessage response)
        {
            var statusCode = Utils.ConvertToInt32(response.StatusCode);
            if (statusCode != (int)HttpStatusCode.OK)
            {
                logger.Debug(string.Format("Return code: {0}, message: {1}", response.StatusCode, response.RequestMessage.RequestUri));
            }
        }
    }
}