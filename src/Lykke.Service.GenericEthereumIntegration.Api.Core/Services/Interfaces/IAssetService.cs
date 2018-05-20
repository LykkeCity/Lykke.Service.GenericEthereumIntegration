using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IAssetService
    {
        [NotNull]
        AssetDto GetAsset([NotNull] string assetId);

        [NotNull, ItemNotNull]
        IEnumerable<AssetDto> GetAssets();
    }
}
