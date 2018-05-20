using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;


namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync([NotNull] TransactionAggregate aggregate);

        Task<bool> DeleteIfExistsAsync(Guid operationId);

        [ItemNotNull]
        Task<IEnumerable<TransactionAggregate>> GetAllForOperationAsync(Guid operationId);

        [ItemNotNull]
        Task<IEnumerable<TransactionAggregate>> GetAllInProgressAsync();

        [ItemCanBeNull]
        Task<TransactionAggregate> TryGetAsync([NotNull] string transactionHash);

        Task UpdateAsync([NotNull] TransactionAggregate aggregate);
    }
}
