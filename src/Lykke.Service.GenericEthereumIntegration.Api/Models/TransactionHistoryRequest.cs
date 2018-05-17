using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.Api.Models
{
    [UsedImplicitly]
    public class TransactionHistoryRequest
    {
        public TransactionHistoryRequest()
        {
            AfterHash = string.Empty;
        }
        
        [FromRoute, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Address { get; set; }
        
        [FromQuery, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public int Take { get; set; }
        
        [FromQuery, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string AfterHash { get; set; }
    }
}
