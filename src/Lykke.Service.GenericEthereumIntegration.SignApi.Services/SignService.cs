using System;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Services
{
    [UsedImplicitly]
    public class SignService : ISignService
    {
        [Pure]
        public string SignTransaction(string transactionHex, string privateKey)
        {
            if (string.IsNullOrEmpty(transactionHex))
            {
                throw new ArgumentException("Should not be null or empty.", nameof (transactionHex));
            }
            
            if (string.IsNullOrEmpty(privateKey))
            {
                throw new ArgumentException("Should not be null or empty.", nameof (privateKey));
            }
            
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
