using JetBrains.Annotations;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Core.Settings.Service
{
    public class GenericEthereumIntegrationSignApiSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public DbSettings Db { get; set; }
    }
}
