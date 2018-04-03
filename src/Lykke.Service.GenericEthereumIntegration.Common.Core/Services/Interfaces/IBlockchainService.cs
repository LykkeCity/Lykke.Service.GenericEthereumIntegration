﻿using System.Collections.Generic;
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
        /// <param name="to">
        /// 
        /// </param>
        /// <param name="amount">
        /// 
        /// </param>
        /// <param name="nonce">
        /// 
        /// </param>
        /// <param name="gasPrice">
        /// 
        /// </param>
        /// <param name="gasAmount">
        /// 
        /// </param>
        /// <returns>
        ///     RLP-encoded transaction data in hex format.
        /// </returns>
        string BuildTransaction(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice, BigInteger gasAmount);

        /// <summary>
        ///    Checks wether transactions was added in node mempool or not
        /// </summary>
        /// <param name="transactionHash">
        ///    Transaction hash to check
        /// </param>
        /// <returns>
        ///    True, if added successfully
        /// </returns>
        Task<bool> CheckIfBroadcastedAsync(string transactionHash);

        /// <summary>
        ///     Estimates gas price for simple transfer transaction.
        /// </summary>
        /// <param name="to">
        ///     Destination address.
        /// </param>
        /// <param name="amount">
        ///     Transfer amount.
        /// </param>
        /// <returns>
        ///     A BigInteger instance of the estimated gas price.
        /// </returns>
        Task<BigInteger> EstimateGasPriceAsync(string to, BigInteger amount);

        /// <summary>
        ///     Get the balance of a public address at a specified block.
        /// </summary>
        /// <param name="address">
        ///     The address to get the balance of.
        /// </param>
        /// <param name="blockNumber">
        ///     The number of a block to get the balance at.
        /// </param>
        /// <returns>
        ///     A BigInteger instance of the current balance for the given address in wei.
        /// </returns>
        Task<BigInteger> GetBalanceAsync(string address, BigInteger blockNumber);


        Task<string> GetBlockHashAsync(BigInteger blockNumber);

        /// <summary>
        ///    Get the code at a specific address.
        /// </summary>
        /// <param name="address">
        ///    The address to get the code of.
        /// </param>
        /// <returns>
        ///    The data at given address as a hex string (or 0x for wallets).
        /// </returns>
        Task<string> GetCodeAsync(string address);

        /// <summary>
        ///     Get the balance of a public address at a latest block.
        /// </summary>
        /// <param name="address">
        ///     The address to get the balance of.
        /// </param>
        /// <returns>
        ///     A BigInteger instance of the current balance for the given address in wei.
        /// </returns>
        Task<BigInteger> GetLatestBalanceAsync(string address);

        /// <summary>
        ///     Returns the current block number.
        /// </summary>
        /// <returns>
        ///     The number of the most recent block.
        /// </returns>
        Task<BigInteger> GetLatestBlockNumberAsync();

        /// <summary>
        ///     Get next nonce for spwecified address.
        /// </summary>
        /// <param name="address">
        ///     The address to get next nonce of.
        /// </param>
        /// <returns>
        ///     A BigInteger instance of the next nonce for the given address.
        /// </returns>
        Task<BigInteger> GetNextNonceAsync(string address);

        /// <summary>
        ///     Get the balance of a public address at a pending block.
        /// </summary>
        /// <param name="address">
        ///     The address to get the balance of.
        /// </param>
        /// <returns>
        ///     A BigInteger instance of the current balance for the given address in wei.
        /// </returns>
        Task<BigInteger> GetPendingBalanceAsync(string address);

        /// <summary>
        ///    Get the timestamp of a specified block.
        /// </summary>
        /// <param name="blockNumber">
        ///    The number of a block to get the timestamp of.
        /// </param>
        /// <returns>
        ///    A BigInteger instance of the timestamp for the specified block number.
        /// </returns>
        Task<BigInteger> GetTimestampAsync(BigInteger blockNumber);

        /// <summary>
        ///    Get first error in a transaction.
        /// </summary>
        /// <param name="txHash">
        ///    The hash of the transaction to get error from.
        /// </param>
        /// <returns>
        ///    First error description or null, if no errors occured.
        /// </returns>
        Task<string> GetTransactionErrorAsync(string txHash);

        /// <summary>
        ///    Get the has of the specified transaction
        /// </summary>
        /// <param name="txData">
        ///    Transaction as a hex string.
        /// </param>
        /// <returns>
        ///    Transaction hash.
        /// </returns>
        string GetTransactionHash(string txData);

        /// <summary>
        ///     Get the receipt of a specified transaction.
        /// </summary>
        /// <param name="txHash">
        ///     The transaction hash.
        /// </param>
        /// <returns>
        ///     A transaction receipt object, or null when no receipt was found.
        /// </returns>
        Task<TransactionReceiptDto> GetTransactionReceiptAsync(string txHash);

        Task<IEnumerable<TransactionDto>> GetTransactionsAsync(BigInteger blockNumber);

        /// <summary>
        /// Get Signer public address for signed transaction
        /// </summary>
        /// <param name="signedTxData">
        ///    Signed transaction data
        /// </param>
        /// <returns>
        ///    Signer public address
        /// </returns>
        string GetTransactionSigner(string signedTxData);

        /// <summary>
        ///     Sends an already signed transaction.
        /// </summary>
        /// <param name="signedTxData">
        ///     Signed transaction data in HEX format.
        /// </param>
        /// <returns>
        ///     The 32 Bytes transaction hash as HEX string.
        /// </returns>
        Task<string> SendRawTransactionAsync(string signedTxData);

        /// <summary>
        ///    Get an unsigned transaction from the signed one.
        /// </summary>
        /// <param name="signedTxData">
        ///    Signed transaction data as a hex string.
        /// </param>
        /// <returns>
        ///    Unsigned transaction data as a hex string.
        /// </returns>
        string UnsignTransaction(string signedTxData);
    }
}
