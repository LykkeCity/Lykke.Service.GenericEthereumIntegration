using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Models;
using Lykke.Service.GenericEthereumIntegration.Common.Controllers;
using Lykke.Service.GenericEthereumIntegration.Common.Validation;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.Api.Controllers
{
    [PublicAPI, Route("/api/transactions/history")]
    public class TransactionsHistoryController : IntegrationControllerBase
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


        [HttpPost("to/{address}"), ValidateModel]
        public async Task<IActionResult> AddAddressToIncomingObservationList(AddressRequest request)
        {
            await _observableAddressService.AddToIncomingObservationListAsync(request.Address);

            return Ok();
        }

        [HttpPost("from/{address}"), ValidateModel]
        public async Task<IActionResult> AddAddressToOutgoingObservationList(AddressRequest request)
        {
            await _observableAddressService.AddToOutgoingObservationListAsync(request.Address);

            return Ok();
        }

        [HttpDelete("to/{address}"), ValidateModel]
        public async Task<IActionResult> DeleteAddressFromIncomingObservationList(AddressRequest request)
        {
            await _observableAddressService.DeleteFromIncomingObservationListAsync(request.Address);

            return Ok();
        }

        [HttpDelete("from/{address}"), ValidateModel]
        public async Task<IActionResult> DeleteAddressFromOutgoingObservationList(AddressRequest request)
        {
            await _observableAddressService.DeleteFromOutgoingObservationListAsync(request.Address);

            return Ok();
        }

        [HttpGet("to/{address}"), ValidateModel]
        public async Task<IActionResult> GetIncomingHistory(TransactionHistoryRequest request)
        {
            var (transactions, assetId) = await _historicalTransactionService.GetIncomingHistoryAsync
            (
                request.Address,
                request.Take,
                request.AfterHash
            );
            
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

        [HttpGet("from/{address}"), ValidateModel]
        public async Task<IActionResult> GetOutgoingHistory(TransactionHistoryRequest request)
        {
            var (transactions, assetId) = await _historicalTransactionService.GetOutgoingHistoryAsync
            (
                request.Address,
                request.Take,
                request.AfterHash
            );

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
