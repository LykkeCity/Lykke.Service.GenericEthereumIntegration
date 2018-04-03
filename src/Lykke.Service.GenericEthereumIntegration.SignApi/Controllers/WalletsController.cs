using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.SignApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Controllers
{
    [PublicAPI, Route("api/wallets")]
    public class WalletsController : Controller
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

            return Ok(new CreateWalletResponse
            {
                PrivateKey = wallet.PrivateKey,
                PublicAddress = wallet.PublicAddress
            });
        }
    }
}
