using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IObservableAddressService
    {
        Task AddToIncomingObservationListAsync([NotNull] string address);

        Task AddToOutgoingObservationListAsync([NotNull] string address);

        Task DeleteFromIncomingObservationListAsync([NotNull] string address);

        Task DeleteFromOutgoingObservationListAsync([NotNull] string address);
    }
}
