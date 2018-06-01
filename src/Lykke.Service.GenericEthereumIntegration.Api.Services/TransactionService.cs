using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;


namespace Lykke.Service.GenericEthereumIntegration.Api.Services
{
    [UsedImplicitly]
    public class TransactionService : ITransactionService
    {
        private readonly IBlockchainService _blockchainService;
        private readonly ICalculateTransactionParamsStrategy _calculateTransactionParamsStrategy;
        private readonly IChaosKitty _chaosKitty;
        private readonly BigInteger _gasAmount;
        private readonly IRegisterTransactionStrategy _registerTransactionStrategy;
        private readonly ISendRawTransactionOrGetTxHashStrategy _sendRawTransactionOrGetTxHashStrategy;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWaitUntilTransactionIsInPoolStrategy _waitUntilTransactionIsInPoolStrategy;
        

        internal TransactionService(
            [NotNull] IBlockchainService blockchainService,
            [NotNull] ICalculateTransactionParamsStrategy calculateTransactionParamsStrategy,
            [NotNull] IRegisterTransactionStrategy registerTransactionStrategy,
            [NotNull] ISendRawTransactionOrGetTxHashStrategy sendRawTransactionOrGetTxHashStrategy,
            [NotNull] ApiSettings settings,
            [NotNull] ITransactionRepository transactionRepository,
            [NotNull] IWaitUntilTransactionIsInPoolStrategy waitUntilTransactionIsInPoolStrategy,
            [NotNull] IChaosKitty chaosKitty)
        {
            _blockchainService = blockchainService;
            _calculateTransactionParamsStrategy = calculateTransactionParamsStrategy;
            _chaosKitty = chaosKitty;
            _gasAmount = BigInteger.Parse(settings.GasAmount);
            _registerTransactionStrategy = registerTransactionStrategy;
            _sendRawTransactionOrGetTxHashStrategy = sendRawTransactionOrGetTxHashStrategy;
            _transactionRepository = transactionRepository;
            _waitUntilTransactionIsInPoolStrategy = waitUntilTransactionIsInPoolStrategy;
        }

        public async Task<string> BroadcastTransactionAsync(Guid operationId, string signedTxData)
        {
            #region Validation
            
            if (signedTxData.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(signedTxData));
            }

            if (signedTxData.IsNotHexString())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidHexString, nameof(signedTxData));
            }
            
            #endregion
            
            var operationTransactions = (await _transactionRepository.GetAllForOperationAsync(operationId)).ToList();

            if (operationTransactions.Any(x => x.SignedTxData == signedTxData))
            {
                throw new ConflictException
                (
                    $"Specified transaction [{signedTxData}] for specified operation [{operationId}] has already been broadcasted."
                );
            }

            var txData = await _blockchainService.UnsignTransactionAsync(signedTxData);
            var builtTransaction = operationTransactions.FirstOrDefault(x => x.TxData == txData && x.State == TransactionState.Built);

            if (builtTransaction == null)
            {
                throw new BadRequestException
                (
                    $"Specified transaction [{txData}] for specified operation [{operationId}] has not been found."
                );
            }

            var txSigner = _blockchainService.GetTransactionSigner(signedTxData);

            if (builtTransaction.FromAddress != txSigner)
            {
                throw new BadRequestException
                (
                    $"Specified transaction [{txData}] for specified operation [{operationId}] has been signed for incorrect address [expected: {builtTransaction.FromAddress}, actual: {txSigner}]."
                );
            }

            var signedTxHash = await _sendRawTransactionOrGetTxHashStrategy.ExecuteAsync(signedTxData);

            _chaosKitty.Meow(signedTxHash);

            await _waitUntilTransactionIsInPoolStrategy.ExecuteAsync(signedTxHash);
            
            builtTransaction.OnBroadcasted(signedTxData, signedTxHash);

            await _transactionRepository.UpdateAsync(builtTransaction);
            
            return signedTxHash;
        }

        public async Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress)
        {
            #region Validation

            if (amount <= 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanZero, nameof(amount));
            }
            
            if (fromAddress.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(fromAddress));
            }

            if (toAddress.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(toAddress));
            }

            if (fromAddress == toAddress)
            {
                throw new ArgumentException($"{nameof(toAddress)} should differ from {nameof(fromAddress)}.", nameof(toAddress));
            }
            
            if (!await AddressChecksum.ValidateAsync(fromAddress))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(fromAddress));
            }

            if (!await AddressChecksum.ValidateAsync(toAddress))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(toAddress));
            }

            #endregion

            var operationTransactions = (await _transactionRepository.GetAllForOperationAsync(operationId));
            var initialTransaction = operationTransactions.OrderBy(x => x.BuiltOn).FirstOrDefault();

            if (initialTransaction != null)
            {
                return initialTransaction.TxData;
            }

            BigInteger fee;
            BigInteger gasPrice;

            (amount, fee, gasPrice) = await _calculateTransactionParamsStrategy.ExecuteAsync(amount, includeFee, toAddress);
            
            var nonce = await _blockchainService.GetNextNonceAsync(fromAddress);

            var txData = _blockchainService.BuildTransaction
            (
                to: toAddress,
                amount: amount,
                nonce: nonce,
                gasPrice: gasPrice,
                gasAmount: _gasAmount
            );

            await _registerTransactionStrategy.ExecuteAsync
            (
                amount: amount,
                fee: fee,
                fromAddress: fromAddress,
                gasPrice: gasPrice,
                includeFee: includeFee,
                nonce: nonce,
                operationId: operationId,
                toAddress: toAddress,
                txData: txData
            );
            
            _chaosKitty.Meow(operationId);

            return txData;
        }

        public async Task DeleteTransactionAsync(Guid operationId)
        {
            if (await _transactionRepository.DeleteIfExistsAsync(operationId))
            {
                return;
            }

            throw new NotFoundException($"No transactions for specified operation [{operationId}] has been found.");
        }

        public async Task<ITransactionAggregate> GetTransactionAsync(Guid operationId)
        {
            var transactions = (await _transactionRepository.GetAllForOperationAsync(operationId))
                .ToList();

            var completedTransactions = transactions
                .Where(x => x.State == TransactionState.Completed || x.State == TransactionState.Failed)
                .ToList();

            if (completedTransactions.Count > 1)
            {
                throw new UnsupportedEdgeCaseException($"Operation [{operationId}] contains more than one completed transaction.");
            }
            
            var completedTransaction = completedTransactions
                .SingleOrDefault();
                
            if (completedTransaction != null)
            {
                return completedTransaction;
            }
            
            var latestInProgressTransaction = transactions
                .Where(x => x.State == TransactionState.InProgress)
                .OrderByDescending(x => x.BroadcastedOn)
                .FirstOrDefault();

            if (latestInProgressTransaction != null)
            {
                return latestInProgressTransaction;
            }

            
            var earliestBuiltTransaction = transactions
                .Where(x => x.State == TransactionState.Built)
                .OrderBy(x => x.BuiltOn)
                .FirstOrDefault();

            if (earliestBuiltTransaction != null)
            {
                return earliestBuiltTransaction;
            }
            
            throw new NotFoundException($"No transactions for specified operation [{operationId}] has been found.");
        }

        public async Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId)
        {
            #region Validation

            if (feeFactor <= 1m)
            {
                throw new ArgumentException("Should be greater than one.", nameof(feeFactor));
            }

            #endregion
            
            var operationTransactions = (await _transactionRepository.GetAllForOperationAsync(operationId)).ToList();
            var initialTransaction = operationTransactions.OrderBy(x => x.BuiltOn).FirstOrDefault();

            if (initialTransaction == null)
            {
                throw new NotFoundException($"Initial transaction for specified operation [{operationId}] has not been not found.");
            }

            var (amount, fee, gasPrice) = _calculateTransactionParamsStrategy.Execute(initialTransaction, feeFactor);

            var txData = _blockchainService.BuildTransaction
            (
                initialTransaction.ToAddress,
                amount,
                initialTransaction.Nonce,
                gasPrice,
                _gasAmount
            );

            // If same transaction has not been built earlier, persisting it
            if (operationTransactions.All(x => x.TxData != txData))
            {
                await _registerTransactionStrategy.ExecuteAsync(
                    amount: amount,
                    fee: fee,
                    fromAddress: initialTransaction.FromAddress,
                    gasPrice: gasPrice,
                    includeFee: initialTransaction.IncludeFee,
                    nonce: initialTransaction.Nonce,
                    operationId: initialTransaction.OperationId,
                    toAddress: initialTransaction.ToAddress,
                    txData: txData
                );
            }

            _chaosKitty.Meow(operationId);

            return txData;
        }
    }
}
