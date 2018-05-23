﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;
using Lykke.Service.GenericEthereumIntegration.Common.Services.Models.Parity;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.Parity;
using Newtonsoft.Json.Linq;


namespace Lykke.Service.GenericEthereumIntegration.Common.Services
{
    public class BlockchainServiceParity : BlockchainServiceBase
    {
        private readonly Web3Parity _web3Parity;

        public BlockchainServiceParity(
            [NotNull] Web3Parity web3Parity) 
            : base(web3Parity)
        {
            _web3Parity = web3Parity;
        }

        /// <inheritdoc />
        public override async Task<BigInteger> GetNextNonceAsync(string address)
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
            
            var request = new RpcRequest($"{Guid.NewGuid()}", "parity_nextNonce", address);
            var response = await _web3Parity.Client.SendRequestAsync<string>(request);
            var result = new HexBigInteger(response);

            return result.Value;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<TransactionDto>> GetTransactionsAsync(BigInteger blockNumber)
        {
            #region Validation
            
            if (blockNumber < 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterOrEqualToZero, nameof(blockNumber));
            }
            
            #endregion
            
            var blocks = _web3Parity.Eth.Blocks;
            var block = await blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(blockNumber));

            var transactions = new List<TransactionDto>();

            foreach (var transaction in block.Transactions)
            {
                var transferActions = (await GetTransactionTracesAsync(transaction.TransactionHash))
                    .Skip(1)
                    // TODO: Ensure, that it is correct
                    .Where(x => new HexBigInteger(x.Action.Value) != new BigInteger(0))
                    .Where(x => x.Action.CallType == "call")
                    .Select(x => x.Action)
                    .Select(x => new TransactionDto
                    {
                        FromAddress = x.From,
                        ToAddress = x.To,
                        TransactionAmount = new HexBigInteger(x.Value).Value 
                    });

                
            }

            return transactions;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<string>> TryGetTransactionErrorsAsync(string txHash)
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
            
            var traces = await GetTransactionTracesAsync(txHash);

            return traces?
                .Select(x => x.Error)
                .Where(x => x.IsNotNullOrEmpty());
        }

        private async Task<IEnumerable<TransactionTrace>> GetTransactionTracesAsync(string txHash)
        {
            var request = new RpcRequest($"{Guid.NewGuid()}", "trace_transaction", txHash);

            return await _web3Parity.Client.SendRequestAsync<IEnumerable<TransactionTrace>>(request);
        }
    }
}
