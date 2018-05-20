using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces
{
    public interface IIndexedBlockRepository
    {
        Task<bool> DeleteIfExistsAsync(BigInteger blockNumber);

        [ItemNotNull]
        Task<IEnumerable<IndexedBlockDto>> GetAsync([NotNull] IEnumerable<BigInteger> blockNumbers);

        [ItemNotNull]
        Task<IndexedBlockDto> TryGetAsync(BigInteger blockNumber);
    }
}
