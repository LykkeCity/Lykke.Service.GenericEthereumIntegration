using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces
{
    public interface IIndexedBlockRepository
    {
        Task<bool> DeleteIfExistsAsync(BigInteger blockNumber);

        Task<IEnumerable<IndexedBlockDto>> GetAsync(IEnumerable<BigInteger> blockNumbers);

        Task<IndexedBlockDto> TryGetAsync(BigInteger blockNumber);
    }
}
