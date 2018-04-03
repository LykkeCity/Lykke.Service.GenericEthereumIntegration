using System.Numerics;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Repositories.Entities;


namespace Lykke.Service.GenericEthereumIntegration.Api.Repositories
{
    public class GasPriceRepository : IGasPriceRepository
    {
        private readonly INoSQLTableStorage<GasPriceEntity> _table;


        internal GasPriceRepository(
            INoSQLTableStorage<GasPriceEntity> table)
        {
            _table = table;
        }


        private static string GetPartitionKey()
        {
            return "GasPrice";
        }

        private static string GetRowKey()
        {
            return "GasPrice";
        }


        public async Task<(BigInteger Min, BigInteger Max)> GetOrAddAsync(BigInteger min, BigInteger max)
        {
            var partitionKey = GetPartitionKey();
            var rowKey = GetRowKey();

            var entity = await _table.GetOrInsertAsync
            (
                partitionKey,
                rowKey,
                () => new GasPriceEntity
                {
                    Max = max,
                    Min = min,

                    PartitionKey = partitionKey,
                    RowKey = rowKey
                }
            );

            return (entity.Min, entity.Max);
        }
    }
}
