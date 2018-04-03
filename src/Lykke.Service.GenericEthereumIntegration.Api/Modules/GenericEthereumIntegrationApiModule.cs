using Autofac;
using Lykke.Common.Chaos;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;

namespace Lykke.Service.GenericEthereumIntegration.Api.Modules
{
    public class GenericEthereumIntegrationApiModule : Module
    {
        private readonly GenericEthereumIntegrationApiSettings _settings;


        public GenericEthereumIntegrationApiModule(
            GenericEthereumIntegrationApiSettings settings)
        {
            _settings = settings;
        }


        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(x => _settings)
                .AsSelf()
                .SingleInstance();

            builder
                .Register(x => _settings.Asset)
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterChaosKitty(_settings.Chaos);
        }
    }
}
