﻿using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Extensions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services
{
    [UsedImplicitly]
    public class AddressValidationService : IAddressValidationService
    {
        private readonly IBlockchainService _blockchainService;


        public AddressValidationService(
            IBlockchainService blockchainService)
        {
            _blockchainService = blockchainService;
        }


        public async Task<bool> ValidateAsync(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException("Should not be null or empty.", nameof(address));
            }
            
            return await AddressValidator.ValidateAsync(address)
                && await _blockchainService.IsWalletAsync(address);
        }
    }
}
