using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces
{
    public interface IObservableBalanceRepository
    {
        Task<bool> DeleteIfExistsAsync([NotNull] string address);

        Task<bool> ExistsAsync(string address);

        Task<(IEnumerable<ObservableBalanceDto> Balances, string ContinuationToken)> GetAllAsync(int take, string continuationToken);

        Task<(IEnumerable<ObservableBalanceDto> Balances, string ContinuationToken)> GetAllWithNonZeroAmountAsync(int take, string continuationToken);

        Task<bool> TryAddAsync([NotNull] string address);

        [ItemCanBeNull]
        Task<ObservableBalanceDto> TryGetAsync([NotNull] string address);

        Task UpdateAmountAsync([NotNull] string address, BigInteger amount, BigInteger blockNumbber);
    }
}
