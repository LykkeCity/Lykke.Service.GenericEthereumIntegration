using System;
using System.ComponentModel;
using System.Numerics;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;


namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class CheckAndUpdateOperation
    {
        public CheckAndUpdateOperation(
            BigInteger transactionBlock,
            string transactionError,
            bool transactionFailed,
            [NotNull] string transactionHash,
            [NotNull] string completionToken)
        {
            #region Validation
            
            if (transactionBlock < 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterOrEqualToZero, nameof(transactionBlock));
            }

            if (transactionFailed && string.IsNullOrEmpty(transactionError))
            {
                throw new ArgumentException("Should not be null or empty for failed transactions.", nameof(transactionError));
            }

            if (transactionHash.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(transactionHash));
            }

            if (transactionHash.IsNotHexString())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidHexString, nameof(transactionHash));
            }
            
            if (completionToken.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(completionToken));
            }
            
            #endregion
            
            CompletionToken = completionToken;
            TransactionBlock = transactionBlock;
            TransactionError = transactionError;
            TransactionFailed = transactionFailed;
            TransactionHash = transactionHash;
        }

        [NotNull]
        public string CompletionToken { get; }

        public BigInteger TransactionBlock { get; }
        
        [CanBeNull]
        public string TransactionError { get; }
        
        public bool TransactionFailed { get; }
        
        [NotNull]
        public string TransactionHash { get; }
    }
}
