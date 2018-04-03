using System.Numerics;
using System.Threading.Tasks;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IGasPriceOracleService
    {
        Task<BigInteger> CalculateGasPriceAsync(string to, BigInteger amount);
    }
}
