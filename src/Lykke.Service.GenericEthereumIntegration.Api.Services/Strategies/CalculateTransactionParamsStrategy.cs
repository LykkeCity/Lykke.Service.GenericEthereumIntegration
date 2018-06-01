using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Utils;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies
{
    internal class CalculateTransactionParamsStrategy : ICalculateTransactionParamsStrategy
    {
        private readonly BigInteger _gasAmount;
        private readonly IGasPriceOracleService _gasPriceOracleService;

        public CalculateTransactionParamsStrategy(
            IGasPriceOracleService gasPriceOracleService,
            ApiSettings settings)
        {
            _gasPriceOracleService = gasPriceOracleService;
            _gasAmount = BigInteger.Parse(settings.GasAmount);
        }


        public (BigInteger Amount, BigInteger Fee, BigInteger GasPrice) Execute(ITransactionAggregate transaction, decimal feeFactor)
        {
            var amount = transaction.Amount;
            var fee = transaction.Fee;
            var gasPrice = transaction.GasPrice;
            var includeFee = transaction.IncludeFee;


            if (includeFee)
            {
                amount += fee;
            }

            fee = FeeCalculator.Calculate(gasPrice, _gasAmount, feeFactor);
            
            if (includeFee)
            {
                amount -= fee;
            }

            return (amount, fee, gasPrice);
        }

        public async Task<(BigInteger Amount, BigInteger Fee, BigInteger GasPrice)> ExecuteAsync(BigInteger amount, bool includeFee, string toAddress)
        {
            var gasPrice = await _gasPriceOracleService.CalculateGasPriceAsync(toAddress, amount);
            var fee = gasPrice * _gasAmount;

            if (includeFee)
            {
                amount -= fee;
            }

            #region Result validation

            if (amount <= 0)
            {
                throw new BadRequestException($"Amount [{amount}] is too small.");
            }

            #endregion

            return (amount, fee, gasPrice);
        }
    }
}
