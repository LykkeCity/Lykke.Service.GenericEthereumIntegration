using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract.Wallets;
using Lykke.Service.GenericEthereumIntegration.Common.Controllers;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.SignApi.Controllers
{
    [PublicAPI, Route("api/wallets")]
    public class WalletsController : IntegrationControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletsController(
            IWalletService walletService)
        {
            _walletService = walletService;
        }
        

        [HttpPost]
        public IActionResult CreateWallet()
        {
            var wallet = _walletService.CreateWallet();
            
            return Ok(new WalletResponse
            {
                PrivateKey = wallet.PrivateKey,
                PublicAddress = wallet.PublicAddress
            });
        }
    }
}
