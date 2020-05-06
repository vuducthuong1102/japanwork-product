//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//using Owin;
//using Microsoft.Owin;

//using Autofac;

//using StackExchange.Redis.Extensions.Core;
//using StackExchange.Redis.Extensions.Newtonsoft;

//using MySite.Services;
//using MySite.Owin.Middlewares;
//using MySite.Caching;
//using MySite.Caching.Providers;

//namespace MySite.AutofacDI
//{
//    public class CoreModule : Module
//    {
//        protected override void Load(ContainerBuilder builder)
//        {
//            //Injecting Owin middleware
//            //builder.Register(c => new Logger()).As<ILogger>().InstancePerLifetimeScope();
//            //builder.RegisterType<LoggerMiddleware>().InstancePerRequest();
//            //builder.RegisterType<CorsMiddleware>().InstancePerRequest();

//            //Injecting sample service
//            builder.RegisterType<SampleService>().As<ISampleService>();

//            //Injecting OwinContext object
//            builder.Register(ctx => HttpContext.Current.GetOwinContext()).As<IOwinContext>();

//            //Injecting the Redis client service
//            builder.RegisterType<NewtonsoftSerializer>().As<ISerializer>().SingleInstance();
//            builder.RegisterType<StackExchangeRedisCacheClient>().As<ICacheClient>().SingleInstance();

//            builder.Register(c => CacheProviderManager.Default).As<ICacheProvider>().SingleInstance();
//        }
//    }
//}