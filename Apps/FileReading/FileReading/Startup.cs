using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using Microsoft.Owin.FileSystems;
using FileReading.Logging;

[assembly: OwinStartup(typeof(FileReading.Startup))]
namespace FileReading
{   
    public class Startup
    {
        private readonly ILog logger = LogProvider.For<Startup>();

        //Need to install:  Microsoft.Owin.Host.SystemWeb package
        public void Configuration(IAppBuilder app)
        {
            ConfigureLog4net();

            logger.Info("======================================================");
            logger.Info("FileReading is starting up");
            logger.Info("======================================================");

            //Configure for WebAPI
            RegisterForWebAPI(app);            

            RegisterCorsPolicy(app);

            logger.Info("======================================================");
            logger.Info("FileReading is successfully configured");
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

        private void RegisterForWebAPI(IAppBuilder app)
        {
            var httpConfig = new HttpConfiguration();

            //Configure for Webp API Route
            WebApiConfig.Register(httpConfig);
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
            var origins = "*";

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