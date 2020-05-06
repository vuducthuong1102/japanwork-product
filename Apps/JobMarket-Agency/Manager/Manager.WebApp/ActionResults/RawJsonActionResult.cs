using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;

namespace Manager.WebApp.ActionResults
{
    public class RawJsonActionResult : IHttpActionResult
    {
        private readonly string _jsonString;

        public RawJsonActionResult(string jsonString)
        {
            _jsonString = jsonString;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            /*
            var content = new StringContent(_jsonString, enco);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            */

            var content = new StringContent(_jsonString, Encoding.UTF8, "application/json");
            var response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = content                 
            };
            return Task.FromResult(response);
        }
    }
}