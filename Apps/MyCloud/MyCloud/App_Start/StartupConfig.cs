using MyCloud.SharedLib.Caching;
using Owin;
using System;
using Microsoft.Owin.FileSystems;
using Autofac.Integration.Mvc;
using Autofac;
using System.Reflection;
using MyCloud.SharedLib.Logging;

namespace MyCloud
{
    public partial class Startup
    {
        private readonly ILog logger = LogProvider.For<Startup>();

        public void ConfigureForOwinStartup(IAppBuilder app)
        {
            //Apply log4net configuration
            ConfigureLog4net();

            try
            {
                CacheProviderManager.Initialize();
            }
            catch (Exception ex)
            {

                logger.Error("Cache initializing error: " + ex.InnerException.ToString());
            }

            logger.Info("======================================================");
            logger.Info("MyCloud is starting up");
            logger.Info("======================================================");

            //Configure Autofac for WebMvc + Owin
            ConfigureAutofac(app);

          
            logger.Info("======================================================");
            logger.Info("MyCloud is successfully configured");
            logger.Info("======================================================");
        }

        #region Log4net

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

        #endregion

        #region Autofac IOC

        private void ConfigureAutofac(IAppBuilder app)
        {

            if (GlobalContainer.IocContainer == null)
            {
                GlobalContainer.IocContainer = InitializeAutofacContainer();
            }

            //Middlewares: Enabling DI for OWIN middleware: 
            //should be the first middleware added to IAppBuilder
            app.UseAutofacMiddleware(GlobalContainer.IocContainer);

            //Extending the lifetime scope to MVC
            // Autofac MVC Integration -- http://alexmg.com/owin-support-for-the-web-api-2-and-mvc-5-integrations-in-autofac/
            app.UseAutofacMvc();

            //MVC: Set MVC DI resolver to use our Autofac container
            var dependencyResolver = new AutofacDependencyResolver(GlobalContainer.IocContainer);            
            System.Web.Mvc.DependencyResolver.SetResolver(dependencyResolver); //TODO: Still needed with OWIN?
        }


        public static IContainer InitializeAutofacContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            // Register all Autofact Modules (OWin, WebApi, WebMvc, ....)
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());

            return builder.Build();
        }

        #endregion

    }
}