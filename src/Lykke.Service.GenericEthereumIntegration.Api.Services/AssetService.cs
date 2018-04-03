using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Service;


namespace Lykke.Service.GenericEthereumIntegration.Api.Services
{
    [UsedImplicitly]
    public class AssetService : IAssetService
    {
        private readonly AssetDto _asset;
        private readonly AssetDto[] _assets;

        public AssetService(
            AssetSettings settings)
        {
            _asset = new AssetDto
            {
                Accuracy = settings.Accuracy,
                AssetId = settings.AssetId,
                Name = settings.Name
            };

            _assets = new[]
            {
                _asset
            };
        }

        public AssetDto GetAsset(string assetId)
        {
            if (assetId == _asset.AssetId)
            {
                return _asset;
            }

            throw new NotFoundException($"Asset with the specified id [{assetId}] has not been found.");
        }

        public IEnumerable<AssetDto> GetAssets()
        {
            return _assets;
        }
    }
}
