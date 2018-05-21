using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces
{
    public interface IBlockchainService
    {
        /// <summary>
        ///     Builds unsigned transaction.
        /// </summary>
        /// <returns>
        ///     RLP-encoded transaction data in hex format.
        /// </returns>
        [Pure, NotNull]
        string BuildTransaction([NotNull] string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice, BigInteger gasAmount);

        /// <summary>
        ///    Checks wether transactions was added in node mempool or not
        /// </summary>
        /// <returns>
        ///    True, if added successfully
        /// </returns>
        Task<bool> CheckIfBroadcastedAsync([NotNull] string transactionHash);

        /// <summary>
        ///     Estimates gas price for simple transfer transaction.
        /// </summary>
        /// <returns>
        ///     A BigInteger instance of the estimated gas price.
        /// </returns>
        Task<BigInteger> EstimateGasPriceAsync([NotNull] string to, BigInteger amount);

        /// <summary>
        ///     Get the balance of a public address at a specified block.
        /// </summary>
        /// <returns>
        ///     A BigInteger instance of the current balance for the given address in wei.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///    Thrown when block with a specified number has not been mined yet.
        /// </exception>
        Task<BigInteger> GetBalanceAsync([NotNull] string address, BigInteger blockNumber);

        /// <summary>
        ///    Get hash of a specified block.
        /// </summary>
        Task<string> GetBlockHashAsync(BigInteger blockNumber);

        /// <summary>
        ///    Get the code at a specific address.
        /// </summary>
        /// <returns>
        ///    The data at given address as a hex string (or 0x for wallets).
        /// </returns>
        Task<string> GetCodeAsync([NotNull] string address);
        
        /// <summary>
        ///     Returns the current block number.
        /// </summary>
        Task<BigInteger> GetLatestBlockNumberAsync();

        /// <summary>
        ///     Get next nonce for spwecified address.
        /// </summary>
        Task<BigInteger> GetNextNonceAsync([NotNull] string address);
        
        /// <summary>
        ///    Get the timestamp of a specified block.
        /// </summary>
        ///
        Task<BigInteger> GetTimestampAsync(BigInteger blockNumber);

        /// <summary>
        ///    Get first error in a transaction.
        /// </summary>
        Task<string> GetTransactionErrorAsync([NotNull] string txHash);

        /// <summary>
        ///    Get the hash of the specified transaction.
        /// </summary>
        string GetTransactionHash([NotNull] string txData);

        /// <summary>
        ///     Get the receipt of a specified transaction.
        /// </summary>
        Task<TransactionReceiptDto> GetTransactionReceiptAsync([NotNull] string txHash);

        /// <summary>
        ///    Get list of transactions in a block.
        /// </summary>
        Task<IEnumerable<TransactionDto>> GetTransactionsAsync(BigInteger blockNumber);

        /// <summary>
        /// Get Signer public address for signed transaction
        /// </summary>
        /// <returns>
        ///    Signer public address
        /// </returns>
        string GetTransactionSigner([NotNull] string signedTxData);

        /// <summary>
        ///     Sends an already signed transaction.
        /// </summary>
        /// <returns>
        ///     Transaction hash as hex string.
        /// </returns>
        Task<string> SendRawTransactionAsync([NotNull] string signedTxData);

        /// <summary>
        ///    Get an unsigned transaction from the signed one.
        /// </summary>
        /// <returns>
        ///    Unsigned transaction data as a hex string.
        /// </returns>
        string UnsignTransaction([NotNull] string signedTxData);
    }
}
