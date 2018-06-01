using System;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies
{
    public class RegisterTransactionStrategy : IRegisterTransactionStrategy
    {
        private readonly ITransactionRepository _transactionRepository;

        public RegisterTransactionStrategy(
            ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public Task ExecuteAsync(
            BigInteger amount,
            BigInteger fee,
            string fromAddress,
            BigInteger gasPrice,
            bool includeFee,
            BigInteger nonce,
            Guid operationId,
            string toAddress,
            string txData)
        {
            var operationTransaction = TransactionAggregate.Build
            (
                amount: amount,
                fee: fee,
                fromAddress: fromAddress,
                gasPrice: gasPrice,
                includeFee: includeFee,
                nonce: nonce,
                operationId: operationId,
                toAddress: toAddress,
                txData: txData
            );

            return _transactionRepository.AddAsync(operationTransaction);
        }
    }
}
