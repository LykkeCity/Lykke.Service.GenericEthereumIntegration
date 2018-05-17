using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
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

        public BlockchainServiceParity(Web3Parity web3Parity) 
            : base(web3Parity)
        {
            _web3Parity = web3Parity;
        }

        public override async Task<BigInteger> GetNextNonceAsync(string address)
        {
            var request = new RpcRequest($"{Guid.NewGuid()}", "parity_nextNonce", address);
            var response = await _web3Parity.Client.SendRequestAsync<string>(request);
            var result = new HexBigInteger(response);

            return result.Value;
        }

        public override async Task<IEnumerable<TransactionDto>> GetTransactionsAsync(BigInteger blockNumber)
        {
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

        public override async Task<string> GetTransactionErrorAsync(string txHash)
        {
            var traces = await GetTransactionTracesAsync(txHash);

            return traces.Select(x => x.Error).FirstOrDefault();
        }

        private async Task<IEnumerable<TransactionTrace>> GetTransactionTracesAsync(string txHash)
        {
            var request = new RpcRequest($"{Guid.NewGuid()}", "trace_transaction", txHash);

            return await _web3Parity.Client.SendRequestAsync<IEnumerable<TransactionTrace>>(request);
        }
    }
}
