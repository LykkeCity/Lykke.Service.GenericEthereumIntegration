﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RLP;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json.Linq;

using CommonUtils = Common.Utils;
using Transaction = Nethereum.Signer.Transaction;

namespace Lykke.Service.GenericEthereumIntegration.Common.Services
{
    public abstract class BlockchainServiceBase : IBlockchainService
    {
        private readonly Web3 _web3;


        protected BlockchainServiceBase(
            Web3 web3)
        {
            _web3 = web3;
        }

        private async Task<BigInteger> GetBalanceAsync(string address, BlockParameter blockParameter)
        {
            return (await _web3.Eth.GetBalance.SendRequestAsync(address, blockParameter))
                .Value;
        }

        /// <inheritdoc />
        public string BuildTransaction(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice, BigInteger gasAmount)
        {
            var transaction = new Transaction
            (
                to,
                amount,
                nonce,
                gasPrice,
                gasAmount,
                null
            );

            return transaction.GetRLPEncoded().ToHex();
        }

        /// <inheritdoc />
        public async Task<BigInteger> EstimateGasPriceAsync(string to, BigInteger amount)
        {
            var input = new TransactionInput
            {
                To = to,
                Value = new HexBigInteger(amount)
            };

            return (await _web3.Eth.Transactions.EstimateGas.SendRequestAsync(input))
                .Value;
        }

        /// <inheritdoc />
        public async Task<BigInteger> GetBalanceAsync(string address, BigInteger blockNumber)
        {
            var block = new BlockParameter((ulong)blockNumber);

            return await GetBalanceAsync(address, block);
        }

        public async Task<string> GetBlockHashAsync(BigInteger blockNumber)
        {
            var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(blockNumber));

            return block.BlockHash;
        }

        /// <inheritdoc />
        public async Task<string> GetCodeAsync(string address)
        {
            return await _web3.Eth.GetCode.SendRequestAsync(address);
        }

        /// <inheritdoc />
        public async Task<BigInteger> GetLatestBalanceAsync(string address)
        {
            var block = BlockParameter.CreateLatest();

            return await GetBalanceAsync(address, block);
        }

        /// <inheritdoc />
        public async Task<BigInteger> GetLatestBlockNumberAsync()
        {
            return await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
        }

        /// <inheritdoc />
        public abstract Task<BigInteger> GetNextNonceAsync(string address);


        /// <inheritdoc />
        public async Task<BigInteger> GetPendingBalanceAsync(string address)
        {
            var block = BlockParameter.CreatePending();

            return await GetBalanceAsync(address, block);
        }

        public async Task<BigInteger> GetTimestampAsync(BigInteger blockNumber)
        {
            var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(blockNumber));

            return block.Timestamp.Value;
        }

        /// <inheritdoc />
        public string GetTransactionHash(string txData)
        {
            return (new Transaction(CommonUtils.HexToArray(txData)))
                .RawHash
                .ToHex(true);
        }

        /// <inheritdoc />
        public async Task<TransactionReceiptDto> GetTransactionReceiptAsync(string txHash)
        {
            // _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash) fails with exception
            // using workaround

            var request = _web3.Eth.Transactions.GetTransactionReceipt.BuildRequest(txHash);
            var receipt = await _web3.Client.SendRequestAsync<JObject>(request);

            if (receipt != null)
            {
                return new TransactionReceiptDto
                {
                    BlockHash = receipt["blockHash"].Value<string>(),
                    BlockNumber = new HexBigInteger(receipt["blockNumber"].Value<string>()).Value,
                    ContractAddress = receipt["contractAddress"].Value<string>(),
                    CumulativeGasUsed = new HexBigInteger(receipt["cumulativeGasUsed"].Value<string>()).Value,
                    GasUsed = new HexBigInteger(receipt["gasUsed"].Value<string>()).Value,
                    Status = new HexBigInteger(receipt["status"].Value<string>()).Value,
                    TransactionHash = receipt["transactionHash"].Value<string>(),
                    TransactionIndex = new HexBigInteger(receipt["transactionIndex"].Value<string>()).Value
                };
            }

            return null;
        }

        public abstract Task<IEnumerable<TransactionDto>> GetTransactionsAsync(BigInteger blockNumber);

        public async Task<bool> CheckIfBroadcastedAsync(string transactionHash)
        {
            var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);

            return transaction != null && transaction.BlockNumber.Value == 0;
        }

        /// <inheritdoc />
        public abstract Task<string> GetTransactionErrorAsync(string txHash);

        /// <inheritdoc />
        public async Task<string> SendRawTransactionAsync(string signedTxData)
        {
            return await _web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTxData);
        }

        /// <inheritdoc />
        public string UnsignTransaction(string signedTxData)
        {
            var signedTransaction = new Transaction(CommonUtils.HexToArray(signedTxData));

            if (signedTransaction.Data != null)
            {
                throw new NotSupportedException("Transactions with data are not supported.");
            }

            var to = signedTransaction.ReceiveAddress.ToHex(true);
            var amount = signedTransaction.Value.ToBigIntegerFromRLPDecoded();
            var nonce = signedTransaction.Nonce.ToBigIntegerFromRLPDecoded();
            var gasPrice = signedTransaction.GasPrice.ToBigIntegerFromRLPDecoded();
            var gasLimit = signedTransaction.GasLimit.ToBigIntegerFromRLPDecoded();

            return BuildTransaction(to, amount, nonce, gasPrice, gasLimit);
        }

        public string GetTransactionSigner(string signedTxData)
        {
            var signedTransaction = new Transaction(CommonUtils.HexToArray(signedTxData));
            var signerPublicAddress = signedTransaction.Key?.GetPublicAddress().ToLowerInvariant();

            return signerPublicAddress;
        }
    }
}
