using System.ComponentModel;
using System.Numerics;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class CheckAndUpdateBalance
    {
        public CheckAndUpdateBalance(string address, BigInteger blockNumber, string completionToken)
        {
            Address = address;
            BlockNumber = blockNumber;
            CompletionToken = completionToken;
        }


        public string Address { get; }

        public BigInteger BlockNumber { get; }

        public string CompletionToken { get; }
    }
}
