using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings;


namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Settings
{
    [UsedImplicitly]
    public class AppSettings : AppSettingsBase
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public ApiSettings Api { get; set; }
    }
}
