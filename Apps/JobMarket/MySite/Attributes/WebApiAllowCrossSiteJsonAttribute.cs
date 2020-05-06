using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace MySite.Attributes
{
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        /*
         [AllowCrossSiteJson]
         public class ValuesController : ApiController
         {
         */
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null)
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}