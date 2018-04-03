using System;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<string> BroadcastTransactionAsync(Guid operationId, string signedTxData);

        Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress);

        Task DeleteTransactionStateAsync(Guid operationId);

        Task<TransactionAggregate> GetTransactionAsync(Guid operationId);

        Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId);
    }
}
