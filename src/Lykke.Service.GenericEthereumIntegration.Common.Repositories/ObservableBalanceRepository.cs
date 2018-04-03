using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Repositories.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.GenericEthereumIntegration.Common.Repositories
{
    public class ObservableBalanceRepository : IObservableBalanceRepository
    {
        private readonly INoSQLTableStorage<ObservableBalanceEntity> _table;


        internal ObservableBalanceRepository(
            INoSQLTableStorage<ObservableBalanceEntity> table)
        {
            _table = table;
        }


        private static string GetPartitionKey(string address)
        {
            return address.CalculateHexHash32(3);
        }

        private static string GetRowKey(string address)
        {
            return address;
        }


        public async Task<bool> DeleteIfExistsAsync(string address)
        {
            return await _table.DeleteIfExistAsync
            (
                GetPartitionKey(address),
                GetRowKey(address)
            );
        }

        public async Task<bool> ExistsAsync(string address)
        {
            var entity = await _table.GetDataAsync(GetPartitionKey(address), GetRowKey(address));

            return entity != null;
        }

        public async Task<(IEnumerable<ObservableBalanceDto> Balances, string ContinuationToken)> GetAllAsync(int take, string continuationToken)
        {
            IEnumerable<ObservableBalanceEntity> entities;

            (entities, continuationToken) = await _table.GetDataWithContinuationTokenAsync(take, continuationToken);

            var balances = entities.Select(x => new ObservableBalanceDto
            {
                Amount = x.Amount,
                BlockNumber = x.BlockNumber
            });

            return (balances, continuationToken);
        }

        public async Task<(IEnumerable<ObservableBalanceDto> Balances, string ContinuationToken)> GetAllWithNonZeroAmountAsync(int take, string continuationToken)
        {
            var filterCondition = TableQuery.GenerateFilterCondition("Amount", QueryComparisons.NotEqual, "0");
            var query = new TableQuery<ObservableBalanceEntity>().Where(filterCondition);

            IEnumerable<ObservableBalanceEntity> entities;

            (entities, continuationToken) = await _table.GetDataWithContinuationTokenAsync(query, take, continuationToken);

            var balances = entities.Select(x => new ObservableBalanceDto
            {
                Address = x.Address,
                Amount = x.Amount,
                BlockNumber = x.BlockNumber
            });

            return (balances, continuationToken);
        }

        public async Task<bool> TryAddAsync(string address)
        {
            var entity = new ObservableBalanceEntity
            {
                PartitionKey = GetPartitionKey(address),
                RowKey = GetRowKey(address),

                Address = address,
                Amount = 0,
                BlockNumber = 0
            };

            return await _table.TryInsertAsync(entity);
        }

        public async Task<ObservableBalanceDto> TryGetAsync(string address)
        {
            var entity = await _table.GetDataAsync(GetPartitionKey(address), GetRowKey(address));

            return new ObservableBalanceDto
            {
                Amount = entity.Amount,
                BlockNumber = entity.BlockNumber
            };
        }

        public async Task UpdateAmountAsync(string address, BigInteger amount, BigInteger blockNumber)
        {
            ObservableBalanceEntity UpdateAction(ObservableBalanceEntity entity)
            {
                if (blockNumber > entity.BlockNumber)
                {
                    entity.Amount = amount;
                    entity.BlockNumber = blockNumber;
                }
                
                return entity;
            }

            await _table.MergeAsync
            (
                GetPartitionKey(address),
                GetRowKey(address),
                UpdateAction
            );
        }
    }
}
