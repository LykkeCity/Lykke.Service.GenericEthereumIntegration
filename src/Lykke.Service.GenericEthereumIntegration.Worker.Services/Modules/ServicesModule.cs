using Autofac;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Services.Modules
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();
        }
    }
}
