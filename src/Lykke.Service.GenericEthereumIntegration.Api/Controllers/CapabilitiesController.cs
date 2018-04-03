using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract.Common;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.Api.Controllers
{
    [PublicAPI, Route("api/capabilities")]
    public class CapabilitiesController : ControllerBase
    {
        private static readonly CapabilitiesResponse CapabilitiesResponse;

        static CapabilitiesController()
        {
            CapabilitiesResponse = new CapabilitiesResponse
            {
                AreManyInputsSupported = false,
                AreManyOutputsSupported = false,
                IsTransactionsRebuildingSupported = true
            };
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(CapabilitiesResponse);
        }
    }
}
