using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Service;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service
{
    public class ApiSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public ChaosSettings Chaos { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public DbSettings Db { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string DefaultMaxGasPrice { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string DefaultMinGasPrice { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string GasAmount { get; set; }
    }
}
