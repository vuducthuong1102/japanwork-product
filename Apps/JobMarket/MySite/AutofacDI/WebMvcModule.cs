using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using Autofac;
using Autofac.Integration.Mvc;

namespace MySite.AutofacDI
{
    public class WebMvcModule : Autofac.Module
    {
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            // Register MVC controllers using assembly scanning.
            builder.RegisterControllers(System.Reflection.Assembly.GetExecutingAssembly());

            // Dependency injection module that registers abstractions for common web application properties. 
            // Ref: http://autofac.org/apidoc/html/DA6737B.htm
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterFilterProvider();
        }
    }
}