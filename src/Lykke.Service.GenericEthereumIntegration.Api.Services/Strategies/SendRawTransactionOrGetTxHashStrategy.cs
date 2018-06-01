using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies
{
    internal class SendRawTransactionOrGetTxHashStrategy : ISendRawTransactionOrGetTxHashStrategy
    {
        private readonly IBlockchainService _blockchainService;

        
        public SendRawTransactionOrGetTxHashStrategy(
            IBlockchainService blockchainService)
        {
            _blockchainService = blockchainService;
        }


        public async Task<string> ExecuteAsync(string signedTxData)
        {
            var txHash = _blockchainService.GetTransactionHash(signedTxData);
            var receipt = await _blockchainService.TryGetTransactionReceiptAsync(txHash);

            if (receipt == null)
            {
                await _blockchainService.SendRawTransactionAsync(signedTxData);
            }

            return txHash;
        }
    }
}
