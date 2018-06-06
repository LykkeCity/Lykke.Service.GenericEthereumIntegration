using System;
using System.Numerics;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Extensions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Domain
{
    public sealed class TransactionAggregate : ITransactionAggregate
    {
        private TransactionAggregate(
            BigInteger amount,
            DateTime builtOn,
            BigInteger fee,
            [NotNull] string fromAddress,
            BigInteger gasPrice,
            bool includeFee,
            BigInteger nonce,
            Guid operationId,
            TransactionState state,
            [NotNull] string toAddress,
            [NotNull] string txData)
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
            [NotNull] string fromAddress,
            BigInteger gasPrice,
            bool includeFee,
            BigInteger nonce,
            Guid operationId,
            [NotNull] string signedTxData,
            [NotNull] string signedTxHash,
            TransactionState state,
            [NotNull] string toAddress,
            [NotNull] string txData)
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

        [NotNull]
        public string FromAddress { get; }

        public BigInteger GasPrice { get; }

        public bool IncludeFee { get; }

        public BigInteger Nonce { get; }

        public Guid OperationId { get; }

        public string SignedTxData { get; private set; }

        public string SignedTxHash { get; private set; }

        public TransactionState State { get; private set; }

        [NotNull]
        public string ToAddress { get; }

        [NotNull]
        public string TxData { get; }


        public bool IsCompleted
            => State == TransactionState.Completed || State == TransactionState.Failed;
        

        public static TransactionAggregate Build(
            BigInteger amount,
            BigInteger fee,
            [NotNull] string fromAddress,
            BigInteger gasPrice,
            bool includeFee,
            BigInteger nonce,
            Guid operationId,
            [NotNull] string toAddress,
            [NotNull] string txData)
        {
            #region Validation

            if (amount <= 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanZero, nameof(amount));
            }
            
            if (fee <= 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanZero, nameof(fee));
            }

            if (fromAddress.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(fromAddress));
            }
            
            if (!AddressChecksum.Validate(fromAddress))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(fromAddress));
            }
            
            if (gasPrice <= 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanZero, nameof(gasPrice));
            }
            
            if (nonce < 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterOrEqualToZero, nameof(nonce));
            }
            
            if (toAddress.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(toAddress));
            }
            
            if (!AddressChecksum.Validate(toAddress))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(toAddress));
            }
            
            if (txData.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(txData));
            }
            
            if (txData.IsNotHexString())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidHexString, nameof(txData));
            }
            
            #endregion
            
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
            [NotNull] string signedTxData,
            [NotNull] string signedTxHash)
        {
            #region Validation
            
            if (signedTxData.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(signedTxData));
            }
            
            if (signedTxData.IsNotHexString())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidHexString, nameof(signedTxData));
            }
            
            if (signedTxHash.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(signedTxHash));
            }
            
            if (signedTxHash.IsNotHexString())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidHexString, nameof(signedTxHash));
            }
            
            #endregion
            
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
            #region Validation
                
            if (blockNumber < 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterOrEqualToZero, nameof(blockNumber));
            }
            
            #endregion
            
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
            #region Validation
                
            if (blockNumber < 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterOrEqualToZero, nameof(blockNumber));
            }

            if (error.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(error));
            }
            
            #endregion
            
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
            if (State != from)
            {
                throw new InvalidOperationException($"Transaction is not in ${from.ToString()} state.");
            }

            if (!State.IsAllowedToSwitch(to))
            {
                throw new InvalidOperationException($"Transaction state can not be switched from {State.ToString()} to {to.ToString()}");
            }
            
            State = to;
        }
    }
}
