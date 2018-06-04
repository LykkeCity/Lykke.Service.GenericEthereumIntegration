using Autofac;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Settings.Service;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Modules
{
    public class GenericEthereumIntegrationWorkerModule : Module
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GenericEthereumIntegrationWorkerSettings _settings;

        public GenericEthereumIntegrationWorkerModule(
            GenericEthereumIntegrationWorkerSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {

        }
    }
}
