using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;
using MessagePack;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
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
            [NotNull] Web3 web3)
        {
            _web3 = web3;
        }

        
        /// <inheritdoc />
        public string BuildTransaction(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice, BigInteger gasAmount)
        {
            #region Validation
            
            if (to.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(to));
            }

            if (!AddressChecksum.Validate(to))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(to));
            }

            if (amount <= 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanZero, nameof(amount));
            }
            
            if (nonce < 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterOrEqualToZero, nameof(nonce));
            }
            
            if (gasPrice <= 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanZero, nameof(gasPrice));
            }
            
            if (gasAmount <= 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanZero, nameof(gasAmount));
            }
            
            #endregion

            var transaction = new UnsignedTransactionDto
            {
                Amount = amount,
                GasAmount = gasAmount,
                GasPrice = gasPrice,
                Nonce = nonce,
                To = to
            };

            return MessagePackSerializer
                .Serialize(transaction)
                .ToHex(prefix: true);
        }
        
        /// <inheritdoc />
        public async Task<bool> CheckIfBroadcastedAsync(string txHash)
        {
            #region Validation
            
            if (txHash.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(txHash));
            }

            if (txHash.IsNotHexString())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidHexString, nameof(txHash));
            }
            
            #endregion
            
            var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txHash);

            return transaction != null && transaction.BlockNumber.Value == 0;
        }

        /// <inheritdoc />
        public async Task<BigInteger> EstimateGasPriceAsync(string to, BigInteger amount)
        {
            #region Validation
            
            if (to.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(to));
            }
            
            if (!await AddressChecksum.ValidateAsync(to))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(to));
            }
            
            if (amount <= 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanZero, nameof(amount));
            }
            
            #endregion
            
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
            #region Validation
            
            if (address.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(address));
            }

            if (!AddressChecksum.Validate(address))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(address));
            }
            
            if (blockNumber < 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterOrEqualToZero, nameof(blockNumber));
            }
            
            #endregion
            
            try
            {
                var block = new BlockParameter((ulong)blockNumber);

                return (await _web3.Eth.GetBalance.SendRequestAsync(address, block))
                    .Value;
            }
            catch (RpcResponseException e) when (e.RpcError.Code == -32602)
            {
                throw new ArgumentOutOfRangeException(CommonExceptionMessages.BlockNumberIsTooHigh, e);
            }
        }

        /// <inheritdoc />
        public async Task<string> GetBlockHashAsync(BigInteger blockNumber)
        {
            #region Validation
            
            if (blockNumber < 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterOrEqualToZero, nameof(blockNumber));
            }
            
            #endregion
            
            var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(blockNumber));

            return block?.BlockHash
                ?? throw new ArgumentOutOfRangeException(CommonExceptionMessages.BlockNumberIsTooHigh);
        }

        /// <inheritdoc />
        public async Task<string> GetCodeAsync(string address)
        {
            #region Validation
            
            if (address.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(address));
            }
            
            if (!AddressChecksum.Validate(address))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(address));
            }
            
            #endregion
            
            return await _web3.Eth.GetCode.SendRequestAsync(address);
        }
        
        /// <inheritdoc />
        public async Task<BigInteger> GetLatestBlockNumberAsync()
        {
            return await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
        }

        /// <inheritdoc />
        public abstract Task<BigInteger> GetNextNonceAsync(string address);
        
        /// <inheritdoc />
        public async Task<BigInteger> GetTimestampAsync(BigInteger blockNumber)
        {
            #region Validation
            
            if (blockNumber < 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterOrEqualToZero, nameof(blockNumber));
            }
            
            #endregion
            
            var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(blockNumber));

            return block?.Timestamp.Value 
                ?? throw new ArgumentOutOfRangeException(CommonExceptionMessages.BlockNumberIsTooHigh);
        }

        /// <inheritdoc />
        public abstract Task<IEnumerable<string>> TryGetTransactionErrorsAsync(string txHash);
        
        /// <inheritdoc />
        public string GetTransactionHash(string signedTxData)
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

            var txDataBytes = HexToBytesArray(signedTxData);
            
            return (new Transaction(txDataBytes)).RawHash
                .ToHex(true);
        }

        /// <inheritdoc />
        public string GetTransactionSigner(string signedTxData)
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

            var signedTxDataBytes = HexToBytesArray(signedTxData);
            var signedTransaction = new Transaction(signedTxDataBytes);

            var signerPublicAddress = signedTransaction.Key.GetPublicAddress();

            return signerPublicAddress;
        }

        /// <inheritdoc />
        public abstract Task<IEnumerable<TransactionDto>> GetTransactionsAsync(BigInteger blockNumber);

        /// <inheritdoc />
        public async Task<string> SendRawTransactionAsync(string signedTxData)
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
            
            return await _web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTxData);
        }

        /// <inheritdoc />
        public async Task<TransactionReceiptDto> TryGetTransactionReceiptAsync(string txHash)
        {
            #region Validation
            
            if (txHash.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(txHash));
            }
            
            if (txHash.IsNotHexString())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidHexString, nameof(txHash));
            }
            
            #endregion
            
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
        
        /// <inheritdoc />
        public async Task<string> UnsignTransactionAsync(string signedTxData)
        {
            #region Validation
            
            if (string.IsNullOrEmpty(signedTxData))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(signedTxData));
            }
            
            if (signedTxData.IsNotHexString())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidHexString, nameof(signedTxData));
            }
            
            #endregion

            var signedTxDataByte = HexToBytesArray(signedTxData);
            var signedTransaction = new Transaction(signedTxDataByte);

            if (signedTransaction.Data != null)
            {
                throw new NotSupportedException("Transactions with data are not supported.");
            }

            var to = signedTransaction.ReceiveAddress.ToHex(true);
            var amount = signedTransaction.Value.ToBigIntegerFromRLPDecoded();
            var nonce = signedTransaction.Nonce.ToBigIntegerFromRLPDecoded();
            var gasPrice = signedTransaction.GasPrice.ToBigIntegerFromRLPDecoded();
            var gasLimit = signedTransaction.GasLimit.ToBigIntegerFromRLPDecoded();

            to = await AddressChecksum.EncodeAsync(to);
            
            return BuildTransaction(to, amount, nonce, gasPrice, gasLimit);
        }

        
        private static byte[] HexToBytesArray(string hexString)
        {
            if (hexString.StartsWith("0x"))
            {
                hexString = hexString.Remove(0, 2);
            }

            return CommonUtils.HexToArray(hexString);
        }
    }
}
