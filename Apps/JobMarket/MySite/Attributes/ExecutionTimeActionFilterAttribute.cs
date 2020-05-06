//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;
//using System.Net.Http.Headers;
//using System.Diagnostics;

//using Owin;
//using Microsoft.Owin;
//using MySite.HttpTracking;
//using MySite.Settings;

//namespace MySite.Attributes
//{
//    public class ExecutionTimeActionFilterAttribute : ActionFilterAttribute
//    {        
//        public override void OnActionExecuting(HttpActionContext actionContext)
//        {
//            base.OnActionExecuting(actionContext);

//            var request = actionContext.Request;
//            var response = actionContext.Response;

//            //Begin login
//            var logEntry = new HttpEntry();
//            var identity =
//             actionContext.RequestContext.Principal.Identity != null && actionContext.RequestContext.Principal.Identity.IsAuthenticated ?
//                 actionContext.RequestContext.Principal.Identity.Name :
//                 "(anonymous)"
//                 ;

//            var record = new HttpEntry
//            {
//                Application = MySiteSettings.MDS_FromAppName,
//                MachineName = Environment.MachineName,
//                CallerIdentity = identity,
//            };


//            record.RequestVerb = request.Method.Method;
//            record.RequestUri = request.RequestUri;
//            record.RequestHeaders = GetHeaderDictionary(request.Headers);

          
//            const string OWIN_CONTEXT = "MS_OwinContext"; //incase of WebAPI
//            const string HTTP_CONTEXT = "MS_HttpContext"; //incase of WebMVC

//            string ipAddressString = null;
//            if (request.Properties.ContainsKey(OWIN_CONTEXT))
//            {
//                OwinContext owinContext = request.Properties[OWIN_CONTEXT] as OwinContext;
//                if (owinContext != null)
//                    ipAddressString = owinContext.Request.RemoteIpAddress;
//            }

//            if (string.IsNullOrEmpty(ipAddressString) && request.Properties.ContainsKey(OWIN_CONTEXT))
//            {
//                HttpContextWrapper httpContext = actionContext.Request.Properties[HTTP_CONTEXT] as HttpContextWrapper;
//                if (httpContext != null)
//                    ipAddressString = httpContext.Request.UserHostAddress;
//            }

//            record.RequestIpAddress = ipAddressString;
//            record.RequestBody =  request.Content.ReadAsStringAsync().Result;
//            record.RequestLength = record.RequestBody!= null ? record.RequestBody.Length:0;


            
//            var controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
//            var actionName = actionContext.ActionDescriptor.ActionName;

//            var uniqueKey = string.Format("{0}::{1}", controllerName, actionName);

//            request.Properties[uniqueKey + "_HTTPENTRY"] = record;
//            request.Properties[uniqueKey + "_STOPWATCH"] = Stopwatch.StartNew();
//        }


        

//        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
//        {
//            base.OnActionExecuted(actionExecutedContext);

//            var response = actionExecutedContext.Response;
//            var request = actionExecutedContext.Request;

//            var controllerName = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
//            var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;

//            var uniqueKey = string.Format("{0}::{1}", controllerName, actionName);

//            HttpEntry record = (HttpEntry)request.Properties[uniqueKey + "_HTTPENTRY"];
//            Stopwatch stopwatch = (Stopwatch)request.Properties[uniqueKey + "_STOPWATCH"];

            

//            record.StatusCode = (int)response.StatusCode;
//            record.ReasonPhrase = response.ReasonPhrase;
//            record.ResponseHeaders = GetHeaderDictionary(response.Headers);

//            record.ResponseBody = response.Content.ReadAsStringAsync().Result;
//            record.ResponseLength = record.ResponseBody != null ? record.ResponseBody.Length : 0;


//            record.CallBackTime = DateTime.Now;
//            record.CallDuration = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);

//            stopwatch.Stop();

//            new HttpTrackingStore().InsertRecord(record);
//        }



//        private IDictionary<string, string[]> GetHeaderDictionary(HttpHeaders headers)
//        {
//            var dict = new Dictionary<string, string[]>();

//            foreach (var item in headers.ToList())
//            {
//                if (item.Value != null)
//                {
//                    dict.Add(item.Key, item.Value.ToArray());
//                }
//            }

//            return dict;
//        }
//    }
//}