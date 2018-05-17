using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Models;

namespace Lykke.Service.GenericEthereumIntegration.Api.Validation
{
    [UsedImplicitly]
    public class TransactionHistoryRequestValidator : AbstractValidator<TransactionHistoryRequest>
    {
        public TransactionHistoryRequestValidator()
        {
            RuleFor(x => x.Address)
                .AddressMustBeValid();

            RuleFor(x => x.Take)
                .GreaterThan(1);
        }
    }
}
