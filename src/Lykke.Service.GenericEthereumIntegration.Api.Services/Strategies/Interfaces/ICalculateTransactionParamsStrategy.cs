using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies.Interfaces
{
    internal interface ICalculateTransactionParamsStrategy
    {
        [Pure]
        (BigInteger Amount, BigInteger Fee, BigInteger GasPrice) Execute([NotNull] ITransactionAggregate transaction, decimal feeFactor);

        Task<(BigInteger Amount, BigInteger Fee, BigInteger GasPrice)> ExecuteAsync(BigInteger amount, bool includeFee, [NotNull] string toAddress);
    }
}
