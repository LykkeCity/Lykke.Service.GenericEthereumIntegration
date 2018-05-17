using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract.Transactions;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Validation
{
    [UsedImplicitly]
    public class SignTransactionRequestValidator : AbstractValidator<SignTransactionRequest>
    {
        public SignTransactionRequestValidator()
        {
            RuleFor(x => x.PrivateKeys)
                .Must((keys, ctx) => keys.PrivateKeys?.Count == 1)
                .WithMessage(x => $"{nameof(x.PrivateKeys)} should contain exact one private key.");
        }
    }
}
