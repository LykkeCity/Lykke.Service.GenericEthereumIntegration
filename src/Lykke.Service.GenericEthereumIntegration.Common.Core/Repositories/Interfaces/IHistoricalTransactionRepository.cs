using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces
{
    public interface IHistoricalTransactionRepository
    {
        Task InsertOrReplaceAsync([NotNull] HistoricalTransactionDto transaction);

        [ItemNotNull]
        Task<IEnumerable<HistoricalTransactionDto>> GetBlockHistoryAsync(BigInteger blockNumber);

        [ItemNotNull]
        Task<IEnumerable<HistoricalTransactionDto>> GetIncomingHistoryAsync([NotNull] string address, int take, string afterHash);

        [ItemNotNull]
        Task<IEnumerable<HistoricalTransactionDto>> GetOutgoingHistoryAsync([NotNull] string address, int take, string afterHash);

        Task ClearBlockAsync(BigInteger blockNumber);
    }
}
