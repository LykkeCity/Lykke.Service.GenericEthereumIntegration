using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
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
            throw new NotImplementedException();
        }

        public override async Task<string> GetTransactionErrorAsync(string txHash)
        {
            var request = new RpcRequest($"{Guid.NewGuid()}", "trace_transaction", txHash);
            var response = await _web3Parity.Client.SendRequestAsync<JArray>(request);

            return response.Select(x => x["error"]?.ToString()).FirstOrDefault();
        }
    }
}
