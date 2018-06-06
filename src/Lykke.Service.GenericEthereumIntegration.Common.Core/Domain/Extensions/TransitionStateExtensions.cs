using System;
using JetBrains.Annotations;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Extensions
{
    public static class TransitionStateExtensions
    {
        [Pure]
        public static bool IsAllowedToSwitch(this TransactionState from, TransactionState to)
        {
            switch (from)
            {
                case TransactionState.Built:
                    return to == TransactionState.InProgress;
                case TransactionState.InProgress:
                    return to == TransactionState.Completed || to == TransactionState.Failed;
                case TransactionState.Completed:
                case TransactionState.Failed:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(from), from.ToString());
            }
        }
    }
}
