using System;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Models;
using Lykke.Service.GenericEthereumIntegration.Common.Controllers;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;
using Lykke.Service.GenericEthereumIntegration.Common.Validation;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.Api.Controllers
{
    [PublicAPI, Route("api/transactions")]
    public class TransactionsController : IntegrationControllerBase
    {
        private readonly ITransactionService _transactionService;


        public TransactionsController(
            ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }


        [HttpPost("broadcast"), ValidateModel]
        public async Task<IActionResult> Broadcast([FromBody] BroadcastTransactionRequest request)
        {
            await _transactionService.BroadcastTransactionAsync
            (
                request.OperationId,
                request.SignedTransaction
            );

            return Ok();
        }

        [HttpPost("single"), ValidateModel]
        public async Task<IActionResult> Build([FromBody] BuildSingleTransactionRequest request)
        {
            var txData = await _transactionService.BuildTransactionAsync
            (
                BigInteger.Parse(request.Amount),
                request.FromAddress.ToLowerInvariant(),
                request.IncludeFee,
                request.OperationId,
                request.ToAddress.ToLowerInvariant()
            );

            return Ok(new BuildTransactionResponse
            {
                TransactionContext = txData
            });
        }

        [HttpPost("many-inputs"), ValidateModel]
        public IActionResult Build([FromBody] BuildTransactionWithManyInputsRequest request)
        {
            return NotImplemented();
        }

        [HttpPost("many-outputs"), ValidateModel]
        public IActionResult Build([FromBody] BuildTransactionWithManyOutputsRequest request)
        {
            return NotImplemented();
        }

        [HttpDelete("broadcast/{operationId:guid}"), ValidateModel]
        public async Task<IActionResult> DeleteState(OperationRequest request)
        {
            await _transactionService.DeleteTransactionStateAsync(request.OperationId);

            return Ok();
        }

        [HttpGet("broadcast/single/{operationId:guid}"), ValidateModel]
        public async Task<IActionResult> GetSingleTransactionState(OperationRequest request)
        {
            var transaction = await _transactionService.GetTransactionAsync(request.OperationId);

            return Ok(new BroadcastedSingleTransactionResponse
            {
                Amount = transaction.Amount.ToString(),
                Block = transaction.BlockNumber.HasValue ? (long)transaction.BlockNumber.Value : 0,
                Error = transaction.Error,
                ErrorCode = BlockchainErrorCode.Unknown,
                Fee = transaction.Fee.ToString(),
                Hash = transaction.SignedTxHash,
                OperationId = transaction.OperationId,
                State = GetState(transaction),
                Timestamp = transaction.CompletedOn ?? transaction.BroadcastedOn ?? transaction.BuiltOn
            });
        }

        [HttpGet("broadcast/many-inputs/{operationId:guid}"), ValidateModel]
        public IActionResult GetManyInputsTransactionState(OperationRequest request)
        {
            return NotImplemented();
        }

        [HttpGet("broadcast/many-outputs/{operationId:guid}"), ValidateModel]
        public IActionResult GetManyOutputsTransactionState(OperationRequest request)
        {
            return NotImplemented();
        }

        [HttpPut, ValidateModel]
        public async Task<IActionResult> Rebuild([FromBody] RebuildTransactionRequest request)
        {
            var txData = await _transactionService.RebuildTransactionAsync
            (
                request.FeeFactor,
                request.OperationId
            );

            return Ok(new RebuildTransactionResponse
            {
                TransactionContext = txData
            });
        }

        private static BroadcastedTransactionState GetState(TransactionAggregate transaction)
        {
            switch (transaction.State)
            {
                case TransactionState.Built:
                    throw new InvalidOperationException
                    (
                        $"Transaction in specified state [{transaction.State.ToString()}] can not be broadcasted transaction."
                    );
                case TransactionState.InProgress:
                    return BroadcastedTransactionState.InProgress;
                case TransactionState.Completed:
                    return BroadcastedTransactionState.Completed;
                case TransactionState.Failed:
                    return BroadcastedTransactionState.Failed;
                default:
                    throw new ArgumentOutOfRangeException
                    (
                        nameof(transaction.State),
                        $"Specified transaction state [{transaction.State.ToString()}] is not supported."
                    );
            }
        }
    }
}
