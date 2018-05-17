using System;
using FluentValidation;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;


namespace Lykke.Service.GenericEthereumIntegration.Api.Validation
{
    public static class Rules
    {
        public static void OperationIdMustBeNonEmptyGuid<T>(this IRuleBuilderInitial<T, Guid> ruleBuilder)
        {
            ruleBuilder
                .Must(operationId => operationId != Guid.Empty)
                .WithMessage(x => "Specified operation id is empty.");
        }

        public static void AddressMustBeValid<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
        {
            ruleBuilder
                .MustAsync((address, ct) => AddressValidator.ValidateAsync(address))
                .WithMessage(x => $"Specified address [{x}] is invalid.");
        }

        public static void AssetMustBeSupported<T>(this IRuleBuilderInitial<T, string> ruleBuilder, AssetSettings assetSettings)
        {
            ruleBuilder
                .Must(assetId => assetId == assetSettings.Id)
                .WithMessage(x => $"Specified asset [{x}] is not supported.");
        }
    }
}
