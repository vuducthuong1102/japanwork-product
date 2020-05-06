using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using MySite.Logging;
using MySite.Resources;
using MySite.Settings;
using System.Collections.Generic;
using MySite.Models.External.OpenWeather;
using MySite.ShareLibs;
using System.Linq;
using MySite.Helpers;
using System.Threading.Tasks;

namespace MySite.Services
{
    public class OpenWeatherService
    {
        private static readonly ILog logger = LogProvider.For<OpenWeatherService>();
        public static readonly double _KenvilToCelsius = 273.15;
        public class Ref<T>
        {
            public T Value;
        }
        //, Ref<bool> hasTimeOut

        public static async Task<WeatherResponseModel> GetWeather(WeatherInputModel model)
        {
            var strError = string.Empty;
            var result = new WeatherResponseModel();
            var _baseUrl = OpenWeatherSettings.ServiceUrl;
            _baseUrl = string.Format("{0}?q={1},{2}&id={3}&appid={4}", _baseUrl, model.city, model.country, OpenWeatherSettings.Id, OpenWeatherSettings.AppId);

            //?q=Hanoi,vn&id=524901&appid=9e56900ad341b8e0e9c626058f977984

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                logger.Debug("Begin GetOpenWeather request");
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
                response = await client.GetAsync(_baseUrl);

                // Parsing the returned result                    
                var responseString = response.Content.ReadAsStringAsync().Result;
                result.message = strError;
                result.statuscode = response.StatusCode;
                result.statuscode = response.StatusCode;
                result.data = JsonConvert.DeserializeObject<OpenWeatherResponseData>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to GetOpenWeather - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);
               // hasTimeOut.Value = true;

                result.code = "-1";
                result.message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                result.statuscode = HttpStatusCode.BadRequest;

                return result;
            }

            logger.Debug("End GetOpenWeather request");

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

        public static string GetTempWithCurrentDate(List<WeatherItem> weatherItems, ref string weatherLabel, ref string icon)
        {
            var dtNow = DateTime.Now;
            foreach (var item in weatherItems)
            {
                var matched = item.DailyData.Where(x => (dtNow <= EpochTime.DateTime(x.dt))).FirstOrDefault();

                if (matched != null)
                {
                    weatherLabel = matched.weather[0].main;
                    var currentTemp = (int)(matched.main.temp - _KenvilToCelsius);
                    icon = matched.weather[0].icon;
                    return currentTemp.ToString();
                }
            }

            return string.Empty;
        }
    }

    public class WeatherResponseModel
    {
        public HttpStatusCode statuscode { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public OpenWeatherResponseData data { get; set; }
    }

    public class OpenWeatherResponseData
    {
        public string cod { get; set; }
        public string message { get; set; }
        public int cnt { get; set; }
        public List<WeatherData> list { get; set; }
        public City city { get; set; }
    }

    public class WeatherInputModel
    {
        public string city { get; set; }
        public string country { get; set; }
    }
}