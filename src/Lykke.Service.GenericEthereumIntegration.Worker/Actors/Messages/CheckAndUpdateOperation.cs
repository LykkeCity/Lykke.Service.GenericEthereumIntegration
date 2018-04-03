using System.ComponentModel;
using System.Numerics;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class CheckAndUpdateOperation
    {
        public CheckAndUpdateOperation(BigInteger transactionBlock, string transactionError, bool transactionFailed, string transactionHash, string completionToken)
        {
            CompletionToken = completionToken;
            TransactionBlock = transactionBlock;
            TransactionError = transactionError;
            TransactionFailed = transactionFailed;
            TransactionHash = transactionHash;
        }

        public string CompletionToken { get; }

        public BigInteger TransactionBlock { get; }
        
        public string TransactionError { get; }
        
        public bool TransactionFailed { get; }
        
        public string TransactionHash { get; }
    }
}
