using System.Runtime.Serialization;

namespace Lykke.Service.GenericEthereumIntegration.Common.Services.Models.Parity
{
    public class TransactionResult
    {
        [DataMember(Name = "gasUsed")]
        public string GasUsed { get; set; }

        [DataMember(Name = "output")]
        public string Output { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }
    }
}
