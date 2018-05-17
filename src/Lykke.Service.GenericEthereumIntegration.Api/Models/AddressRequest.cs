using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.Api.Models
{
    [UsedImplicitly]
    public class AddressRequest
    {
        [FromRoute, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Address { get; set; }
    }
}
