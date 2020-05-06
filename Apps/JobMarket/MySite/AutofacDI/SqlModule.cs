using Autofac;

namespace MySite.AutofacDI
{
    public class SqlModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<UserStore>().As<IUserStore>();

            //builder.RegisterType<PostStore>().As<IPostStore>();
        }
    }
}