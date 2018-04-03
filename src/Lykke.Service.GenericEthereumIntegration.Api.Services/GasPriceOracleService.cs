using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services
{
    public class GasPriceOracleService : IGasPriceOracleService
    {
        private readonly IBlockchainService _blockchainService;
        private readonly BigInteger _defaultMaxGasPrice;
        private readonly BigInteger _defaultMinGasPrice;
        private readonly IGasPriceRepository _gasPriceRepository;


        public GasPriceOracleService(
            IBlockchainService blockchainService,
            IGasPriceRepository gasPriceRepository,
            GenericEthereumIntegrationApiSettings settings)
        {
            _blockchainService = blockchainService;
            _defaultMaxGasPrice = BigInteger.Parse(settings.DefaultMaxGasPrice);
            _defaultMinGasPrice = BigInteger.Parse(settings.DefaultMinGasPrice);
            _gasPriceRepository = gasPriceRepository;
        }


        public async Task<BigInteger> CalculateGasPriceAsync(string to, BigInteger amount)
        {
            (var minGasPrice, var maxGasPrice) = await _gasPriceRepository.GetOrAddAsync
            (
                min: _defaultMinGasPrice,
                max: _defaultMaxGasPrice
            );


            var gasPrice = await _blockchainService.EstimateGasPriceAsync(to, amount);
            
            if (gasPrice <= minGasPrice)
            {
                gasPrice = minGasPrice;
            }
            else if (gasPrice >= maxGasPrice)
            {
                gasPrice = maxGasPrice;
            }

            return gasPrice;
        }
    }
}
