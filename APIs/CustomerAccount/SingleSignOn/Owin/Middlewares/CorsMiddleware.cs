using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Diagnostics;

using Owin;
using Microsoft.Owin;

using SingleSignOn.Settings;

namespace SingleSignOn.Owin.Middlewares
{
    public class CorsMiddleware : OwinMiddleware
    {     
        public CorsMiddleware(OwinMiddleware next)
            : base(next)
        {
            
        }

        public override async Task Invoke(IOwinContext context)
        {
            IOwinRequest req = context.Request;
            IOwinResponse res = context.Response;

            req.ContentType = "application/json";
            if(req.Path.StartsWithSegments(new PathString("/api/user"))){
                var origin = req.Headers.Get("Origin");

                if (string.IsNullOrEmpty(origin))
                    origin = SingleSignOnSettings.AllowedCorsOrigins;

                if (!string.IsNullOrEmpty(origin))
                {
                    res.Headers.Set("Access-Control-Allow-Origin", origin);
                }

                if (req.Method == "OPTIONS"){
                    res.StatusCode = 200;
                    res.Headers.AppendCommaSeparatedValues("Access-Control-Allow-Methods", "GET", "POST");
                    res.Headers.AppendCommaSeparatedValues("Access-Control-Allow-Headers", "authorization", "content-type");
                    return;
                }
            }
            await Next.Invoke(context);
        }
    }
}



/*
 * http://stackoverflow.com/questions/20079813/how-to-make-cors-authentication-in-webapi-2
 public void ConfigureAuth(IAppBuilder app)
{
    //other stuff
    app.Use(async (context, next) =>
    {
        IOwinRequest req = context.Request;
        IOwinResponse res = context.Response;
        if (req.Path.StartsWithSegments(new PathString("/Token")))
        {
            var origin = req.Headers.Get("Origin");
            if (!string.IsNullOrEmpty(origin))
            {
                res.Headers.Set("Access-Control-Allow-Origin", origin);
            }
            if (req.Method == "OPTIONS")
            {
                res.StatusCode = 200;
                res.Headers.AppendCommaSeparatedValues("Access-Control-    Allow-Methods", "GET", "POST");
                res.Headers.AppendCommaSeparatedValues("Access-Control-    Allow-Headers", "authorization", "content-type");
                return;
            }
        }
        await next();
    });
    //other stuff
}
 */
