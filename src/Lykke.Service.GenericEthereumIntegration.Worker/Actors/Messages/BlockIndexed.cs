using System.ComponentModel;
using System.Numerics;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class BlockIndexed
    {
        public BlockIndexed(BigInteger blockNumber)
        {
            BlockNumber = blockNumber;
        }

        public BigInteger BlockNumber { get; }
    }
}
