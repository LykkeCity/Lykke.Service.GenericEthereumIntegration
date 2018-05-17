using System.Linq;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Assets;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Models;
using Lykke.Service.GenericEthereumIntegration.Common.Controllers;
using Lykke.Service.GenericEthereumIntegration.Common.Validation;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.Api.Controllers
{
    [PublicAPI, Route("/api/assets")]
    public class AssetsController : IntegrationControllerBase
    {
        private readonly IAssetService _assetService;

        public AssetsController(
            IAssetService assetService)
        {
            _assetService = assetService;
        }


        [HttpGet("{assetId}"), ValidateModel]
        public IActionResult GetAsset(AssetRequest request)
        {
            var asset = _assetService.GetAsset(request.AssetId);

            return Ok(new AssetResponse
            {
                Accuracy = asset.Accuracy,
                AssetId = asset.AssetId,
                Name = asset.Name
            });
        }
        
        [HttpGet, ValidateModel]
        public IActionResult GetAssetList(PaginationRequest request)
        {
            var assets = _assetService.GetAssets();

            return Ok(new PaginationResponse<AssetContract>
            {
                Continuation = null,
                Items = assets.Select(x => new AssetContract
                {
                    Accuracy = x.Accuracy,
                    AssetId = x.AssetId,
                    Name = x.Name
                }).ToList()
            });
        }
    }
}
