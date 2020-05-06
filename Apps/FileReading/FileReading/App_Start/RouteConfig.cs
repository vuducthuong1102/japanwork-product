using FileReading.Settings;
using System.Web.Mvc;
using System.Web.Routing;

namespace FileReading
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var defaultUrl = (SystemSettings.ReadFileDirectly == false) ? "{controller}/{action}/{id}" : "{*any}";

            routes.MapRoute(
                name: "Default",
                url: defaultUrl,
                //url: "{*any}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}
