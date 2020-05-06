using System;
using System.Threading.Tasks;
using System.Reflection;

using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


using System.Web.Http;
using System.Net.Http.Formatting;


using Owin;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;


using System.Web.Cors;
using Microsoft.Owin.Cors;



using MySite.Logging;
using MySite.Settings;
using MySite.HttpTracking;

[assembly: OwinStartup(typeof(MySite.Startup))]
namespace MySite
{
    public partial class Startup
    {
        private readonly ILog logger = LogProvider.For<Startup>();
        
        public void Configuration(IAppBuilder app)
        {           
            //Apply log4net configuration
            ConfigureLog4net();

            logger.Info("======================================================");
            logger.Info("MySite is starting up");
            logger.Info("======================================================");

                      
            //Initialize autofac container
            InitAutofacContainer(app);


           // app.UseHttpTracking(
           //   new HttpTrackingOptions
           //   {
           //       TrackingStore = new HttpTrackingStore(),
           //       //TrackingIdPropertyName = "x-tracking-id",
           //       TrackingIdPropertyName = "X-Tracking-Id",
           //       MaximumRecordedRequestLength = 64 * 64 * 1024,
           //       MaximumRecordedResponseLength = 64 * 64 * 1024,
           //   }
           //);


            //Configure for WebAPI
          
            //Configure for WebMVC
            RegisterForWebMVC(app);

            ConfigureAuth(app);

            //Apply CORS policy:
            RegisterCorsPolicy(app);

            logger.Info("======================================================");
            logger.Info("MySite is successfully configured");
            logger.Info("======================================================");
        }


        #region Helpers

        private void ConfigureLog4net()
        {
            //log4net.Config.XmlConfigurator.Configure();
            //. = root, Web = your physical directory that contains all other static content, see prev step
            var physicalFileSystem = new PhysicalFileSystem(@".\App_Data\Log4net");
            IFileInfo log4netConfigFileInfo = null;
            physicalFileSystem.TryGetFileInfo("log4net.config", out log4netConfigFileInfo);
            if (log4netConfigFileInfo != null)
            {
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(log4netConfigFileInfo.PhysicalPath));
            }
        }

        private void InitAutofacContainer(IAppBuilder app)
        {
            //ContainerBuilder builder = new ContainerBuilder();

            //// Register all Autofact Modules (OWin, WebApi, WebMvc, ....)
            //builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());

            //// You can  register all dependencies here, normally we define in modules

            ////then
            //_container = builder.Build();

            //if (GlobalContainer.IocContainer == null)
            //{
            //    GlobalContainer.IocContainer = _container;
            //}


            ////Middlewares: Enabling DI for OWIN middleware: 
            ////should be the first middleware added to IAppBuilder

            //// Register the Autofac middleware FIRST. This also adds
            //// Autofac-injected middleware registered with the container.
            //app.UseAutofacMiddleware(_container);

            //// ...then register your other middleware not registered
            //// with Autofac.
        }

        private void RegisterForWebAPI(IAppBuilder app)
        {
            var httpConfig = new HttpConfiguration();
            //var httpConfig = GlobalConfiguration.Configuration;

            //WebAPI: Set the WebApi dependency resolver.
            //httpConfig.DependencyResolver = new AutofacWebApiDependencyResolver(_container);           

            //Web.API enable lifetime scope created during the OWIN request to extend into WebAPI.
            //app.UseAutofacWebApi(httpConfig);         
            
            // We’ll pass the “config” object to the extension method “UseWebApi” 
            // which will be responsible to wire up ASP.NET Web API to our Owin server pipeline.
            app.UseWebApi(httpConfig);

            //Configure for Webp API Route
            WebApiConfig.Register(httpConfig);            
        }

        private void RegisterForWebMVC(IAppBuilder app)
        {
            //Extending the lifetime scope to MVC
            // Autofac MVC Integration -- http://alexmg.com/owin-support-for-the-web-api-2-and-mvc-5-integrations-in-autofac/
            //app.UseAutofacMvc();

            //MVC: Set MVC DI resolver to use our Autofac container
            //var dependencyResolver = new AutofacDependencyResolver(_container);
            //System.Web.Mvc.DependencyResolver.SetResolver(dependencyResolver); //TODO: Still needed with OWIN?

            /*
            Currently ASP.NET MVC doesn't run on OWIN. Web API will because it's been decoupled from System.Web, specifically HttpContext. 
            This is the main dependency that prevents MVC from running on OWIN as well. 
            Some alternatives that do run on OWIN are FubuMVC, Nancy and Simple.Web
            */


            /*** REGISTER MVC ************/
            // Register for MVC configs for OWIN startup so we don't need Global.asax
            // Infact the registration is still in GLOBAL.ASAX
            /*
            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);//Don't use this line because we use owin to register API
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            */
        }


        private void RegisterCorsPolicy(IAppBuilder app)
        {
            /*
            Browser security prevents a web page from making AJAX requests to another domain. 
            This restriction is called the same-origin policy, and prevents a malicious site from reading sentitive data from another site. 
            However, sometimes you might want to let other sites call your web API.

            Cross Origin Resource Sharing (CORS) is a W3C standard that allows a server to relax the same-origin policy. 
            Using CORS, a server can explicitly allow some cross-origin requests while rejecting others. 
            CORS is safer and more flexible than earlier techniques such as	JSONP. 
            This tutorial shows how to enable CORS in your Web API application.

            http://www.asp.net/web-api/overview/security/enabling-cross-origin-requests-in-web-api
            */


            var corsPolicy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true,
            };

            //Try and load allowed origins from web.config
            //If none are specified we'll allow all origins            
            var origins = MySiteSettings.AllowedCorsOrigins;

            if (!string.IsNullOrEmpty(origins))
            {
                foreach (var origin in origins.Split(new char[] { ';', ',' }))
                {
                    corsPolicy.Origins.Add(origin);
                }
            }
            else
            {
                corsPolicy.AllowAnyOrigin = true;
            }

            var corsOptions = new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context => Task.FromResult(corsPolicy)
                }
            };

            app.UseCors(corsOptions);


        }

        #endregion
    }
}
