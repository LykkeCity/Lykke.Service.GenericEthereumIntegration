using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Service;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Core.Settings.Service
{
    [UsedImplicitly]
    public class GenericEthereumIntegrationWorkerSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public DbSettings Db { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public int ConfirmationLevel { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public int NrOfBalanceObservers { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public int NrOfOperationMonitors { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public int NrOfTransactionIndexers { get; set; }
    }
}
