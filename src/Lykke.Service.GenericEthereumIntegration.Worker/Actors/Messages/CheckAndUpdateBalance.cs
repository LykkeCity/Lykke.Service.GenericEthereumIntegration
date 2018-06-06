using System.ComponentModel;
using System.Numerics;
using JetBrains.Annotations;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class CheckAndUpdateBalance
    {
        public CheckAndUpdateBalance(
            [NotNull] string address,
            BigInteger blockNumber,
            [NotNull] string completionToken)
        {
            Address = address;
            BlockNumber = blockNumber;
            CompletionToken = completionToken;
        }


        [NotNull]
        public string Address { get; }

        public BigInteger BlockNumber { get; }

        [NotNull]
        public string CompletionToken { get; }
    }
}
