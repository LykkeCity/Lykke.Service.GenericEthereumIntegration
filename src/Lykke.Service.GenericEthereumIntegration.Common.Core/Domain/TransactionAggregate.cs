using System;
using System.Numerics;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Domain
{
    public sealed class TransactionAggregate
    {
        private TransactionAggregate(
            BigInteger amount,
            DateTime builtOn,
            BigInteger fee,
            string fromAddress,
            BigInteger gasPrice,
            bool includeFee,
            BigInteger nonce,
            Guid operationId,
            TransactionState state,
            string toAddress,
            string txData)
        {
            Amount = amount;
            BuiltOn = builtOn;
            Fee = fee;
            FromAddress = fromAddress;
            GasPrice = gasPrice;
            IncludeFee = includeFee;
            Nonce = nonce;
            OperationId = operationId;
            State = state;
            ToAddress = toAddress;
            TxData = txData;
        }

        internal TransactionAggregate(
            BigInteger amount,
            BigInteger? blockNumber,
            DateTime? broadcastedOn,
            DateTime builtOn,
            DateTime? completedOn,
            string error,
            BigInteger fee,
            string fromAddress,
            BigInteger gasPrice,
            bool includeFee,
            BigInteger nonce,
            Guid operationId,
            string signedTxData,
            string signedTxHash,
            TransactionState state,
            string toAddress,
            string txData)
        {
            Amount = amount;
            BlockNumber = blockNumber;
            BroadcastedOn = broadcastedOn;
            BuiltOn = builtOn;
            CompletedOn = completedOn;
            Error = error;
            Fee = fee;
            FromAddress = fromAddress;
            GasPrice = gasPrice;
            IncludeFee = includeFee;
            Nonce = nonce;
            OperationId = operationId;
            SignedTxData = signedTxData;
            SignedTxHash = signedTxHash;
            State = state;
            ToAddress = toAddress;
            TxData = txData;
        }


        public BigInteger Amount { get; }

        public BigInteger? BlockNumber { get; private set; }

        public DateTime? BroadcastedOn { get; private set; }

        public DateTime BuiltOn { get; }

        public DateTime? CompletedOn { get; private set; }

        public string Error { get; private set; }

        public BigInteger Fee { get; }

        public string FromAddress { get; }

        public BigInteger GasPrice { get; }

        public bool IncludeFee { get; }

        public BigInteger Nonce { get; }

        public Guid OperationId { get; }

        public string SignedTxData { get; private set; }

        public string SignedTxHash { get; private set; }

        public TransactionState State { get; private set; }

        public string ToAddress { get; }

        public string TxData { get; }


        public static TransactionAggregate Build(
            BigInteger amount,
            BigInteger fee,
            string fromAddress,
            BigInteger gasPrice,
            bool includeFee,
            BigInteger nonce,
            Guid operationId,
            string toAddress,
            string txData)
        {
            return new TransactionAggregate
            (
                amount: amount,
                builtOn: DateTime.UtcNow,
                fee: fee,
                fromAddress: fromAddress,
                gasPrice: gasPrice,
                includeFee: includeFee,
                nonce: nonce,
                operationId: operationId,
                state: TransactionState.Built,
                toAddress: toAddress,
                txData: txData
            );
        }

        public void OnBroadcasted(
            string signedTxData,
            string signedTxHash)
        {
            SwitchState
            (
                from: TransactionState.Built,
                to: TransactionState.InProgress
            );

            BroadcastedOn = DateTime.UtcNow;
            SignedTxData = signedTxData;
            SignedTxHash = signedTxHash;
        }

        public void OnCompleted(
            BigInteger blockNumber)
        {
            SwitchState
            (
                from: TransactionState.InProgress,
                to: TransactionState.Completed
            );

            BlockNumber = blockNumber;
            CompletedOn = DateTime.UtcNow;
        }

        public void OnFailed(
            BigInteger blockNumber,
            string error)
        {
            SwitchState
            (
                from: TransactionState.InProgress,
                to: TransactionState.Failed
            );

            BlockNumber = blockNumber;
            CompletedOn = DateTime.UtcNow;
            Error = error;
        }

        private void SwitchState(
            TransactionState from,
            TransactionState to)
        {
            if (to > State && from == State)
            {
                State = to;
            }
            else
            {
                throw new InvalidOperationException($"Transaction state can not be switched from {State.ToString()} to {to.ToString()}");
            }
        }
    }
}
