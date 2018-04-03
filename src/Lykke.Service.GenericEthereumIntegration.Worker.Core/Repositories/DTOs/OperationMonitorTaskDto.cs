using System.Numerics;
using MessagePack;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.DTOs
{
    [MessagePackObject]
    public class OperationMonitorTaskDto
    {
        [Key(0)]
        public BigInteger TransactionBlock { get; set; }

        [Key(1)]
        public string TransactionError { get; set; }

        [Key(2)]
        public bool TransactionFailed { get; set; }

        [Key(3)]
        public string TransactionHash { get; set; }
    }
}
