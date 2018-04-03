using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Models;

namespace Lykke.Service.GenericEthereumIntegration.Api.Validation
{
    [UsedImplicitly]
    public class OperationRequestValidator : AbstractValidator<OperationRequest>
    {
        public OperationRequestValidator()
        {
            RuleFor(x => x.OperationId)
                .OperationIdMustBeNonEmptyGuid();
        }
    }
}
