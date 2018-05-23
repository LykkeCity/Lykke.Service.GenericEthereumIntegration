namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration
{
    public class IntegrationSettings
    {
        public AssetSettings Asset { get; set; }

        public string Name { get; set; }

        public RpcNodeSettings RpcNode { get; set; }
    }
}
