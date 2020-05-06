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
using System.Collections.Generic;
using ApiJobMarket.SharedLib.Extensions;
using System.Web.Http;

namespace ApiJobMarket.Services
{
    public class CdnServices
    {
        private static readonly ILog logger = LogProvider.For<CdnServices>();

        public static async Task<ResponseApiModel> UploadImagesAsync(ApiUploadFileModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", CDNSettings.SocialFileReadingServer, "api/upload/images");

            try
            {
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        if (model.Files != null && model.Files.Count > 0)
                        {
                            foreach (var item in model.Files)
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

                        parameters.Add("FilesInString", JsonConvert.SerializeObject(model.FilesInString));
                        parameters.Add("SubDir", model.SubDir.ToString());
                        parameters.Add("ObjectId", model.ObjectId.ToString());
                        parameters.Add("InCludeDatePath", model.InCludeDatePath.ToString());
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

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> UploadVideoAsync(ApiUploadFileModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", CDNSettings.SocialFileReadingServer, "api/upload/videos");

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(5);
                    using (var content = new MultipartFormDataContent())
                    {
                        if (model.Files != null && model.Files.Count > 0)
                        {
                            foreach (var item in model.Files)
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

                        parameters.Add("FilesInString", JsonConvert.SerializeObject(model.FilesInString));
                        parameters.Add("SubDir", model.SubDir.ToString());
                        parameters.Add("ObjectId", model.ObjectId.ToString());
                        parameters.Add("InCludeDatePath", model.InCludeDatePath.ToString());
                        parameters.Add("GenerateThumb", model.GenerateThumb.ToString());
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
                strError = string.Format("Failed when calling api to UploadVideoAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> UploadImagesFromBase64Async(ApiUploadFileModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", CDNSettings.SocialFileReadingServer, "api/upload/base64_images");

            try
            {
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        if (model.Files != null && model.Files.Count > 0)
                        {
                            foreach (var item in model.Files)
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
                        parameters.Add("FilesInString", JsonConvert.SerializeObject(model.FilesInString));
                        parameters.Add("SubDir", model.SubDir.ToString());
                        parameters.Add("ObjectId", model.ObjectId.ToString());
                        parameters.Add("InCludeDatePath", model.InCludeDatePath.ToString());
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
                strError = string.Format("Failed when calling api to UploadImagesFromBase64Async - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> DeleteImagesAsync(FilesDeleteModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", CDNSettings.SocialFileReadingServer, "api/file/delete");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                //client.AuthorizationHttpClientCore();

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
                strError = string.Format("Failed when calling api to DeleteImagesAsync - {0} because: {1}", _baseUrl, ex.ToString());
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
    }
}