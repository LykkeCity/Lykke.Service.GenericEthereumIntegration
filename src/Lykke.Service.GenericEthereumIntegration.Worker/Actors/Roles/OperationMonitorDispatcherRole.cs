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
    public class OperationMonitorDispatcherRole : IOperationMonitorDispatcherRole
    {
        private readonly IOperationMonitorTaskRepository _operationMonitorTaskRepository;

        private int _inProgressOperationsCount;


        public OperationMonitorDispatcherRole(
            IOperationMonitorTaskRepository operationMonitorTaskRepository,
            GenericEthereumIntegrationWorkerSettings settings)
        {
            _operationMonitorTaskRepository = operationMonitorTaskRepository;
            InProgressOperationsLimit = settings.NrOfOperationMonitors;
        }


        public int InProgressOperationsLimit { get; }


        public async Task<IEnumerable<CheckAndUpdateOperation>> BeginNextTasksProcessingAsync()
        {
            if (_inProgressOperationsCount > InProgressOperationsLimit)
            {
                throw new InvalidOperationException($"{nameof(InProgressOperationsLimit)} [{InProgressOperationsLimit}] exceeded [{_inProgressOperationsCount}].");
            }

            var result = new List<CheckAndUpdateOperation>();

            for (var i = 0; i < InProgressOperationsLimit - _inProgressOperationsCount; i++)
            {
                var (task, completionToken) = await _operationMonitorTaskRepository.TryGetAsync(TimeSpan.FromMinutes(1));

                if (task != null)
                {
                    result.Add(new CheckAndUpdateOperation
                    (
                        transactionBlock: task.TransactionBlock,
                        transactionError: task.TransactionError,
                        transactionFailed: task.TransactionFailed,
                        transactionHash: task.TransactionHash,
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
