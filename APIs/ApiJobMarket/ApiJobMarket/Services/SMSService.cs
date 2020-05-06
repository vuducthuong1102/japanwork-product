using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using ApiJobMarket.Logging;
using ApiJobMarket.Settings;
using ApiJobMarket.Resources;

namespace ApiJobMarket.Services
{
    public class SMSService
    {
        private static readonly ILog logger = LogProvider.For<SMSService>();

        public static SMSResponseModel Send(SMSInputModel model, ref bool hasTimeOut)
        {
            var strError = string.Empty;
            var result = new SMSResponseModel();
            var _baseUrl = SystemSettings.SMSServiceUrl;

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                logger.Debug("Begin SendSMS request");
                logger.Debug(string.Format("Raw data: {0}", rawData));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    result.message = strError;
                    result.statuscode = response.StatusCode;
                    result.statuscode = response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to send sms - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);
                hasTimeOut = true;

                result.code = "-1";
                result.message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                result.statuscode = HttpStatusCode.BadRequest;

                return result;
            }

            logger.Debug("End SendSMS request");

            return result;
        }

        public static string ReformatPhoneNumber(string phoneNumber, string region = "vn")
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                if (region == "vn")
                {
                    if (phoneNumber.StartsWith("0"))
                    {
                        //Cut the zero of the phonenumber
                        phoneNumber = phoneNumber.Substring(0);
                        phoneNumber = "+84" + phoneNumber;
                    }                    
                }

                return phoneNumber;
            }

            return string.Empty;
        }
    }

    public class SMSResponseModel
    {
        public HttpStatusCode statuscode { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }
    
    public class SMSInputModel
    {
        public string phoneNumber { get; set; }
        public string message { get; set; }
    }

    public class EmailService
    {
        private static readonly ILog logger = LogProvider.For<EmailService>();

        public static EmailResponseModel Send(EmailInputModel model, ref bool hasTimeOut)
        {
            var strError = string.Empty;
            var result = new EmailResponseModel();
            var _baseUrl = SystemSettings.SendEmailServiceUrl;

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                logger.Debug("Begin SendEmail request");
                logger.Debug(string.Format("Raw data: {0}", rawData));

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    result.message = strError;
                    result.statuscode = response.StatusCode;
                    result.statuscode = response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to send Email - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);
                hasTimeOut = true;

                result.code = "-1";
                result.message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                result.statuscode = HttpStatusCode.BadRequest;

                return result;
            }

            logger.Debug("End SendEmail request");

            return result;
        }
    }

    public class EmailResponseModel
    {
        public HttpStatusCode statuscode { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }

    public class EmailInputModel
    {
        public string sendto { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
}