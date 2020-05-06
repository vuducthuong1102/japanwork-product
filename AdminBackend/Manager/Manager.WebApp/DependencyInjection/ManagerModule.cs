using Autofac;
using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.MsSqlStores;
using MsSql.AspNet.Identity.Stores;

namespace Manager.WebApp.DependencyInjection
{
    public class ManagerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //For system
            builder.RegisterType<IdentityStore>().As<IIdentityStore>();

            //For business
			builder.RegisterType<StoreReport>().As<IStoreReport>();
			builder.RegisterType<StoreNavigation>().As<IStoreNavigation>();
            builder.RegisterType<StoreUser>().As<IStoreUser>();
            builder.RegisterType<StorePost>().As<IStorePost>();

            builder.RegisterType<StoreConversation>().As<IStoreConversation>();
            builder.RegisterType<StoreConversationReply>().As<IStoreConversationReply>();

            //builder.RegisterType<StoreProject>().As<IStoreProject>();
            //builder.RegisterType<StoreProjectCategory>().As<IStoreProjectCategory>();
            //builder.RegisterType<StoreUnit>().As<IStoreUnit>();

            //builder.RegisterType<StorePage>().As<IStorePage>();
            //builder.RegisterType<StoreWidget>().As<IStoreWidget>();
            //builder.RegisterType<StorePageTemplate>().As<IStorePageTemplate>();
        }
    }
}