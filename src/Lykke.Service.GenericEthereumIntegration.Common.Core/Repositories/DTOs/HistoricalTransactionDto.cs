using System;
using System.Numerics;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs
{
    public class HistoricalTransactionDto
    {
        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public BigInteger TransactionAmount { get; set; }

        public BigInteger TransactionBlock { get; set; }

        public bool TransactionFailed { get; set; }

        public string TransactionHash { get; set; }

        public BigInteger TransactionIndex { get; set; }

        public DateTime TransactionTimestamp { get; set; }
    }
}
