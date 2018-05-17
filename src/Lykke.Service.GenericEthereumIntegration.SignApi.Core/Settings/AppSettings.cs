using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Settings.Service;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Core.Settings
{
    [UsedImplicitly]
    public class AppSettings : AppSettingsBase
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public GenericEthereumIntegrationSignApiSettings GenericEthereumIntegrationSignApi { get; set; }
    }
}
