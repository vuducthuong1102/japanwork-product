using System;
using Owin;
using Microsoft.Owin;
using Autofac;
using System.Reflection;
using System.Configuration;
using System.Web.Cors;
using Microsoft.Owin.Cors;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using System.Web.Http;

[assembly: OwinStartup(typeof(MyCloud.Startup))]
namespace MyCloud
{
    public partial class Startup
    {
        private static IContainer _container;
        public static IContainer AutofacContainer { get { return _container; } }

        public void Configuration(IAppBuilder app)
        {
            //If CorsPolicy enabled
            //if (Convert.ToBoolean(ConfigurationManager.AppSettings["SignalR:AllowCorsPolicy"]))
            //{
            //    var policy = new CorsPolicy()
            //    {
            //        AllowAnyHeader = true,
            //        AllowAnyMethod = true,
            //        SupportsCredentials = true
            //    };

            //    var strAllowHosts = ConfigurationManager.AppSettings["SignalR:AllowHosts"].ToString();
            //    if (!string.IsNullOrEmpty(strAllowHosts))
            //    {
            //        string[] hostsList = strAllowHosts.Split(';');
            //        if (hostsList.Length > 0)
            //        {
            //            foreach (string host in hostsList)
            //            {
            //                policy.Origins.Add(host); //be sure to include the port:
            //                //example: "http://localhost:8081"

            //                app.UseCors(new CorsOptions
            //                {
            //                    PolicyProvider = new CorsPolicyProvider
            //                    {
            //                        PolicyResolver = context => Task.FromResult(policy)
            //                    }
            //                });
            //            }
            //        }
            //    }
            //}

            //app.MapSignalR();
            // Branch the pipeline here for requests that start with "/signalr"
            app.Map("/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });

            //Initialize autofac container
            InitAutofacContainer(app);

            ConfigureForOwinStartup(app);

            //RegisterForWebAPI(app);
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


        private void InitAutofacContainer(IAppBuilder app)
        {
            ContainerBuilder builder = new ContainerBuilder();

            // Register all Autofact Modules (OWin, WebApi, WebMvc, ....)
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());

            // You can  register all dependencies here, normally we define in modules

            //then
            _container = builder.Build();


            //Middlewares: Enabling DI for OWIN middleware: 
            //should be the first middleware added to IAppBuilder

            // Register the Autofac middleware FIRST. This also adds
            // Autofac-injected middleware registered with the container.
            //app.UseAutofacMiddleware(_container);

            // ...then register your other middleware not registered
            // with Autofac.
        }
    }
}
