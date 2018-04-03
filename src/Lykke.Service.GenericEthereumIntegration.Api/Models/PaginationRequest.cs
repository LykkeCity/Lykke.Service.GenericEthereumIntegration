using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.Api.Models
{
    public class PaginationRequest
    {
        public PaginationRequest()
        {
            Continuation = string.Empty;
        }

        [FromQuery, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Continuation { get; set; }

        [FromQuery, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public int Take { get; set; }
    }
}
