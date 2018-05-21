using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services
{
    [UsedImplicitly]
    public class ObservableAddressService : IObservableAddressService
    {
        private readonly IObservableAddressRepository _observableAddressRepository;


        public ObservableAddressService(
            IObservableAddressRepository observableAddressRepository)
        {
            _observableAddressRepository = observableAddressRepository;
        }


        public async Task AddToIncomingObservationListAsync(string address)
        {
            #region Validation
            
            await ValidateInputParameterAsync(address);
            
            #endregion
            
            if (await _observableAddressRepository.TryAddToIncomingObservationListAsync(address))
            {
                return;
            }

            throw new NotFoundException($"Specified address [{address}] has not been found in the incoming observation list.");
        }

        public async Task AddToOutgoingObservationListAsync(string address)
        {
            #region Validation
            
            await ValidateInputParameterAsync(address);
            
            #endregion
            
            if (await _observableAddressRepository.TryAddToOutgoingObservationListAsync(address))
            {
                return;
            }

            throw new NotFoundException($"Specified address [{address}] has not been found in the outgoing observation list.");
        }

        public async Task DeleteFromIncomingObservationListAsync(string address)
        {
            #region Validation
            
            await ValidateInputParameterAsync(address);
            
            #endregion
            
            if (await _observableAddressRepository.TryDeleteFromIncomingObservationListAsync(address))
            {
                return;
            }

            throw new ConflictException($"Specified address [{address}] has already bbeen added to the incoming observation list.");
        }

        public async Task DeleteFromOutgoingObservationListAsync(string address)
        {
            #region Validation
            
            await ValidateInputParameterAsync(address);
            
            #endregion
            
            if (await _observableAddressRepository.TryDeleteFromOutgoingObservationListAsync(address))
            {
                return;
            }

            throw new ConflictException($"Specified address [{address}] has already bbeen added to the outgoing observation list.");
        }

        private async Task ValidateInputParameterAsync(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException("Should not be null or empty.", nameof(address));
            }

            if (await AddressChecksum.ValidateAsync(address))
            {
                throw new ArgumentException("Should be a valid address.", nameof(address));
            }
        }
    }
}
