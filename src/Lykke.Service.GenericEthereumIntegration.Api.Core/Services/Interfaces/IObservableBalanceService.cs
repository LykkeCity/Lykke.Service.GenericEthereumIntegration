using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IObservableBalanceService
    {
        Task BeginObservationAsync([NotNull] string address);

        Task EndObservationAsync([NotNull] string address);

        Task<(IEnumerable<ObservableBalanceDto> Balances, string AssetId, string ContinuationToken)> GetBalancesAsync(int take, string continuationToken);
    }
}
