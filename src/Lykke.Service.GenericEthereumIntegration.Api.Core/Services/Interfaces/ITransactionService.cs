using System;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface ITransactionService
    {
        [ItemNotNull]
        Task<string> BroadcastTransactionAsync(Guid operationId, [NotNull] string signedTxData);

        [ItemNotNull]
        Task<string> BuildTransactionAsync(BigInteger amount, [NotNull] string fromAddress, bool includeFee, Guid operationId, [NotNull] string toAddress);

        Task DeleteTransactionStateAsync(Guid operationId);

        [ItemNotNull]
        Task<TransactionAggregate> GetTransactionAsync(Guid operationId);

        [ItemNotNull]
        Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId);
    }
}
