using System;
using System.Reflection;

using Autofac;
using Autofac.Integration.WebApi;

namespace MySite.AutofacDI
{
    public class WebApiModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register API controllers using assembly scanning.            
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
        }
    }
}