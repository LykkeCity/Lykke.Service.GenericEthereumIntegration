using System.Threading.Tasks;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces
{
    public interface IObservableAddressRepository
    {
        Task<bool> ExistsInIncomingObservationListAsync(string address);

        Task<bool> ExistsInOutgoingObservationListAsync(string address);

        Task<bool> TryAddToIncomingObservationListAsync(string address);

        Task<bool> TryAddToOutgoingObservationListAsync(string address);

        Task<bool> TryDeleteFromIncomingObservationListAsync(string address);

        Task<bool> TryDeleteFromOutgoingObservationListAsync(string address);
    }
}
