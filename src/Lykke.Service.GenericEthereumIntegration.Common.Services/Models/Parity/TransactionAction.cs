using System.Runtime.Serialization;

namespace Lykke.Service.GenericEthereumIntegration.Common.Services.Models.Parity
{
    public class TransactionAction
    {
        [DataMember(Name = "callType")]
        public string CallType { get; set; }

        [DataMember(Name = "from")]
        public string From { get; set; }
        
        [DataMember(Name = "to")]
        public string To { get; set; }
        
        [DataMember(Name = "gas")]
        public string Gas { get; set; }
        
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
