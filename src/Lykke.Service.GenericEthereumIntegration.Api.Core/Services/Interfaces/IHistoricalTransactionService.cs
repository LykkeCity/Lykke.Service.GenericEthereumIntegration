using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IHistoricalTransactionService
    {
        Task<(IEnumerable<HistoricalTransactionDto> transactions, string assetId)> GetIncomingHistory(string address, int take, string afterHash);

        Task<(IEnumerable<HistoricalTransactionDto> transactions, string assetId)> GetOutgoingHistory(string address, int take, string afterHash);
    }
}
