using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles.Interfaces
{
    public interface IOperationMonitorDispatcherRole : IActorRole
    {
        int InProgressOperationsLimit { get; }


        Task<IEnumerable<CheckAndUpdateOperation>> BeginNextTasksProcessingAsync();

        void CompleteTaskProcessing();
    }
}
