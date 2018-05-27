﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;


namespace Lykke.Service.GenericEthereumIntegration.Api.Services
{
    [UsedImplicitly]
    public class ObservableBalanceService : IObservableBalanceService
    {
        private readonly IObservableBalanceRepository _observableBalanceRepository;
        private readonly AssetSettings _assetSettings;


        public ObservableBalanceService(
            AssetSettings assetSettings,
            IObservableBalanceRepository observableBalanceRepository)
        {
            _assetSettings = assetSettings;
            _observableBalanceRepository = observableBalanceRepository;
        }


        public async Task BeginObservationAsync(string address)
        {
            #region Validation
            
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(address));
            }

            if (!await AddressChecksum.ValidateAsync(address))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(address));
            }
            
            #endregion
            
            if (await _observableBalanceRepository.TryAddAsync(address))
            {
                return;
            }

            throw new ConflictException($"Specified address [{address}] has already been added to the observation list.");
        }

        public async Task EndObservationAsync(string address)
        {
            #region Validation
            
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldNotBeNullOrEmpty, nameof(address));
            }

            if (!await AddressChecksum.ValidateAsync(address))
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeValidAddress, nameof(address));
            }
            
            #endregion
            
            if (await _observableBalanceRepository.DeleteIfExistsAsync(address))
            {
                return;
            }

            throw new NotFoundException($"Specified address [{address}] has not been found in the observation list.");
        }

        public async Task<(IEnumerable<ObservableBalanceDto> balances, string assetId, string continuationToken)> GetBalancesAsync(int take, string continuationToken)
        {
            #region Validation
            
            if (take <= 0)
            {
                throw new ArgumentException(CommonExceptionMessages.ShouldBeGreaterThanZero, nameof(take));
            }
            
            #endregion
            
            IEnumerable<ObservableBalanceDto> balances;

            (balances, continuationToken) = await _observableBalanceRepository.GetAllWithNonZeroAmountAsync(take, continuationToken);

            return (balances, _assetSettings.Id, continuationToken);
        }
    }
}
