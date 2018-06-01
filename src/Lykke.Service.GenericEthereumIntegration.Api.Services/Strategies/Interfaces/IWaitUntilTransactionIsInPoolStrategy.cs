using System.Threading.Tasks;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies.Interfaces
{
    internal interface IWaitUntilTransactionIsInPoolStrategy
    {
        Task ExecuteAsync(string txHash);
    }
}
