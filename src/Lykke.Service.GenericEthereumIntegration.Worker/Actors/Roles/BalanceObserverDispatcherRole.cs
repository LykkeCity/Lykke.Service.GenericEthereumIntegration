using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Settings.Service;


namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles
{
    [UsedImplicitly]
    public class BalanceObserverDispatcherRole : IBalanceObserverDispatcherRole
    {
        private readonly IBalanceObserverTaskRepository _balanceObserverTaskRepository;
        
        private int _inProgressOperationsCount;
        

        public BalanceObserverDispatcherRole(
            IBalanceObserverTaskRepository balanceObserverTaskRepository,
            GenericEthereumIntegrationWorkerSettings settings)
        {
            _balanceObserverTaskRepository = balanceObserverTaskRepository;
            InProgressOperationsLimit = settings.NrOfBalanceObservers;
        }


        public int InProgressOperationsLimit { get; }


        public async Task<IEnumerable<CheckAndUpdateBalance>> BeginNextTasksProcessingAsync()
        {
            if (_inProgressOperationsCount > InProgressOperationsLimit)
            {
                throw new InvalidOperationException($"{nameof(InProgressOperationsLimit)} [{InProgressOperationsLimit}] exceeded [{_inProgressOperationsCount}].");
            }

            var result = new List<CheckAndUpdateBalance>();

            for (var i = 0; i < InProgressOperationsLimit - _inProgressOperationsCount; i++)
            {
                var (task, completionToken) = await _balanceObserverTaskRepository.TryGetAsync(TimeSpan.FromMinutes(1));

                if (task != null)
                {
                    result.Add(new CheckAndUpdateBalance
                    (
                        address: task.Address,
                        blockNumber: task.BlockNumber,
                        completionToken: completionToken
                    ));
                }
                else
                {
                    break;
                }
            }

            _inProgressOperationsCount += result.Count;

            return result;
        }

        public void CompleteTaskProcessing()
        {
            if (_inProgressOperationsCount == 0)
            {
                throw new InvalidOperationException($"{nameof(_inProgressOperationsCount)} is 0.");
            }

            _inProgressOperationsCount--;
        }
    }
}
