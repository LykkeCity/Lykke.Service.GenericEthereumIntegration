﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration;
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


        public async Task<(IEnumerable<HistoricalTransactionDto> Transactions, string AssetId)> GetIncomingHistoryAsync(string address, int take, string afterHash)
        {
            #region Validation
            
            if (address.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(address));
            }

            if (!await AddressChecksum.ValidateAsync(address))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(address));
            }

            if (take <= 1)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanOne, nameof(take));
            }
            
            #endregion
            
            var transactions = await _historicalTransactionRepository.GetIncomingHistoryAsync(address, take, afterHash);

            return (transactions, _assetSettings.Id);
        }

        public async Task<(IEnumerable<HistoricalTransactionDto> Transactions, string AssetId)> GetOutgoingHistoryAsync(string address, int take, string afterHash)
        {
            #region Validation
            
            if (address.IsNullOrEmpty())
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(address));
            }

            if (!await AddressChecksum.ValidateAsync(address))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(address));
            }

            if (take <= 1)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanOne, nameof(take));
            }
            
            #endregion
            
            var transactions = await _historicalTransactionRepository.GetOutgoingHistoryAsync(address, take, afterHash);

            return (transactions, _assetSettings.Id);
        }
    }
}
