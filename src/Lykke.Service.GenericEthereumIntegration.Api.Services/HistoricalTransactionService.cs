using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;


namespace Lykke.Service.GenericEthereumIntegration.Api.Services
{
    [UsedImplicitly]
    public class HistoricalTransactionService : IHistoricalTransactionService
    {
        private readonly IHistoricalTransactionRepository _historicalTransactionRepository;
        private readonly AssetSettings _assetSettings;


        public HistoricalTransactionService(
            AssetSettings assetSettings,
            IHistoricalTransactionRepository historicalTransactionRepository)
        {
            _historicalTransactionRepository = historicalTransactionRepository;
            _assetSettings = assetSettings;
        }


        public async Task<(IEnumerable<HistoricalTransactionDto> transactions, string assetId)> GetIncomingHistoryAsync(string address, int take, string afterHash)
        {
            await ValidatInputParametersAsync(address, take);
            
            var transactions = await _historicalTransactionRepository.GetIncomingHistory(address, take, afterHash);

            return (transactions, _assetSettings.Id);
        }

        public async Task<(IEnumerable<HistoricalTransactionDto> transactions, string assetId)> GetOutgoingHistoryAsync(string address, int take, string afterHash)
        {
            await ValidatInputParametersAsync(address, take);
            
            var transactions = await _historicalTransactionRepository.GetOutgoingHistory(address, take, afterHash);

            return (transactions, _assetSettings.Id);
        }

        private async Task ValidatInputParametersAsync(string address, int take)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException("Should not be null or empty.", nameof(address));
            }

            if (await AddressValidator.ValidateAsync(address))
            {
                throw new ArgumentException("Should be a valid address.", nameof(address));
            }

            if (take <= 1)
            {
                throw new ArgumentException("Should be greater than one.", nameof(take));
            }
        }
    }
}
