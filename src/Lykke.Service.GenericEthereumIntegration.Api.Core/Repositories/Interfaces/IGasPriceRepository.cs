using System.Numerics;
using System.Threading.Tasks;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Repositories.Interfaces
{
    public interface IGasPriceRepository
    {
        Task<(BigInteger Min, BigInteger Max)> GetOrAddAsync(BigInteger min, BigInteger max);
    }
}
