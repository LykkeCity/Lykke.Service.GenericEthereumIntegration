using System;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Interfaces;


namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface ITransactionService
    {
        [ItemNotNull]
        Task<string> BroadcastTransactionAsync(Guid operationId, [NotNull] string signedTxData);

        [ItemNotNull]
        Task<string> BuildTransactionAsync(BigInteger amount, [NotNull] string fromAddress, bool includeFee, Guid operationId, [NotNull] string toAddress);

        Task DeleteTransactionAsync(Guid operationId);

        [ItemNotNull]
        Task<ITransactionAggregate> GetTransactionAsync(Guid operationId);

        [ItemNotNull]
        Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId);
    }
}
