using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MySite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(name: "signin-google", url: "signin-google", defaults: new { controller = "WebAuth", action = "ExternalLoginCallback" });

            routes.MapRoute("TermsConditions", "chinh-sach-dieu-khoan", defaults: new { controller = "Privacy", action = "TermsConditions", id = UrlParameter.Optional });

            routes.MapRoute("PrivacyPolicy", "chinh-sach-bao-mat", defaults: new { controller = "Privacy", action = "PrivacyPolicy", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "JobDetail",
                url: "job/detail/{id}/{slug}",
                defaults: new { controller = "Job", action = "Detail", id = UrlParameter.Optional, slug = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
