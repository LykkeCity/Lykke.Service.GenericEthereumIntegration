using System.Numerics;
using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Service;


namespace Lykke.Service.GenericEthereumIntegration.Api.Validation
{
    [UsedImplicitly]
    public class BuildSingleTransactionRequestValidator : AbstractValidator<BuildSingleTransactionRequest>
    {
        public BuildSingleTransactionRequestValidator(
            AssetSettings assetSettings)
        {
            RuleFor(x => x.Amount)
                .Must(amount => BigInteger.TryParse(amount, out var amountParsed) && amountParsed > 0)
                .WithMessage(x => "Amount should be a positive integer.");

            RuleFor(x => x.FromAddress)
                .AddressMustBeValid();

            RuleFor(x => x.OperationId)
                .OperationIdMustBeNonEmptyGuid();

            RuleFor(x => x.ToAddress)
                .AddressMustBeValid();

            RuleFor(x => x.AssetId)
                .AssetMustBeSupported(assetSettings);
        }
    }
}
