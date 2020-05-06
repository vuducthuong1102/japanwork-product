using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Manager.WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "DynamicViewRoute",
                url: "DynamicViews/{*pageName}",
                defaults: new { controller = "Dynamic", action = "GetView" },
                constraints: new { controller = "Dynamic", action = "GetView" }
            );

            routes.MapRoute(
                name: "ArticleDetail",
                url: "article/detail/{id}/{slug}",
                defaults: new { controller = "Article", action = "Detail", id = UrlParameter.Optional, slug = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProjectDetail",
                url: "ourproject/detail/{id}/{slug}",
                defaults: new { controller = "OurProject", action = "Detail", id = UrlParameter.Optional, slug = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );            
        }
    }
}
