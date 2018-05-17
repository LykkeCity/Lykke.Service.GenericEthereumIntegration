using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.GenericEthereumIntegration.Common.Validation;
using Lykke.Service.GenericEthereumIntegration.Common.Controllers;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Controllers
{
    [PublicAPI, Route("api/sign")]
    public class SignController : IntegrationControllerBase
    {
        private readonly ISignService _signService;

        public SignController(
            ISignService signService)
        {
            _signService = signService;
        }
        

        [HttpPost, ValidateModel]
        public IActionResult SignAsync([FromBody] SignTransactionRequest signRequest)
        {
            var signedTransactionRaw = _signService.SignTransaction
            (
                signRequest.PrivateKeys[0],
                signRequest.TransactionContext
            );

            return Ok(new SignedTransactionResponse
            {
                SignedTransaction = signedTransactionRaw
            });
        }
    }
}
