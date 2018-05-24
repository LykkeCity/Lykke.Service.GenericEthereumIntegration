using System;
using System.Numerics;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs
{
    public class TransactionDto
    {
        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public BigInteger TransactionAmount { get; set; }
        
        public string TransactionError { get; set; }

        public bool TransactionFailed { get; set; }

        public string TransactionHash { get; set; }

        public BigInteger TransactionIndex { get; set; }

        public bool TransactionIsInternal { get; set; }

        public BigInteger TransactionTimestamp { get; set; }
    }
}
