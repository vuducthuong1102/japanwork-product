using Autofac;
using SingleSignOn.DB.Sql.Stores;

namespace SingleSignOn.AutofacDI
{
    public class SqlModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StoreUser>().As<IStoreUser>();
            builder.RegisterType<StorePost>().As<IPostStore>();
            builder.RegisterType<StoreLocation>().As<IStoreLocation>();
            builder.RegisterType<StoreDocumentApi>().As<IStoreDocumentApi>();
            builder.RegisterType<StoreSystemEmail>().As<IStoreSystemEmail>();
        }
    }
}