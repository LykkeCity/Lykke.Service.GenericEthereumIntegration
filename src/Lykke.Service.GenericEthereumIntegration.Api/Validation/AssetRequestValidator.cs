using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Models;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration;


namespace Lykke.Service.GenericEthereumIntegration.Api.Validation
{
    [UsedImplicitly]
    public class AssetRequestValidator : AbstractValidator<AssetRequest>
    {
        public AssetRequestValidator(
            AssetSettings assetSettings)
        {
            RuleFor(x => x.AssetId)
                .AssetMustBeSupported(assetSettings);
        }
    }
}
