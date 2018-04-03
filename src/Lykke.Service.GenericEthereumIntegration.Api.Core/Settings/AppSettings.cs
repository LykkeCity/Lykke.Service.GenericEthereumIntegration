using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.SlackNotifications;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Settings
{
    [UsedImplicitly]
    public class AppSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public GenericEthereumIntegrationApiSettings GenericEthereumIntegrationApi { get; set; }
        
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
