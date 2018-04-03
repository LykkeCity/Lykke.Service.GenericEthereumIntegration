using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Service
{
    [UsedImplicitly]
    public class DbSettings
    {
        [AzureTableCheck, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string DataConnString { get; set; }

        [AzureTableCheck, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string LogsConnString { get; set; }
    }
}
