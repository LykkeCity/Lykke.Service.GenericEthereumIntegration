using AzureStorage.Queue;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.DTOs;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Repositories
{
    [UsedImplicitly]
    public class OperationMonitorTaskRepository : TaskRepositoryBase<OperationMonitorTaskDto>, IOperationMonitorTaskRepository
    {
        internal OperationMonitorTaskRepository(IQueueExt queue) 
            : base(queue)
        {
        }
    }
}
