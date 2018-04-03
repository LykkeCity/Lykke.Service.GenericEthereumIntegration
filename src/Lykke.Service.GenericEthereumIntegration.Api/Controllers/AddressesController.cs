﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract.Addresses;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.Api.Controllers
{
    [PublicAPI, Route("/api/addresses")]
    public class AddressesController : ControllerBase
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
    }
}
