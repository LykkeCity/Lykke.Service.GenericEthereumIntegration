using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.Api.Models
{
    public class AssetRequest
    {
        [FromRoute]
        public string AssetId { get; set; }
    }
}
