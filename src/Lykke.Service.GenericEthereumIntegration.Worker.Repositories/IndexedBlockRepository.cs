using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.DTOs;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Repositories.Entities;


namespace Lykke.Service.GenericEthereumIntegration.Worker.Repositories
{
    [UsedImplicitly]
    public class IndexedBlockRepository : IIndexedBlockRepository
    {
        private readonly INoSQLTableStorage<IndexedBlockEntity> _table;


        internal IndexedBlockRepository(
            INoSQLTableStorage<IndexedBlockEntity> table)
        {
            _table = table;
        }


        private static string GetPartitionKey(BigInteger blockNumber)
        {
            return $"{blockNumber}".CalculateHexHash32(3);
        }

        private static string GetRowKey(BigInteger blockNumber)
        {
            return $"{blockNumber}";
        }


        public async Task<bool> DeleteIfExistsAsync(BigInteger blockNumber)
        {
            return await _table.DeleteIfExistAsync
            (
                partitionKey: GetPartitionKey(blockNumber),
                rowKey: GetRowKey(blockNumber)
            );
        }

        public async Task<IEnumerable<IndexedBlockDto>> GetAsync(IEnumerable<BigInteger> blockNumbers)
        {
            var keys = blockNumbers.Select(x => new Tuple<string, string>
            (
                GetPartitionKey(x),
                GetRowKey(x)
            ));

            return (await _table.GetDataAsync(keys))
                .Select(ConvertEntityToDto);
        }

        public async Task<IndexedBlockDto> TryGetAsync(BigInteger blockNumber)
        {
            var entity = await _table.GetDataAsync
            (
                partition: GetPartitionKey(blockNumber),
                row: GetRowKey(blockNumber)
            );

            return ConvertEntityToDto(entity);
        }

        private static IndexedBlockDto ConvertEntityToDto(IndexedBlockEntity entity)
        {
            return new IndexedBlockDto
            {
                BlockHash = entity.BlockHash,
                BlockNumber = entity.BlockNumber,
                ParentHash = entity.ParentHash
            };
        }
    }
}
