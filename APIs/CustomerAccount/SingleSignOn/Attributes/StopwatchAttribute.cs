using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace SingleSignOn.Attributes
{
    /// <summary>
    /// http://stackoverflow.com/questions/17069159/log-duration-of-an-asp-web-api-action
    /// http://www.rbwestmoreland.com/posts/instrumenting-asp-net-web-api/
    /// </summary>
    public class StopwatchAttribute : ActionFilterAttribute
    {
        private const string StopwatchKey = "StopwatchFilter.Value";

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            actionContext.Request.Properties[StopwatchKey] = Stopwatch.StartNew();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            Stopwatch stopwatch = (Stopwatch)actionExecutedContext.Request.Properties[StopwatchKey];
            // TODO something useful with stopwatch.Elapsed
            Trace.WriteLine("Elapsed = " + stopwatch.Elapsed);
        }
    }
}