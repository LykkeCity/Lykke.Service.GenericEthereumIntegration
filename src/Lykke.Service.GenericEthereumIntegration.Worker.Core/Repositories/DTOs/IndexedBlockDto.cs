using System.Numerics;


namespace Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.DTOs
{
    public class IndexedBlockDto
    {
        public string BlockHash { get; set; }

        public BigInteger BlockNumber { get; set; }

        public string ParentHash { get; set; }
    }
}
