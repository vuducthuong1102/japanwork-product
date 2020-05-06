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
            builder.RegisterType<StoreUser>().As<IStoreUser>();

            //For business
            //builder.RegisterType<StoreMember>().As<IStoreMember>();
            //builder.RegisterType<StorePost>().As<IStorePost>();
            //builder.RegisterType<StoreProject>().As<IStoreProject>();
            //builder.RegisterType<StoreProjectCategory>().As<IStoreProjectCategory>();
            //builder.RegisterType<StoreUnit>().As<IStoreUnit>();

            //Others
            builder.RegisterType<StoreAgency>().As<IStoreAgency>();
            builder.RegisterType<StoreEmailServer>().As<IStoreEmailServer>();
            builder.RegisterType<StoreEmailMessage>().As<IStoreEmailMessage>();
            builder.RegisterType<StoreEmailBusiness>().As<IStoreEmailBusiness>();

            builder.RegisterType<StoreConversation>().As<IStoreConversation>();
            builder.RegisterType<StoreConversationReply>().As<IStoreConversationReply>();            

            //builder.RegisterType<StorePage>().As<IStorePage>();
            //builder.RegisterType<StoreWidget>().As<IStoreWidget>();
            //builder.RegisterType<StoreFooter>().As<IStoreFooter>();
            //builder.RegisterType<StorePageTemplate>().As<IStorePageTemplate>();
        }
    }
}