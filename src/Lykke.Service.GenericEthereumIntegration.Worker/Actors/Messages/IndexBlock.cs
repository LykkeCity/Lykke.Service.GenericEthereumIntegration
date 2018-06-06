using System;
using System.ComponentModel;
using System.Numerics;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class IndexBlock
    {
        public IndexBlock(
            BigInteger blockNumber)
        {
            #region Validation

            if (blockNumber < 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterOrEqualToZero, nameof(blockNumber));
            }

            #endregion
            
            BlockNumber = blockNumber;
        }

        public BigInteger BlockNumber { get; }
    }
}
