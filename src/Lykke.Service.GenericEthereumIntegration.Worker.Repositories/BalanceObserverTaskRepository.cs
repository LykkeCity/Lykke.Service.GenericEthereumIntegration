using AzureStorage.Queue;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.DTOs;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Repositories
{
    [UsedImplicitly]
    public class BalanceObserverTaskRepository : TaskRepositoryBase<BalanceObserverTaskDto>, IBalanceObserverTaskRepository
    {
        public BalanceObserverTaskRepository(IQueueExt queue) 
            : base(queue)
        {

        }
    }
}
