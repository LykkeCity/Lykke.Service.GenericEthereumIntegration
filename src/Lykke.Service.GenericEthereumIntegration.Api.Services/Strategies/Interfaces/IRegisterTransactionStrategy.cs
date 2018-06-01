using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies.Interfaces
{
    public interface IRegisterTransactionStrategy
    {
        Task ExecuteAsync(
            BigInteger amount,
            BigInteger fee,
            string fromAddress,
            BigInteger gasPrice,
            bool includeFee,
            BigInteger nonce,
            Guid operationId,
            string toAddress,
            string txData);
    }
}
