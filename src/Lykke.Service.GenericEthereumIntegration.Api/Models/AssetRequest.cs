using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.Api.Models
{
    [UsedImplicitly]
    public class AssetRequest
    {
        [FromRoute, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string AssetId { get; set; }
    }
}
