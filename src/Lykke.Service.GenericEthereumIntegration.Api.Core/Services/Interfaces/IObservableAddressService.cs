using System.Threading.Tasks;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IObservableAddressService
    {
        Task AddToIncomingObservationListAsync(string address);

        Task AddToOutgoingObservationListAsync(string address);

        Task DeleteFromIncomingObservationListAsync(string address);

        Task DeleteFromOutgoingObservationListAsync(string address);
    }
}
