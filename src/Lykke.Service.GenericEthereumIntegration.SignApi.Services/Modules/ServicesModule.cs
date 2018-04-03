using Autofac;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Services.Modules
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder
                .RegisterType<SignService>()
                .As<ISignService>()
                .SingleInstance();

            builder
                .RegisterType<WalletService>()
                .As<IWalletService>()
                .SingleInstance();
        }
    }
}
