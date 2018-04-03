using System.Numerics;
using MessagePack;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.DTOs
{
    [MessagePackObject]
    public class BalanceObserverTaskDto
    {
        [Key(0)]
        public string Address { get; set; }

        [Key(1)]
        public BigInteger BlockNumber { get; set; }
    }
}
