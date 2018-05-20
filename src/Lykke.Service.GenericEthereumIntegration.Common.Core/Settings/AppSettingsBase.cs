using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.SlackNotifications;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Settings
{
    public abstract class AppSettingsBase
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public IntegrationSettings Integration { get; set; }
        
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
