using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces
{
    public interface IObservableAddressRepository
    {
        Task<bool> ExistsInIncomingObservationListAsync([NotNull] string address);

        Task<bool> ExistsInOutgoingObservationListAsync([NotNull] string address);

        Task<bool> TryAddToIncomingObservationListAsync([NotNull] string address);

        Task<bool> TryAddToOutgoingObservationListAsync([NotNull] string address);

        Task<bool> TryDeleteFromIncomingObservationListAsync([NotNull] string address);

        Task<bool> TryDeleteFromOutgoingObservationListAsync([NotNull] string address);
    }
}
