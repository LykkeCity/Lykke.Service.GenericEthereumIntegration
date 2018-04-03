using System.Collections.Generic;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Models
{
    public class SignTransactionRequest
    {
        public IEnumerable<string> PrivateKeys { get; set; }
        
        public string TransactionContext { get; set; }
    }
}
