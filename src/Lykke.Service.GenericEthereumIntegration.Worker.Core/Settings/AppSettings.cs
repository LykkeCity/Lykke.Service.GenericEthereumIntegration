using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.SlackNotifications;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Settings.Service;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Core.Settings
{
    [UsedImplicitly]
    public class AppSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public GenericEthereumIntegrationWorkerSettings GenericEthereumIntegrationWorker { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
