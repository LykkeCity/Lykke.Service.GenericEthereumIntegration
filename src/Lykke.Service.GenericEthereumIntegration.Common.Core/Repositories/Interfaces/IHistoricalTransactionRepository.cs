using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces
{
    public interface IHistoricalTransactionRepository
    {
        Task InsertOrReplaceAsync(HistoricalTransactionDto transaction);

        Task<IEnumerable<HistoricalTransactionDto>> GetBlockHistoryAsync(BigInteger blockNumber);

        Task<IEnumerable<HistoricalTransactionDto>> GetIncomingHistory(string address, int take, string afterHash);


        Task<IEnumerable<HistoricalTransactionDto>> GetOutgoingHistory(string address, int take, string afterHash);

        Task ClearBlockAsync(BigInteger blockNumber);
    }
}
