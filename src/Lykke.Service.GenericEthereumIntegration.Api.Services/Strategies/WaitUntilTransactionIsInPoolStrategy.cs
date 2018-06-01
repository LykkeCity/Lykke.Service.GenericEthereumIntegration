using System;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Polly;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies
{
    internal class WaitUntilTransactionIsInPoolStrategy : IWaitUntilTransactionIsInPoolStrategy
    {
        private readonly IBlockchainService _blockchainService;
        private readonly int _delayFactor;


        public WaitUntilTransactionIsInPoolStrategy(
            IBlockchainService blockchainService,
            int delayFactor)
        {
            _blockchainService = blockchainService;
            _delayFactor = delayFactor;
        }
        
        public async Task ExecuteAsync(string txHash)
        {
            var retryPolicy = Policy
                .HandleResult(false)
                .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromMilliseconds(_delayFactor * retryAttempt));

            var txIsInPoolOrMined = await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    return await _blockchainService.CheckIfBroadcastedAsync(txHash)
                        || await _blockchainService.TryGetTransactionReceiptAsync(txHash) != null;
                }
                catch (Exception)
                {
                    return false;
                }
            });

            if (!txIsInPoolOrMined)
            {
                throw new UnsupportedEdgeCaseException("Transaction didn't appear in memory pool in the specified period of time.");
            }
        }
    }
}
