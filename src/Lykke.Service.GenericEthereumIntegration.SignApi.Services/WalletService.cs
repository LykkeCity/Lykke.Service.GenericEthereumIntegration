﻿using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Services
{
    public class WalletService : IWalletService
    {
        public WalletDto CreateWallet()
        {
            var key = Nethereum.Signer.EthECKey.GenerateKey();

            return new WalletDto()
            {
                PrivateKey = key.GetPrivateKey(),
                PublicAddress = key.GetPublicAddress().ToLower()
            };
        }
    }
}
