using Microsoft.AspNet.SignalR;
using MyCloud.SharedLib.Logging;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MyCloud
{
    public class MvcApplication : System.Web.HttpApplication
    {
         private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //Register api
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Wait a maximum of 30 seconds (Default value) after a transport connection is lost before raising the
            // Disconnected event to terminate the SignalR connection.
            //int disconnectTimeout = 30;
            //GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(disconnectTimeout);
        }
    }
}
