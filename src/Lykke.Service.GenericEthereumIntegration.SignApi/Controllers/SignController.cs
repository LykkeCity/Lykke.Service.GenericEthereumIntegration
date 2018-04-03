using System.Linq;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.SignApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Controllers
{
    [PublicAPI, Route("api/sign")]
    public class SignController : Controller
    {
        private readonly ISignService _signService;

        public SignController(
            ISignService signService)
        {
            _signService = signService;
        }
        

        [HttpPost]
        public IActionResult SignAsync([FromBody] SignTransactionRequest signRequest)
        {
            var signedTransactionRaw = _signService.SignTransaction
            (
                signRequest.PrivateKeys?.FirstOrDefault(),
                signRequest.TransactionContext
            );

            return Ok(new SignTransactionResponse
            {
                SignedTransaction = signedTransactionRaw
            });
        }
    }
}
