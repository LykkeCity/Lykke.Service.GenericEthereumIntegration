using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.DTOs;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;


namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles
{
    public class TransactionIndexerRole : ITransactionIndexerRole
    {
        private readonly IBalanceObserverTaskRepository _balanceObserverTaskRepository;
        private readonly IBlockchainService _blockchainService;
        private readonly IHistoricalTransactionRepository _historicalTransactionRepository;
        private readonly IIndexedBlockRepository _indexedBlockRepository;
        private readonly IOperationMonitorTaskRepository _operationMonitorTaskRepository;


        public TransactionIndexerRole(
            IBalanceObserverTaskRepository balanceObserverTaskRepository,
            IBlockchainService blockchainService,
            IHistoricalTransactionRepository historicalTransactionRepository,
            IIndexedBlockRepository indexedBlockRepository,
            IOperationMonitorTaskRepository operationMonitorTaskRepository)
        {
            _balanceObserverTaskRepository = balanceObserverTaskRepository;
            _blockchainService = blockchainService;
            _historicalTransactionRepository = historicalTransactionRepository;
            _indexedBlockRepository = indexedBlockRepository;
            _operationMonitorTaskRepository = operationMonitorTaskRepository;
        }


        public async Task IndexBlockAsync(IndexBlock message)
        {
            await _indexedBlockRepository.DeleteIfExistsAsync(message.BlockNumber);

            await _historicalTransactionRepository.ClearBlockAsync(message.BlockNumber);

            var transactions = (await _blockchainService.GetTransactionsAsync(message.BlockNumber))
                .ToList();
            
            await Task.WhenAll
            (
                EnqueueBalanceObserverTasksAsync(transactions, message.BlockNumber),
                EnqueueOperationMonitorTasksAsync(transactions, message.BlockNumber),
                InsertHistoricalTransactionsAsync(transactions, message.BlockNumber)
            );
        }

        private async Task EnqueueBalanceObserverTasksAsync(IEnumerable<TransactionDto> transactions, BigInteger blockNumber)
        {
            var affectedAddresses = transactions
                .Where(x => !x.TransactionFailed)
                .SelectMany(x => new[] {x.FromAddress, x.ToAddress});

            foreach (var address in affectedAddresses)
            {
                await _balanceObserverTaskRepository.EnqueueAsync(new BalanceObserverTaskDto
                {
                    Address = address,
                    BlockNumber = blockNumber
                });
            }
        }

        private async Task EnqueueOperationMonitorTasksAsync(IEnumerable<TransactionDto> transactions, BigInteger blockNumber)
        {
            var externalTransactions = transactions
                .Where(x => !x.TransactionIsInternal);

            foreach (var transaction in externalTransactions)
            {
                await _operationMonitorTaskRepository.EnqueueAsync(new OperationMonitorTaskDto
                {
                    TransactionBlock = blockNumber,
                    TransactionError = transaction.TransactionError,
                    TransactionFailed = transaction.TransactionFailed,
                    TransactionHash = transaction.TransactionHash
                });
            }
        }

        private async Task InsertHistoricalTransactionsAsync(IEnumerable<TransactionDto> transactions, BigInteger blockNumber)
        {
            foreach (var transaction in transactions)
            {
                await _historicalTransactionRepository.InsertOrReplaceAsync(new HistoricalTransactionDto
                {
                    FromAddress = transaction.FromAddress,
                    ToAddress = transaction.ToAddress,
                    TransactionAmount = transaction.TransactionAmount,
                    TransactionBlock = blockNumber,
                    TransactionFailed = transaction.TransactionFailed,
                    TransactionHash = transaction.TransactionHash,
                    TransactionIndex = transaction.TransactionIndex,
                    TransactionTimestamp = transaction.TransactionTimestamp
                });
            }
        }
    }
}
