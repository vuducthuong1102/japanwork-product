
using Manager.SharedLibs;
using Manager.SharedLibs.Logging;
using Manager.ShareLibs;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Manager.WebApp.Services
{
    public class CdnServices
    {
        private static readonly ILog logger = LogProvider.For<CdnServices>();

        public static async Task<ResponseApiModel> UploadImagesAsync(List<HttpPostedFileBase> files, string objectId, string subDir)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.SocialContainerServer, "api/upload/postimages");

            try
            {
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        if (files.HasData())
                        {
                            foreach (var item in files)
                            {
                                byte[] Bytes = new byte[item.InputStream.Length + 1];
                                item.InputStream.Read(Bytes, 0, Bytes.Length);
                                var fileContent = new ByteArrayContent(Bytes);
                                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = item.FileName };
                                content.Add(fileContent);
                            }
                        }

                        //Extends parameters
                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters.Add("ObjectId", objectId);
                        parameters.Add("SubDir", subDir);
                        HttpContent DictionaryItems = new FormUrlEncodedContent(parameters);
                        content.Add(DictionaryItems, "MyFormData");
                        var response = client.PostAsync(_baseUrl, content).Result;

                        //Trace log
                        HttpStatusCodeTrace(response);

                        // Parsing the returned result                    
                        var responseString = await response.Content.ReadAsStringAsync();

                        result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to UploadImagesAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> GenerateStackFiles(ApiBatchEmailModel model)
        {
            var currentAction = "GenerateStackFiles";
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.SocialContainerServer, "api/email/generate_stack_files");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", CommonHelpers.GetCurrentLangageCode());
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
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api {0} - {1} because: {2}", currentAction, _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
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