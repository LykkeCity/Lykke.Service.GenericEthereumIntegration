using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IGasPriceOracleService
    {
        Task<BigInteger> CalculateGasPriceAsync([NotNull] string to, BigInteger amount);
    }
}
