using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Filters;
using Lykke.Service.GenericEthereumIntegration.Api.Models;
using Lykke.Service.GenericEthereumIntegration.Api.Validation;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.Api.Controllers
{
    [PublicAPI, Route("api/balances")]
    public class BalancesController : ControllerBase
    {
        private readonly IObservableBalanceService _observableBalanceService;


        public BalancesController(
            IObservableBalanceService observableBalanceService)
        {
            _observableBalanceService = observableBalanceService;
        }

        
        [HttpPost("{address}/observation"), ValidateModel]
        public async Task<IActionResult> AddAddressToObservationList(AddressRequest request)
        {
            await _observableBalanceService.BeginObservationAsync(request.Address);

            return Ok();
        }
        
        [HttpDelete("{address}/observation"), ValidateModel]
        public async Task<IActionResult> DeleteAddressFromObservationList(AddressRequest request)
        {
            await _observableBalanceService.EndObservationAsync(request.Address);

            return Ok();
        }

        [HttpGet, ValidateModel]
        public async Task<IActionResult> GetBalanceList(PaginationRequest request)
        {
            (var balances, var assetId, var continuation) = await _observableBalanceService.GetBalancesAsync(request.Take, request.Continuation);

            return Ok(new PaginationResponse<WalletBalanceContract>
            {
                Continuation = continuation,
                Items = balances.Select(x => new WalletBalanceContract
                {
                    Address = x.Address,
                    AssetId = assetId,
                    Balance = x.Amount.ToString(),
                    Block = (long) x.BlockNumber
                }).ToList()
            });
        }
    }
}
