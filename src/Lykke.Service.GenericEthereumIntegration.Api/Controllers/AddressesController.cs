using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract.Addresses;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Models;
using Lykke.Service.GenericEthereumIntegration.Common.Controllers;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.Api.Controllers
{
    [PublicAPI, Route("/api/addresses")]
    public class AddressesController : IntegrationControllerBase
    {
        private readonly IAddressValidationService _addressValidationService;

        public AddressesController(
            IAddressValidationService addressValidationService)
        {
            _addressValidationService = addressValidationService;
        }


        [HttpGet("{address}/validity")]
        public async Task<IActionResult> GetAddressValidity(string address)
        {
            return Ok(new AddressValidationResponse
            {
                IsValid = await _addressValidationService.ValidateAsync(address)
            });
        }
        
        [HttpGet("{address}/checksum")]
        public async Task<IActionResult> GetAddressChecksum(string address)
        {
            try
            {
                return Ok(new AddressChecksumResponse
                {
                    AddressWithChecksum = await AddressChecksum.EncodeAsync(address)
                });
            }
            catch (ArgumentException e)
            {
                throw new BadRequestException(e.Message);
            }
        }
    }
}
