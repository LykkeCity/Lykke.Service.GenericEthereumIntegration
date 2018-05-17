using System;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration;
using Nethereum.Parity;

namespace Lykke.Service.GenericEthereumIntegration.Common.Services.Factories
{
    public class BlockchainServiceFactory
    {
        private readonly RpcNodeSettings _rpcNodeSettings;

        public BlockchainServiceFactory(
            RpcNodeSettings rpcNodeSettings)
        {
            _rpcNodeSettings = rpcNodeSettings;
        }

        private IBlockchainService BuildGeth()
        {
            throw new NotSupportedException("Geth is not supported yet");
        }

        private IBlockchainService BuildParity()
        {
            var web3Parity = new Web3Parity(_rpcNodeSettings.Url);

            return new BlockchainServiceParity(web3Parity);
        }

        public IBlockchainService Build()
        {
            var nodeType = _rpcNodeSettings.Type;

            switch (nodeType.ToLowerInvariant())
            {
                case "geth":
                    return BuildGeth();
                case "parity":
                    return BuildParity();
                default:
                    throw new NotSupportedException($"{nodeType} is not supported Ethereum client.");
            }
        }
    }
}
