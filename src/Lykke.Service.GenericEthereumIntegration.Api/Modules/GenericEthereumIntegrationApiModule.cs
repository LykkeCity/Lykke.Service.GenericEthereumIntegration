using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration;

namespace Lykke.Service.GenericEthereumIntegration.Api.Modules
{
    public class GenericEthereumIntegrationApiModule : Module
    {
        private readonly ApiSettings _apiSettings;
        private readonly IntegrationSettings _integrationSettings;


        public GenericEthereumIntegrationApiModule(
            [NotNull] ApiSettings apiSettings,
            [NotNull] IntegrationSettings integrationSettings)
        {
            _apiSettings = apiSettings;
            _integrationSettings = integrationSettings;
        }


        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(x => _apiSettings)
                .AsSelf()
                .SingleInstance();

            builder
                .Register(x => _integrationSettings.Asset)
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterChaosKitty(_apiSettings.Chaos);
        }
    }
}
