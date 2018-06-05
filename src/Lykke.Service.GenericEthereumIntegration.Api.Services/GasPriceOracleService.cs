using System;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
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
            [NotNull] IBlockchainService blockchainService,
            [NotNull] IGasPriceRepository gasPriceRepository,
            [NotNull] ApiSettings settings)
        {
            _blockchainService = blockchainService;
            _defaultMaxGasPrice = BigInteger.Parse(settings.DefaultMaxGasPrice);
            _defaultMinGasPrice = BigInteger.Parse(settings.DefaultMinGasPrice);
            _gasPriceRepository = gasPriceRepository;
        }

        
        public async Task<BigInteger> CalculateGasPriceAsync(string to, BigInteger amount)
        {
            #region Validation
            
            if (to.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(to));
            }

            if (!await AddressChecksum.ValidateAsync(to))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(to));
            }

            if (amount <= 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanZero, nameof(amount));
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
