using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;
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
        private readonly IChaosKitty _chaosKitty;
        private readonly BigInteger _gasAmount;
        private readonly IGasPriceOracleService _gasPriceOracleService;
        private readonly ITransactionRepository _transactionRepository;
        

        public TransactionService(
            IBlockchainService blockchainService,
            IGasPriceOracleService gasPriceOracleService,
            GenericEthereumIntegrationApiSettings settings,
            ITransactionRepository transactionRepository,
            IChaosKitty chaosKitty)
        {
            _blockchainService = blockchainService;
            _chaosKitty = chaosKitty;
            _gasAmount = BigInteger.Parse(settings.GasAmount);
            _gasPriceOracleService = gasPriceOracleService;
            _transactionRepository = transactionRepository;
        }


        public async Task<string> BroadcastTransactionAsync(Guid operationId, string signedTxData)
        {
            var operationTransactions = (await _transactionRepository.GetAllForOperationAsync(operationId)).ToList();

            if (operationTransactions.Any(x => x.SignedTxData == signedTxData && x.State != TransactionState.InProgress))
            {
                throw new ConflictException
                (
                    $"Specified transaction [{signedTxData}] for specified operation [{operationId}] has already been broadcasted."
                );
            }

            var txData = _blockchainService.UnsignTransaction(signedTxData);
            var builtTransaction = operationTransactions.FirstOrDefault(x => x.TxData == txData && x.State == TransactionState.Built);

            if (builtTransaction == null)
            {
                throw new BadRequestException
                (
                    $"Specified transaction [{txData}] for specified operation [{operationId}] has not been found."
                );
            }

            var txSigner = _blockchainService.GetTransactionSigner(signedTxData);

            if (!builtTransaction.FromAddress.Equals(txSigner, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new BadRequestException
                (
                    $"Specified transaction [{txData}] for specified operation [{operationId}] has been signed for incorrect address [expected: {builtTransaction.FromAddress}, actual: {txSigner}]."
                );
            }

            var txHash = await SendRawTransactionOrGetTxHashAsync(signedTxData);

            _chaosKitty.Meow(txHash);

            await WaitUntilTransactionIsInPoolAsync(txHash);

            builtTransaction.OnBroadcasted(signedTxData, txHash);
            
            return txHash;
        }

        public async Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress)
        {
            #region Validation

            if (amount <= 0)
            {
                throw new ArgumentException("Amount should be greater then zero.", nameof(amount));
            }
            
            if (!await AddressValidator.ValidateAsync(fromAddress))
            {
                throw new ArgumentException("Address is invalid.", nameof(fromAddress));
            }

            if (!await AddressValidator.ValidateAsync(toAddress))
            {
                throw new ArgumentException("Address is invalid.", nameof(toAddress));
            }

            #endregion

            BigInteger fee;
            BigInteger gasPrice;

            (amount, fee, gasPrice) = await CalculateTransactionParamsAsync(amount, includeFee, toAddress);

            var operationTransactions = (await _transactionRepository.GetAllForOperationAsync(operationId));
            var initialTransaction = operationTransactions.OrderBy(x => x.BuiltOn).FirstOrDefault();

            if (initialTransaction != null)
            {
                return initialTransaction.TxData;
            }

            var nonce = await _blockchainService.GetNextNonceAsync(fromAddress);

            var txData = _blockchainService.BuildTransaction
            (
                to: toAddress,
                amount: amount,
                nonce: nonce,
                gasPrice: gasPrice,
                gasAmount: _gasAmount
            );

            var operationTransaction = TransactionAggregate.Build
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

            await _transactionRepository.AddAsync(operationTransaction);
            
            _chaosKitty.Meow(operationId);

            return txData;
        }

        public async Task DeleteTransactionStateAsync(Guid operationId)
        {
            if (await _transactionRepository.DeleteIfExistsAsync(operationId))
            {
                return;
            }

            throw new NotFoundException($"No transactions for specified operation [{operationId}] has been found.");
        }

        public async Task<TransactionAggregate> GetTransactionAsync(Guid operationId)
        {
            var transactions = (await _transactionRepository.GetAllForOperationAsync(operationId))
                .ToList();
            
            var completedTransaction = transactions
                .SingleOrDefault(x => x.State == TransactionState.Completed || x.State == TransactionState.Failed);

            if (completedTransaction != null)
            {
                return completedTransaction;
            }
            else
            {
                var latestInProgressTransaction = transactions
                    .Where(x => x.State == TransactionState.InProgress)
                    .OrderByDescending(x => x.BroadcastedOn)
                    .FirstOrDefault();

                if (latestInProgressTransaction != null)
                {
                    return latestInProgressTransaction;
                }
            }

            throw new NotFoundException($"No transactions for specified operation [{operationId}] has been found.");
        }

        [Pure]
        internal static BigInteger CalculateFeeWithFeeFactor(BigInteger gasPrice, BigInteger gasAmount, decimal feeFactor)
        {
            if (gasPrice <= 0)
            {
                throw new ArgumentException("Gas price should be greater then zero.", nameof(gasPrice));
            }

            if (feeFactor <= 1)
            {
                throw new ArgumentException("Fee factor should be greater then one.", nameof(feeFactor));
            }

            var feeFactorBits = decimal.GetBits(feeFactor);
            var feeFactorMultiplier = new BigInteger(new decimal(feeFactorBits[0], feeFactorBits[1], feeFactorBits[2], false, 0));
            var decimalPlacesNumber = (int)BitConverter.GetBytes(feeFactorBits[3])[2];
            var feeFactorDivider = new BigInteger(Math.Pow(10, decimalPlacesNumber));
            var newGasPrice = gasPrice * feeFactorMultiplier / feeFactorDivider;
            
            if (newGasPrice > gasPrice)
            {
                return newGasPrice * gasAmount;
            }

            return (gasPrice + 1) * gasAmount;
        }

        public (BigInteger Amount, BigInteger Fee, BigInteger GasPrice) CalculateTransactionParams(TransactionAggregate transaction, decimal feeFactor)
        {
            var amount = transaction.Amount;
            var fee = transaction.Fee;
            var gasPrice = transaction.GasPrice;
            var includeFee = transaction.IncludeFee;


            if (includeFee)
            {
                amount += fee;
            }

            fee = CalculateFeeWithFeeFactor(gasPrice, _gasAmount, feeFactor);
            
            if (includeFee)
            {
                amount -= fee;
            }

            return (amount, fee, gasPrice);
        }

        public async Task<(BigInteger Amount, BigInteger Fee, BigInteger GasPrice)> CalculateTransactionParamsAsync(BigInteger amount, bool includeFee, string toAddress)
        {
            #region Validation

            if (amount <= 0)
            {
                throw new ArgumentException("Amount should be greater then zero.", nameof(amount));
            }

            if (!await AddressValidator.ValidateAsync(toAddress))
            {
                throw new ArgumentException("Address is invalid.", nameof(toAddress));
            }

            #endregion

            var gasPrice = await _gasPriceOracleService.CalculateGasPriceAsync(toAddress, amount);
            var fee = gasPrice * _gasAmount;

            if (includeFee)
            {
                amount -= fee;
            }

            #region Result validation

            if (amount <= 0)
            {
                throw new BadRequestException
                (
                    $"Amount [{amount}] is too small."
                );
            }

            #endregion

            return (amount, fee, gasPrice);
        }

        public async Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId)
        {
            #region Validation

            if (feeFactor <= 1m)
            {
                throw new BadRequestException("Fee factor should be greater than one.");
            }

            #endregion

            var operationTransactions = (await _transactionRepository.GetAllForOperationAsync(operationId)).ToList();
            var initialTransaction = operationTransactions.OrderBy(x => x.BuiltOn).FirstOrDefault();

            if (initialTransaction == null)
            {
                throw new NotFoundException($"Initial transaction for specified operation [{operationId}] has not been not found.");
            }

            (var amount, var fee, var gasPrice) = CalculateTransactionParams(initialTransaction, feeFactor);

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
                var operationTransaction = TransactionAggregate.Build
                (
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

                await _transactionRepository.AddAsync(operationTransaction);
            }

            _chaosKitty.Meow(operationId);

            return txData;
        }

        /// <summary>
        ///     Sends raw transaction, or, if it has already been sent, returns it's txHash
        /// </summary>
        /// <param name="signedTxData">
        ///     Signed transaction data.
        /// </param>
        /// <returns>
        ///     Transaction hash.
        /// </returns>
        public async Task<string> SendRawTransactionOrGetTxHashAsync(string signedTxData)
        {
            var txHash = _blockchainService.GetTransactionHash(signedTxData);
            var receipt = await _blockchainService.GetTransactionReceiptAsync(txHash);

            if (receipt == null)
            {
                await _blockchainService.SendRawTransactionAsync(signedTxData);
            }

            return txHash;
        }

        private async Task WaitUntilTransactionIsInPoolAsync(string txHash)
        {
            var retryCount = 0;

            do
            {

                try
                {
                    var isBroadcasted = await _blockchainService.CheckIfBroadcastedAsync(txHash);

                    if (isBroadcasted)
                    {
                        return;
                    }
                    else
                    {
                        if (await _blockchainService.GetTransactionReceiptAsync(txHash) != null)
                        {
                            return;
                        }

                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

            } while (retryCount++ < 5);

            throw new UnsupportedEdgeCaseException("Transaction not appeared in memory pool in the specified period of time.");
        }
    }
}
