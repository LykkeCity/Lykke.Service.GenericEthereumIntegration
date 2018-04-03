using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Common;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Repositories.Entities;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Lykke.Service.GenericEthereumIntegration.Common.Repositories
{
    public class HistoricalTransactionRepository : IHistoricalTransactionRepository
    {
        private readonly INoSQLTableStorage<AzureIndex> _blockIndexTable;
        private readonly INoSQLTableStorage<AzureIndex> _fromAddressIndexTable;
        private readonly INoSQLTableStorage<HistoricalTransactionEntity> _table;
        private readonly INoSQLTableStorage<AzureIndex> _toAddressIndexTable;


        internal HistoricalTransactionRepository(
            INoSQLTableStorage<AzureIndex> blockIndexTable,
            INoSQLTableStorage<AzureIndex> fromAddressIndexTable,
            INoSQLTableStorage<HistoricalTransactionEntity> table,
            INoSQLTableStorage<AzureIndex> toAddressIndexTable)
        {
            _blockIndexTable = blockIndexTable;
            _fromAddressIndexTable = fromAddressIndexTable;
            _table = table;
            _toAddressIndexTable = toAddressIndexTable;
        }

        private static string GetPartitionKey(string transactionHash)
        {
            return transactionHash;
        }

        private static string GetRowKey(string fromAddress, string toAddress)
        {
            return $"{fromAddress}-{toAddress}";
        }

        private static string GetBlockIndexPartitionKey(BigInteger blockNumber)
        {
            return $"{blockNumber}";
        }

        private static string GetBlockIndexRowKey(string transactionHash, string fromAddress, string toAddress)
        {
            return $"{transactionHash}-{fromAddress}-{toAddress}";
        }

        private static string GetAddressIndexPartitionKey(string address)
        {
            return address;
        }

        private static string GetAddressIndexRowKey(BigInteger blockNumber, BigInteger transactionIndex)
        {
            return $"{blockNumber:0000000000000000}{transactionIndex:0000000000000000}";
        }



        public async Task InsertOrReplaceAsync(HistoricalTransactionDto transaction)
        {
            // 1. Insert entity

            var entity = new HistoricalTransactionEntity
            {
                FromAddress = transaction.FromAddress,
                ToAddress = transaction.ToAddress,
                TransactionAmount = transaction.TransactionAmount,
                TransactionBlock = transaction.TransactionBlock,
                TransactionFailed = transaction.TransactionFailed,
                TransactionHash = transaction.TransactionHash,
                TransactionIndex = transaction.TransactionIndex,
                TransactionTimestamp = transaction.TransactionTimestamp,

                PartitionKey = GetPartitionKey(transaction.TransactionHash),
                RowKey = GetRowKey(transaction.FromAddress, transaction.ToAddress)
            };

            await _table.InsertOrReplaceAsync(entity);

            // 2. Insert from address index

            var fromAddressIndex = AzureIndex.Create
            (
                GetAddressIndexPartitionKey(transaction.FromAddress),
                GetAddressIndexRowKey(transaction.TransactionBlock, transaction.TransactionIndex),
                entity
            );

            await _toAddressIndexTable.InsertOrReplaceAsync(fromAddressIndex);

            // 3. Insert to address index

            var toAddressIndex = AzureIndex.Create
            (
                GetAddressIndexPartitionKey(transaction.ToAddress),
                GetAddressIndexRowKey(transaction.TransactionBlock, transaction.TransactionIndex),
                entity
            );

            await _toAddressIndexTable.InsertOrReplaceAsync(toAddressIndex);

            // 4. Insert block index

            var blockIndex = AzureIndex.Create
            (
                GetBlockIndexPartitionKey(transaction.TransactionBlock),
                GetBlockIndexRowKey(transaction.TransactionHash, transaction.FromAddress, transaction.ToAddress),
                entity
            );

            await _blockIndexTable.InsertOrReplaceAsync(blockIndex);
        }

        public async Task<IEnumerable<HistoricalTransactionDto>> GetBlockHistoryAsync(BigInteger blockNumber)
        {
            var blockIndices = await _blockIndexTable.GetDataAsync
            (
                GetBlockIndexPartitionKey(blockNumber)
            );

            var blockTransactions = await _table.GetDataAsync
            (
                blockIndices.Select(x => new Tuple<string, string>(x.PrimaryPartitionKey, x.PrimaryRowKey))
            );

            return blockTransactions.Select(ConvertToDto);
        }

        private async Task<IEnumerable<HistoricalTransactionDto>> GetAddressHistory(INoSQLTableStorage<AzureIndex> index, string address, int take, string afterHash)
        {
            string continuationToken = null;

            if (!string.IsNullOrEmpty(afterHash))
            {
                var nextRowKey = (await _table.GetDataAsync(GetPartitionKey(afterHash)))
                    .Where(x => x.FromAddress == address || x.ToAddress == address)
                    .Select(x => GetAddressIndexRowKey(x.TransactionBlock, x.TransactionIndex))
                    .FirstOrDefault();

                if (nextRowKey != null)
                {
                    continuationToken = JsonConvert.SerializeObject(new TableContinuationToken
                    {
                        NextPartitionKey = GetAddressIndexPartitionKey(address),
                        NextRowKey = nextRowKey
                    }).StringToHex();
                }
            }
            
            var addressTransactionKeys = (await index.GetDataWithContinuationTokenAsync(take + 1, continuationToken)).Entities
                .Skip(1)
                .Select(x => new Tuple<string, string>(x.PrimaryPartitionKey, x.PrimaryRowKey));

            return (await _table.GetDataAsync(addressTransactionKeys))
                .Select(ConvertToDto);
        }

        public Task<IEnumerable<HistoricalTransactionDto>> GetIncomingHistory(string address, int take, string afterHash)
        {
            return GetAddressHistory(_toAddressIndexTable, address, take, afterHash);
        }

        public Task<IEnumerable<HistoricalTransactionDto>> GetOutgoingHistory(string address, int take, string afterHash)
        {
            return GetAddressHistory(_fromAddressIndexTable, address, take, afterHash);
        }

        public async Task ClearBlockAsync(BigInteger blockNumber)
        {
            var blockIndices = (await _blockIndexTable.GetDataAsync
            (
                GetBlockIndexPartitionKey(blockNumber)
            )).ToList();

            var blockTransactions = await _table.GetDataAsync
            (
                blockIndices.Select(x => new Tuple<string, string>(x.PrimaryPartitionKey, x.PrimaryRowKey))
            );

            foreach (var blockTransaction in blockTransactions)
            {
                var addressIndexRowKey = GetAddressIndexRowKey(blockNumber, blockTransaction.TransactionIndex);

                await Task.WhenAll
                (
                    _fromAddressIndexTable.DeleteIfExistAsync
                    (
                        GetAddressIndexPartitionKey(blockTransaction.FromAddress),
                        addressIndexRowKey
                    ),
                    _toAddressIndexTable.DeleteIfExistAsync
                    (
                        GetAddressIndexPartitionKey(blockTransaction.ToAddress),
                        addressIndexRowKey
                    )
                );

                await _table.DeleteIfExistAsync(blockTransaction.PartitionKey, blockTransaction.RowKey);
            }

            foreach (var blockIndex in blockIndices)
            {
                await _blockIndexTable.DeleteIfExistAsync(blockIndex.PartitionKey, blockIndex.RowKey);
            }
        }

        private static HistoricalTransactionDto ConvertToDto(HistoricalTransactionEntity entity)
        {
            return new HistoricalTransactionDto
            {
                FromAddress = entity.FromAddress,
                ToAddress = entity.ToAddress,
                TransactionAmount = entity.TransactionAmount,
                TransactionFailed = entity.TransactionFailed,
                TransactionBlock = entity.TransactionBlock,
                TransactionHash = entity.TransactionHash,
                TransactionIndex = entity.TransactionIndex,
                TransactionTimestamp = entity.TransactionTimestamp
            };
        }
    }
}
