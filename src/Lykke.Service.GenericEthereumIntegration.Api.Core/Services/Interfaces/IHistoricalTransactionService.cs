using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IHistoricalTransactionService
    {
        Task<(IEnumerable<HistoricalTransactionDto> transactions, string assetId)> GetIncomingHistoryAsync([NotNull] string address, int take, string afterHash);

        Task<(IEnumerable<HistoricalTransactionDto> transactions, string assetId)> GetOutgoingHistoryAsync([NotNull] string address, int take, string afterHash);
    }
}
