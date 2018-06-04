using System;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces;
using MessagePack;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Services
{
    [UsedImplicitly]
    public class SignService : ISignService
    {
        public string SignTransaction(string transactionHex, string privateKey)
        {
            #region Validation
            
            if (transactionHex.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof (transactionHex));
            }

            if (transactionHex.IsNotHexString())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidHexString, nameof (transactionHex));
            }
            
            if (privateKey.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof (privateKey));
            }
            
            if (privateKey.IsNotHexString())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidHexString, nameof (privateKey));
            }
            
            #endregion
            
            var transactionBytes = transactionHex.HexToByteArray();
            var transactionDto = MessagePackSerializer.Deserialize<UnsignedTransactionDto>(transactionBytes);

            var transaction = new Transaction
            (
                to: transactionDto.To,
                amount: transactionDto.Amount,
                nonce: transactionDto.Nonce,
                gasPrice: transactionDto.GasPrice,
                gasLimit: transactionDto.GasAmount,
                data: null
            );
            
            var key = new EthECKey(privateKey);

            transaction.Sign(key);

            return transaction
                .GetRLPEncoded()
                .ToHex(prefix: true);
        }
    }
}
