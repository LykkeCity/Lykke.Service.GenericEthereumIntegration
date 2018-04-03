using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.Api.Controllers
{
    [PublicAPI, Route("/api/transactions/history")]
    public class TransactionsHistoryController : ControllerBase
    {
        private readonly IObservableAddressService _observableAddressService;
        private readonly IHistoricalTransactionService _historicalTransactionService;


        public TransactionsHistoryController(
            IObservableAddressService observableAddressService,
            IHistoricalTransactionService historicalTransactionService)
        {
            _observableAddressService = observableAddressService;
            _historicalTransactionService = historicalTransactionService;
        }


        [HttpPost("to/{address}")]
        public async Task<IActionResult> AddAddressToIncomingObservationList(string address)
        {
            await _observableAddressService.AddToIncomingObservationListAsync(address);

            return Ok();
        }

        [HttpPost("from/{address}")]
        public async Task<IActionResult> AddAddressToOutgoingObservationList(string address)
        {
            await _observableAddressService.AddToOutgoingObservationListAsync(address);

            return Ok();
        }

        [HttpDelete("to/{address}")]
        public async Task<IActionResult> DeleteAddressFromIncomingObservationList(string address)
        {
            await _observableAddressService.DeleteFromIncomingObservationListAsync(address);

            return Ok();
        }

        [HttpDelete("from/{address}")]
        public async Task<IActionResult> DeleteAddressFromOutgoingObservationList(string address)
        {
            await _observableAddressService.DeleteFromOutgoingObservationListAsync(address);

            return Ok();
        }

        [HttpGet("to/{address}")]
        public async Task<IActionResult> GetIncomingHistory(string address, [FromQuery] int take, [FromQuery] string afterHash = "")
        {
            (var transactions, var assetId) = await _historicalTransactionService.GetIncomingHistory(address, take, afterHash);
            
            return Ok(transactions.Select(x => new HistoricalTransactionContract
            {
                Amount = x.TransactionAmount.ToString(),
                AssetId = assetId,
                FromAddress = x.FromAddress,
                Hash = x.TransactionHash,
                Timestamp = x.TransactionTimestamp,
                ToAddress = x.ToAddress
            }));
        }

        [HttpGet("from/{address}")]
        public async Task<IActionResult> GetOutgoingHistory(string address, [FromQuery] int take, [FromQuery] string afterHash = "")
        {
            (var transactions, var assetId) = await _historicalTransactionService.GetOutgoingHistory(address, take, afterHash);

            return Ok(transactions.Select(x => new HistoricalTransactionContract
            {
                Amount = x.TransactionAmount.ToString(),
                AssetId = assetId,
                FromAddress = x.FromAddress,
                Hash = x.TransactionHash,
                Timestamp = x.TransactionTimestamp,
                ToAddress = x.ToAddress
            }));
        }
    }
}
