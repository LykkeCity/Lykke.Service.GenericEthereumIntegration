using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.Api.Models
{
    public class AddressRequest
    {
        [FromRoute]
        public string Address { get; set; }
    }
}
