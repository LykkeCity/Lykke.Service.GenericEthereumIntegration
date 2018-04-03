using System;
using System.Numerics;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.GenericEthereumIntegration.Common.Repositories.Entities
{
    public class HistoricalTransactionEntity : AzureTableEntity
    {
        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public BigInteger TransactionAmount { get; set; }

        public BigInteger TransactionBlock { get; set; }
        
        public bool TransactionFailed { get; set; }

        public string TransactionHash { get; set; }

        public BigInteger TransactionIndex { set; get; }

        public DateTime TransactionTimestamp { get; set; }
    }
}
