using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

using System.Threading.Tasks;

using Microsoft.Owin;
using Newtonsoft.Json;


namespace MySite.Owin.Middlewares
{

    /// <summary>
    /// To wire this up we have to register the middleware and its dependencies with the container, 
    /// and then enable the middleware dependency injection support via a call to UseAutofacMiddleware. 
    /// This call should be the first piece of middleware registered with the IAppBuilder
    /// </summary>
    public class LoggerMiddleware : OwinMiddleware
    {      
        public LoggerMiddleware(OwinMiddleware next)
            : base(next)
        {
            
        }
       
        public override async Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);
        }
       

        /*
        public override async Task Invoke(IOwinContext context)
        {
            ApiLogEntry logEntry = null;
            IOwinRequest req = context.Request;
            IOwinResponse res = context.Response;

            if (MySiteSettings.LogIsAliveRequestEnabled)
            {
                //Log for IsAlive request
                if (req.Path.StartsWithSegments(new PathString("/api/fp/isalive")))
                {
                    logEntry = new ApiLogEntry
                    {
                        Application = MySiteSettings.MDS_FromAppName,
                        User = req.User.Identity.Name,
                        Machine = Environment.MachineName,
                        RequestContentType = context.Request.ContentType,
                        RequestRouteTemplate = "",
                        RequestRouteData = "",
                        RequestIpAddress = context.Request.RemoteIpAddress,
                        RequestMethod = req.Method,
                        RequestHeaders = SerializeHeaders(req.Headers),
                        RequestTimestamp = DateTime.Now,
                        RequestUri = req.Uri.ToString()
                    };
                    context.Set(ApiLogEntry.TrackingIdPropertyName, logEntry.ApiLogEntryId);
                    context.Set(string.Format("ApiLogEntry#{0}", logEntry.ApiLogEntryId), logEntry);

                    // add the "Http-Tracking-Id" response header
                    context.Response.OnSendingHeaders(state =>
                    {
                        var ctx = state as IOwinContext;
                        var resp = ctx.Response;

                        // adding the tracking id response header so that the user
                        // of the API can correlate the call back to this entry

                        resp.Headers.Add(ApiLogEntry.TrackingIdPropertyName, new[] { logEntry.ApiLogEntryId, });

                    }, context)
                    ;
                }
            }


            await Next.Invoke(context);


            //Process end request here       
            if (MySiteSettings.LogIsAliveRequestEnabled)
            {
                if (req.Path.StartsWithSegments(new PathString("/api/fp/isalive")))
                {
                    var entryID = context.Get<string>(ApiLogEntry.TrackingIdPropertyName);
                    if (!string.IsNullOrEmpty(entryID))
                    {
                        var contextKey = string.Format("ApiLogEntry#{0}", entryID);
                        var combackLogEntry = context.Get<ApiLogEntry>(contextKey);

                        if (combackLogEntry != null)
                        {
                            // Update the API log entry with response info
                            combackLogEntry.ResponseStatusCode = (int)res.StatusCode;
                            combackLogEntry.ResponseContentType = res.Headers["Content-Type"];
                            combackLogEntry.ResponseHeaders = SerializeHeaders(res.Headers);
                            combackLogEntry.ResponseTimestamp = DateTime.Now;


                            var duration = (combackLogEntry.ResponseTimestamp - combackLogEntry.RequestTimestamp).Value.TotalMilliseconds;

                            // Send the log entry to MDS system
                            _mdsService.SendLogEntryInfo(combackLogEntry);
                        }
                    }
                }
            }
        }
        */

        private string SerializeRouteData(IHttpRouteData routeData)
        {
            return JsonConvert.SerializeObject(routeData, Formatting.Indented);
        }

        private string SerializeHeaders(IHeaderDictionary headers)
        {
            var dict = new Dictionary<string, string>();

            foreach (var item in headers.ToList())
            {
                if (item.Value != null)
                {
                    var header = String.Empty;
                    foreach (var value in item.Value)
                    {
                        header += value + " ";
                    }

                    // Trim the trailing space and add item to the dictionary
                    header = header.TrimEnd(" ".ToCharArray());
                    dict.Add(item.Key, header);
                }
            }

            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }
    }
}