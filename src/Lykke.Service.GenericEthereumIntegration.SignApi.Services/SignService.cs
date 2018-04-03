using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Services
{
    public class SignService : ISignService
    {
        public string SignTransaction(string transactionHex, string privateKey)
        {
            var transactionBytes = transactionHex.HexToByteArray();
            var transaction = new Transaction(transactionBytes);
            var key = new EthECKey(privateKey);

            transaction.Sign(key);

            return transaction
                .GetRLPEncoded()
                .ToHex();
        }
    }
}
