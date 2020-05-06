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

            routes.MapRoute("candidates", "candidates", defaults:
                new { controller = "Content", action = "candidates" }
                );
            routes.MapRoute("company", "company", defaults:
              new { controller = "Content", action = "company" }
              );
            routes.MapRoute("staff", "staff", defaults:
              new { controller = "Content", action = "staff" }
              );
            routes.MapRoute("job", "job", defaults:
              new { controller = "Content", action = "job" }
              );
            routes.MapRoute("apply", "apply", defaults:
              new { controller = "Content", action = "apply" }
              );
            routes.MapRoute("matching", "matching", defaults:
              new { controller = "Content", action = "matching" }
              );
            routes.MapRoute("schedule", "schedule", defaults:
              new { controller = "Content", action = "schedule" }
              );
            routes.MapRoute("task", "task", defaults:
              new { controller = "Content", action = "task" }
              );
            routes.MapRoute("file", "file", defaults:
              new { controller = "Content", action = "file" }
              );
            routes.MapRoute("import", "import", defaults:
              new { controller = "Content", action = "import" }
              );
            routes.MapRoute("data", "data", defaults:
             new { controller = "Content", action = "data" }
             );
            routes.MapRoute("sales", "sales", defaults:
              new { controller = "Content", action = "sales" }
              );
            routes.MapRoute("invoice", "invoice", defaults:
              new { controller = "Content", action = "invoice" }
              );
            routes.MapRoute("candidates-option", "candidates-option", defaults:
              new { controller = "Content", action = "candidates_option" }
              );
            routes.MapRoute("data-option", "data-option", defaults:
              new { controller = "Content", action = "DataOption" }
              );
            routes.MapRoute("map", "map", defaults:
              new { controller = "Content", action = "map" }
              );
            routes.MapRoute("sms", "sms", defaults:
              new { controller = "Content", action = "sms" }
              );
            routes.MapRoute("facebook", "facebook", defaults:
              new { controller = "Content", action = "facebook" }
              );
            routes.MapRoute("line", "line", defaults:
              new { controller = "Content", action = "line" }
              );
            routes.MapRoute("introduce", "introduce", defaults:
             new { controller = "Content", action = "introduce" }
             );
            routes.MapRoute("cloud-plan", "cloud-plan", defaults:
            new { controller = "Content", action = "cloudplan" }
            );
            routes.MapRoute("installation-plan", "installation-plan", defaults:
           new { controller = "Content", action = "installationplan" }
           );

            routes.MapRoute(
                name: "JobDetail",
                url: "job/detail/{id}/{slug}",
                defaults: new { controller = "Job", action = "Detail", id = UrlParameter.Optional, slug = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Content", action = "introduce", id = UrlParameter.Optional }
            );
        }
    }
}
