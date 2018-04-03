using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Core.Settings.Service
{
    public class DbSettings
    {
        [AzureTableCheck, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string LogsConnString { get; set; }
    }
}
