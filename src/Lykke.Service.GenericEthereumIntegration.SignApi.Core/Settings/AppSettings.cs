using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.SlackNotifications;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Settings.Service;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Core.Settings
{
    [UsedImplicitly]
    public class AppSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public GenericEthereumIntegrationSignApiSettings GenericEthereumIntegrationSignApi { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
