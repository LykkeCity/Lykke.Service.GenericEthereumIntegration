using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract.Common;
using Lykke.Service.GenericEthereumIntegration.Common.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.Api.Controllers
{
    [PublicAPI, Route("api/capabilities")]
    public class CapabilitiesController : IntegrationControllerBase
    {
        private static readonly CapabilitiesResponse CapabilitiesResponse;

        static CapabilitiesController()
        {
            CapabilitiesResponse = new CapabilitiesResponse
            {
                AreManyInputsSupported = false,
                AreManyOutputsSupported = false,
                CanReturnExplorerUrl = false,
                IsPublicAddressExtensionRequired = false,
                IsReceiveTransactionRequired = false,
                IsTestingTransfersSupported = false,
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
