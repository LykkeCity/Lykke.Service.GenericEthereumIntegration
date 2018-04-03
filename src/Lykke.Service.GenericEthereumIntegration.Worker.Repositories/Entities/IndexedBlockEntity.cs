using System.Numerics;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Repositories.Entities
{
    public class IndexedBlockEntity : AzureTableEntity
    {
        public string BlockHash { get; set; }

        public BigInteger BlockNumber { get; set; }

        public string ParentHash { get; set; }
    }
}
