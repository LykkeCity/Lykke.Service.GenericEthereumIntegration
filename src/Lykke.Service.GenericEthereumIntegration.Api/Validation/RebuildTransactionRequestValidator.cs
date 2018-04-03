using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract.Transactions;


namespace Lykke.Service.GenericEthereumIntegration.Api.Validation
{
    [UsedImplicitly]
    public class RebuildTransactionRequestValidator : AbstractValidator<RebuildTransactionRequest>
    {
        public RebuildTransactionRequestValidator()
        {
            RuleFor(x => x.OperationId)
                .OperationIdMustBeNonEmptyGuid();
            
            RuleFor(x => x.FeeFactor)
                .Must(feeFactor => feeFactor > 1m)
                .WithMessage(x => "Fee factor should be greater then one.");
        }
    }
}
