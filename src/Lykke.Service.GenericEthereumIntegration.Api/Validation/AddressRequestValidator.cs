using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Models;

namespace Lykke.Service.GenericEthereumIntegration.Api.Validation
{
    [UsedImplicitly]
    public class AddressRequestValidator : AbstractValidator<AddressRequest>
    {
        public AddressRequestValidator()
        {
            RuleFor(x => x.Address)
                .AddressMustBeValid();
        }
    }
}
