using System;
using System.Threading.Tasks;
using EmailConsoler.DataStorage;
using EmailConsoler.Models;
using EmailConsoler.Logging;
using EmailConsoler.Helpers;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace EmailConsoler.Services
{
    public interface INotificationService
    {
        //Notification
        Task<SqlObjectResponse> TransferQueueForContainer();

        Task<NotificationResponseModel> SendNotifAsync(NotificationInputModel model);
    }

    public class NotificationService : INotificationService
    {
        private static ILog logger = LogProvider.For<NotificationService>();
        private string _dateTimeFormat = "dd/MM/yyyy H:mm:ss";
        private string _dateTimeXMLFormat = "yyyy-MM-ddTHH:mm:ssZ";
        private VodMetadata _vodMetadata = null;

        IMySQLVodStore _vodStore;

        //public NotificationService(IMySQLVodStore vodStore)
        //{
        //    _vodStore = vodStore;
        //}

        #region Notifications

        public virtual async Task<SqlObjectResponse> TransferQueueForContainer()
        {
            var sqlObjectResponse = await _vodStore.TransferQueueForContainer();
            return sqlObjectResponse;
        }

        public virtual async Task<SqlObjectResponse> PickMetadataFromQueueContainer()
        {
            var sqlObjectResponse = await _vodStore.PickMetadataFromQueueContainer();
            return sqlObjectResponse;
        }

        public virtual async Task<NotificationResponseModel> SendNotifAsync(NotificationInputModel model)
        {
            var strError = string.Empty;
            var result = new NotificationResponseModel();
            var _baseUrl = NotificationSettings.SendNotifUrl;

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                logger.Debug(string.Format("Raw notification object: {0}", rawData));

                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Accept-Language", model.LangCode);

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = await client.PostAsync(_baseUrl, theContent);

                // Parsing the returned result                    
                var responseString = response.Content.ReadAsStringAsync().Result;

                result = JsonConvert.DeserializeObject<NotificationResponseModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to SendNotif - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);
                result.code = -1;
                result.message = strError;
                result.statuscode = HttpStatusCode.BadRequest;

                return result;
            }

            logger.Debug("End SendNotif request");

            return result;
        }

        #endregion

        private static void HttpStatusCodeTrace(HttpResponseMessage response)
        {
            var statusCode = Utils.ConvertToInt32(response.StatusCode);
            if (statusCode != (int)HttpStatusCode.OK)
            {
                logger.Debug(string.Format("Return code: {0}, message: {1}", response.StatusCode, response.RequestMessage.RequestUri));
            }
        }
    }

    public class NotificationResponseModel
    {
        public HttpStatusCode statuscode { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    public class NotificationInputModel
    {
        public string RegistrationID { get; set; } // registrationID FCM được user gửi lên qua signal
        public string DeviceID { get; set; } // deviceID được user gửi lên qua signal
        public bool iosDevice { get; set; } // thiết bị là android hay iOS
        public string LangCode { get; set; }
        public dynamic Data { get; set; } // data notification       
    }
}