using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Interfaces;


namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync([NotNull] ITransactionAggregate aggregate);

        Task<bool> DeleteIfExistsAsync(Guid operationId);

        [ItemNotNull]
        Task<IEnumerable<ITransactionAggregate>> GetAllForOperationAsync(Guid operationId);

        [ItemNotNull]
        Task<IEnumerable<ITransactionAggregate>> GetAllInProgressAsync();

        [ItemCanBeNull]
        Task<ITransactionAggregate> TryGetAsync([NotNull] string transactionHash);

        Task UpdateAsync([NotNull] ITransactionAggregate aggregate);
    }
}
