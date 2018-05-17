using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Settings.Service;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Core.Settings
{
    [UsedImplicitly]
    public class AppSettings : AppSettingsBase
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public GenericEthereumIntegrationWorkerSettings GenericEthereumIntegrationWorker { get; set; }
    }
}
