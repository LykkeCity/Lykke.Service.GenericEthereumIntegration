using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Common;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Repositories.Entities;


namespace Lykke.Service.GenericEthereumIntegration.Common.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly INoSQLTableStorage<AzureIndex> _signedTxHashIndexTable;
        private readonly INoSQLTableStorage<TransactionEntity> _table;


        internal TransactionRepository(
            INoSQLTableStorage<TransactionEntity> table,
            INoSQLTableStorage<AzureIndex> signedTxHashIndexTable)
        {
            _signedTxHashIndexTable = signedTxHashIndexTable;
            _table = table;
        }


        public async Task AddAsync(ITransactionAggregate aggregate)
        {
            var entity = new TransactionEntity
            {
                Amount = aggregate.Amount,
                BlockNumber = aggregate.BlockNumber,
                BroadcastedOn = aggregate.BroadcastedOn,
                BuiltOn = aggregate.BuiltOn,
                CompletedOn = aggregate.CompletedOn,
                Error = aggregate.Error,
                Fee = aggregate.Fee,
                FromAddress = aggregate.FromAddress,
                GasPrice = aggregate.GasPrice,
                IncludeFee = aggregate.IncludeFee,
                Nonce = aggregate.Nonce,
                OperationId = aggregate.OperationId,
                SignedTxHash = aggregate.SignedTxHash,
                SignedTxData = aggregate.SignedTxData,
                State = aggregate.State,
                ToAddress = aggregate.ToAddress,
                TxData = aggregate.TxData
            };

            await _table.InsertAsync(entity);
        }

        public async Task<bool> DeleteIfExistsAsync(Guid operationId)
        {
            var entities = (await _table.GetDataAsync(GetPartitionKey(operationId))).ToList();

            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    await _table.DeleteIfExistAsync(entity.PartitionKey, entity.RowKey);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        
        public async Task<IEnumerable<ITransactionAggregate>> GetAllForOperationAsync(Guid operationId)
        {
            return (await _table.GetDataAsync(GetPartitionKey(operationId)))
                .Select(ConvertEntityToAggregate);
        }

        public async Task<IEnumerable<ITransactionAggregate>> GetAllInProgressAsync()
        {
            return (await _table.GetDataAsync(x => x.State == TransactionState.InProgress))
                .Select(ConvertEntityToAggregate);
        }

        public async Task<ITransactionAggregate> TryGetAsync(string transactionHash)
        {
            var index = await _signedTxHashIndexTable.GetDataAsync
            (
                partition: GetIndexPartitionKey(transactionHash),
                row: GetIndexRowKey(transactionHash)
            );

            if (index != null)
            {
                var entity = await _table.GetDataAsync
                (
                    partition: index.PrimaryPartitionKey,
                    row: index.PrimaryRowKey
                );

                return ConvertEntityToAggregate(entity);
            }
            else
            {
                return null;
            }
        }

        public async Task UpdateAsync(ITransactionAggregate aggregate)
        {
            TransactionEntity UpdateAction(TransactionEntity entity)
            {
                entity.Amount = aggregate.Amount;
                entity.BlockNumber = aggregate.BlockNumber;
                entity.BroadcastedOn = aggregate.BroadcastedOn;
                entity.BuiltOn = aggregate.BuiltOn;
                entity.CompletedOn = aggregate.CompletedOn;
                entity.Error = aggregate.Error;
                entity.Fee = aggregate.Fee;
                entity.FromAddress = aggregate.FromAddress;
                entity.GasPrice = aggregate.GasPrice;
                entity.IncludeFee = aggregate.IncludeFee;
                entity.Nonce = aggregate.Nonce;
                entity.OperationId = aggregate.OperationId;
                entity.SignedTxHash = aggregate.SignedTxHash;
                entity.SignedTxData = aggregate.SignedTxData;
                entity.State = aggregate.State;
                entity.ToAddress = aggregate.ToAddress;
                entity.TxData = aggregate.TxData;

                return entity;
            }

            var partitionKey = GetPartitionKey(aggregate.OperationId);
            var rowKey = GetRowKey(aggregate.TxData);

            await _table.MergeAsync(partitionKey, rowKey, UpdateAction);

            if (!string.IsNullOrEmpty(aggregate.SignedTxHash))
            {
                var indexPartitionKey = GetIndexPartitionKey(aggregate.SignedTxHash);
                var indexRowKey = GetIndexRowKey(aggregate.SignedTxHash);

                var indexEntity = AzureIndex.Create(indexPartitionKey, indexRowKey, partitionKey, rowKey);

                await _signedTxHashIndexTable.InsertOrReplaceAsync(indexEntity);
            }
        }


        private static string GetPartitionKey(Guid operationId)
        {
            return $"{operationId}";
        }

        private static string GetRowKey(string txData)
        {
            return txData.CalculateHexHash64();
        }

        private static string GetIndexPartitionKey(string signedTxHash)
        {
            return signedTxHash.CalculateHexHash32(3);
        }

        private static string GetIndexRowKey(string signedTxHash)
        {
            return signedTxHash;
        }

        private static TransactionAggregate ConvertEntityToAggregate(TransactionEntity entity)
        {
            return new TransactionAggregate
            (
                amount: entity.Amount,
                blockNumber: entity.BlockNumber,
                broadcastedOn: entity.BroadcastedOn,
                builtOn: entity.BuiltOn,
                completedOn: entity.CompletedOn,
                error: entity.Error,
                fee: entity.Fee,
                fromAddress: entity.FromAddress,
                gasPrice: entity.GasPrice,
                includeFee: entity.IncludeFee,
                nonce: entity.Nonce,
                operationId: entity.OperationId,
                signedTxData: entity.SignedTxData,
                signedTxHash: entity.SignedTxHash,
                state: entity.State,
                toAddress: entity.ToAddress,
                txData: entity.TxData
            );
        }
    }
}
