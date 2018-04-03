using System.ComponentModel;
using System.Numerics;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class IndexBlock
    {
        public IndexBlock(BigInteger blockNumber)
        {
            BlockNumber = blockNumber;
        }

        public BigInteger BlockNumber { get; }
    }
}
