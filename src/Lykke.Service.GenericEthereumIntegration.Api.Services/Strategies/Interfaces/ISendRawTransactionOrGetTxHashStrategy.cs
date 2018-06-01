using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies.Interfaces
{
    internal interface ISendRawTransactionOrGetTxHashStrategy
    {
        [ItemNotNull]
        Task<string> ExecuteAsync([NotNull] string signedTxData);
    }
}
