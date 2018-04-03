using System.Collections.Generic;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IAssetService
    {
        AssetDto GetAsset(string assetId);

        IEnumerable<AssetDto> GetAssets();
    }
}
