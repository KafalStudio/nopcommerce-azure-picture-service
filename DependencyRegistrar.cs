using Autofac;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Media;

namespace Nop.Plugin.Pictures.AzurePictureService
{
    public class DependencyRegistrar : IDependencyRegistrar
    {

        public DependencyRegistrar()
        {
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<AzurePictureService>().As<IPictureService>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
