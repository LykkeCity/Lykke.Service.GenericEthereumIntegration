using System;
using System.Numerics;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Interfaces
{
    public interface ITransactionAggregate
    {
        BigInteger Amount { get; }
        
        BigInteger? BlockNumber { get; }
        
        DateTime? BroadcastedOn { get; }
        
        DateTime BuiltOn { get; }
        
        DateTime? CompletedOn { get; }
        
        string Error { get; }
        
        BigInteger Fee { get; }
        
        string FromAddress { get; }
        
        BigInteger GasPrice { get; }
        
        bool IncludeFee { get; }
        
        BigInteger Nonce { get; }
        
        Guid OperationId { get; }
        
        string SignedTxData { get; }
        
        string SignedTxHash { get; }
        
        TransactionState State { get; }
        
        string ToAddress { get; }
        
        string TxData { get; }

        
        bool IsCompleted { get; }
        
        
        void OnBroadcasted(string signedTxData, string signedTxHash);

        void OnCompleted(BigInteger blockNumber);

        void OnFailed(BigInteger blockNumber, string error);
    }
}
