using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AzureStorage;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Domain;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;
using MessagePack;


namespace Lykke.Service.GenericEthereumIntegration.Worker.Repositories
{
    [UsedImplicitly]
    public class IndexationStateRepository : IIndexationStateRepository
    {
        private const string Container = "IndexationState";
        private const string Key = ".bin";

        private readonly IBlobStorage _blobStorage;


        internal IndexationStateRepository(
            IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public async Task<IndexationStateAggregate> GetOrCreateAsync()
        {
            if (await _blobStorage.HasBlobAsync(Container, Key))
            {
                using (var stream = await _blobStorage.GetAsync(Container, Key))
                {
                    return new IndexationStateAggregate
                    (
                        ranges: await MessagePackSerializer.DeserializeAsync<IEnumerable<IndexationStateAggregate.Range>>(stream)
                    );
                }
            }

            return new IndexationStateAggregate();
        }

        public async Task UpdateAsync(IndexationStateAggregate aggregate)
        {
            using (var stream = new MemoryStream())
            {
                await MessagePackSerializer.SerializeAsync(stream, aggregate.Ranges);
                await _blobStorage.SaveBlobAsync(Container, Key, stream);
            }
        }
    }
}
