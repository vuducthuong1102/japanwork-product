﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using MySite.Logging;
using MySite.Settings;
using MySite.Resources;
using MySite.Models;
using MySite.Caching;
using MySite.ShareLibs.Exceptions;
using System.Threading.Tasks;
using MySite.ShareLibs;

namespace MySite.Services
{
    public class InvitationServices
    {
        private static readonly ILog logger = LogProvider.For<InvitationServices>();

        public static async Task<ApiResponseCommonModel> AcceptAsync(ApiInvitationActionModel model)
        {
            var currentAction = "AcceptAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/invitations/accepts");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));
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

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        public static async Task<ApiResponseCommonModel> IgnoreAsync(ApiInvitationActionModel model)
        {
            var currentAction = "IgnoreAsync";
            var strError = string.Empty;
            var result = new ApiResponseCommonModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/invitations/ignorances");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                //logger.Debug(string.Format("Begin GET {0} request", currentAction));
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

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }
            finally
            {
                //logger.Debug(string.Format("End {0} request", currentAction));
            }

            return result;
        }

        private static void HttpStatusCodeTrace(HttpResponseMessage response)
        {
            var statusCode = Utils.ConvertToInt32(response.StatusCode);
            if (statusCode != (int)HttpStatusCode.OK)
            {
                logger.Error(string.Format("Return code: {0}, message: {1}", response.StatusCode, response.RequestMessage.RequestUri));
            }
        }
    }
}