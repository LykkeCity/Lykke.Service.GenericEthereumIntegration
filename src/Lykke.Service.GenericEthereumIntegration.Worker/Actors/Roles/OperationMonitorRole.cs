using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles
{
    [UsedImplicitly]
    public class OperationMonitorRole : IOperationMonitorRole
    {
        private readonly IOperationMonitorTaskRepository _operationMonitorTaskRepository;
        private readonly ITransactionRepository _transactionRepository;



        public OperationMonitorRole(
            IOperationMonitorTaskRepository operationMonitorTaskRepository,
            ITransactionRepository transactionRepository)
        {
            _operationMonitorTaskRepository = operationMonitorTaskRepository;
            _transactionRepository = transactionRepository;
        }


        public async Task CheckAndUpdateOperationAsync(CheckAndUpdateOperation message)
        {
            var transactionAggregate = await _transactionRepository.TryGetAsync(message.TransactionHash);

            if (transactionAggregate != null)
            {
                if (!message.TransactionFailed)
                {
                    transactionAggregate.OnCompleted(message.TransactionBlock);
                }
                else
                {
                    transactionAggregate.OnFailed(message.TransactionBlock, message.TransactionError);
                }

                await _transactionRepository.UpdateAsync(transactionAggregate);
            }
            
            await _operationMonitorTaskRepository.CompleteAsync(message.CompletionToken);
        }
    }
}
