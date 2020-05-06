using System;

using Autofac;

using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.MsSqlStores;
using MsSql.AspNet.Identity.Repositories;

namespace Manager.WebApp.DependencyInjection
{
    public class StoreModule: Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IdentityStore>().As<IIdentityStore>();
            builder.RegisterType<StoreRole>().As<IStoreRole>();
            builder.RegisterType<AccessRolesStore>().As<IAccessRolesStore>();
            builder.RegisterType<ActivityStore>().As<IActivityStore>();
            builder.RegisterType<MsSqlStore>().As<IMsSqlStore>();
            builder.RegisterType<MsSqlFrontEndStore>().As<IMsSqlFrontEndStore>();
        }
    }
}