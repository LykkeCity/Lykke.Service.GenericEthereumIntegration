using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles.Interfaces
{
    public interface IBalanceObserverRole : IActorRole
    {
        Task CheckAndUpdateBalanceAsync(CheckAndUpdateBalance message);
    }
}
