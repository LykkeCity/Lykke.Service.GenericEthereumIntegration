using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces
{
    public interface IObservableBalanceRepository
    {
        Task<bool> DeleteIfExistsAsync(string address);

        Task<bool> ExistsAsync(string address);

        Task<(IEnumerable<ObservableBalanceDto> Balances, string ContinuationToken)> GetAllAsync(int take, string continuationToken);

        Task<(IEnumerable<ObservableBalanceDto> Balances, string ContinuationToken)> GetAllWithNonZeroAmountAsync(int take, string continuationToken);

        Task<bool> TryAddAsync(string address);

        Task<ObservableBalanceDto> TryGetAsync(string address);

        Task UpdateAmountAsync(string address, BigInteger amount, BigInteger blockNumbber);
    }
}
