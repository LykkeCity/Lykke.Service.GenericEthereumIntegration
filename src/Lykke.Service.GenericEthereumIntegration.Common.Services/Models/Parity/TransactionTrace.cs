using System.Runtime.Serialization;

namespace Lykke.Service.GenericEthereumIntegration.Common.Services.Models.Parity
{
    public class TransactionTrace
    {
        [DataMember(Name = "action")]
        public TransactionAction Action { get; set; }

        [DataMember(Name = "blockHash")]
        public string BlockHash { get; set; }

        [DataMember(Name = "blockNumber")]
        public ulong BlockNumber { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }

        [DataMember(Name = "result")]
        public TransactionResult Result { get; set; }

        [DataMember(Name = "subtraces")]
        public int Subtraces { get; set; }

        [DataMember(Name = "traceAddresses")]
        public int[] TraceAddresses { get; set; }

        [DataMember(Name = "transactionHash")]
        public string TransactionHash { get; set; }

        [DataMember(Name = "transactionPosition")]
        public int TransactionPosition { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
