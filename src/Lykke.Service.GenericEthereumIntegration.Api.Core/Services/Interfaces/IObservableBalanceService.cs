using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IObservableBalanceService
    {
        Task BeginObservationAsync(string address);

        Task EndObservationAsync(string address);

        Task<(IEnumerable<ObservableBalanceDto> balances, string assetId, string continuationToken)> GetBalancesAsync(int take, string continuationToken);
    }
}
