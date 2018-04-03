using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Repositories.Entities;

namespace Lykke.Service.GenericEthereumIntegration.Common.Repositories
{
    public class ObservableAddressRepository : IObservableAddressRepository
    {
        private readonly INoSQLTableStorage<ObservableAddressEntity> _table;


        internal ObservableAddressRepository(
            INoSQLTableStorage<ObservableAddressEntity> table)
        {
            _table = table;
        }


        private static string GetPartitionKey(string address, ObservationType observationType)
        {
            return $"{observationType.ToString()}-{address.CalculateHexHash32(3)}";
        }

        private static string GetRowKey(string address)
        {
            return address;
        }


        public Task<bool> ExistsInIncomingObservationListAsync(string address)
        {
            return ExistsInObservationListAsync(address, ObservationType.Incoming);
        }

        public Task<bool> ExistsInOutgoingObservationListAsync(string address)
        {
            return ExistsInObservationListAsync(address, ObservationType.Outgoing);
        }

        private async Task<bool> ExistsInObservationListAsync(string address, ObservationType observationType)
        {
            var entity = await _table.GetDataAsync
            (
                GetPartitionKey(address, observationType),
                GetRowKey(address)
            );

            return entity != null;
        }

        public Task<bool> TryAddToIncomingObservationListAsync(string address)
        {
            return TryAddToObservationListAsync(address, ObservationType.Incoming);
        }

        public Task<bool> TryAddToOutgoingObservationListAsync(string address)
        {
            return TryAddToObservationListAsync(address, ObservationType.Outgoing);
        }

        private Task<bool> TryAddToObservationListAsync(string address, ObservationType observationType)
        {
            return _table.TryInsertAsync
            (
                new ObservableAddressEntity
                {
                    Address = address,
                    PartitionKey = GetPartitionKey(address, observationType),
                    RowKey = GetRowKey(address)
                }
            );
        }

        public Task<bool> TryDeleteFromIncomingObservationListAsync(string address)
        {
            return TryDeleteFromObservationListAsync(address, ObservationType.Incoming);
        }

        public Task<bool> TryDeleteFromOutgoingObservationListAsync(string address)
        {
            return TryDeleteFromObservationListAsync(address, ObservationType.Outgoing);
        }

        private Task<bool> TryDeleteFromObservationListAsync(string address, ObservationType observationType)
        {
            return _table.DeleteIfExistAsync
            (
                GetPartitionKey(address, observationType),
                GetRowKey(address)
            );
        }


        private enum ObservationType
        {
            Incoming,
            Outgoing
        }
    }
}
