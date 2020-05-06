using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;

namespace SingleSignOn.ActionResults
{    
    public class HttpActionResult : IHttpActionResult
    {
        private readonly string _message;
        private readonly string _mediaType; //JSON = "application/json", XML = application/xml, HTML = text/html, CSV = text/csv, 
        private readonly HttpStatusCode _statusCode;

        public HttpActionResult(HttpStatusCode statusCode, string message, string mediaType )
        {
            _statusCode = statusCode;
            _message = message;
            _mediaType = mediaType;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_message, Encoding.UTF8, _mediaType)       
            };
            return Task.FromResult(response);
        }
    }


    public class JsonActionResult: HttpActionResult
    {
        public JsonActionResult(HttpStatusCode statusCode, string message) : base(statusCode, message, "application/json")
        {

        }
    }

    public class TextPlainActionResult : HttpActionResult
    {
        public TextPlainActionResult(HttpStatusCode statusCode, string message) : base(statusCode, message, "text/plain")
        {

        }
    }


    public class TextHtmlActionResult : HttpActionResult
    {
        public TextHtmlActionResult(HttpStatusCode statusCode, string message) : base(statusCode, message, "text/html")
        {

        }
    }

    /*
    public IHttpActionResult Get()
    {
       return new HttpActionResult(HttpStatusCode.InternalServerError, "error message"); // can use any HTTP status code
    }
     */
}