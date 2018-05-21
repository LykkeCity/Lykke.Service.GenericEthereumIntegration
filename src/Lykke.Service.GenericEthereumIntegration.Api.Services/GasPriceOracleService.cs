using System;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;

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
            ApiSettings settings)
        {
            _blockchainService = blockchainService;
            _defaultMaxGasPrice = BigInteger.Parse(settings.DefaultMaxGasPrice);
            _defaultMinGasPrice = BigInteger.Parse(settings.DefaultMinGasPrice);
            _gasPriceRepository = gasPriceRepository;
        }


        public async Task<BigInteger> CalculateGasPriceAsync(string to, BigInteger amount)
        {
            #region Validation
            
            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentException("Should not be null or empty.", nameof(to));
            }

            if (!AddressChecksum.ValidateAsync(to).Result)
            {
                throw new ArgumentException("Should be a valid address.", nameof(to));
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Should be greater than zero.", nameof(amount));
            }
            
            #endregion
            
            var (minGasPrice, maxGasPrice) = await _gasPriceRepository.GetOrAddAsync
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
