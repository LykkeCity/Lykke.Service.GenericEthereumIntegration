using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;


namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(TransactionAggregate aggregate);

        Task<bool> DeleteIfExistsAsync(Guid operationId);

        Task<IEnumerable<TransactionAggregate>> GetAllForOperationAsync(Guid operationId);

        Task<IEnumerable<TransactionAggregate>> GetAllInProgressAsync();

        Task<TransactionAggregate> TryGetAsync(string transactionHash);

        Task UpdateAsync(TransactionAggregate aggregate);
    }
}
