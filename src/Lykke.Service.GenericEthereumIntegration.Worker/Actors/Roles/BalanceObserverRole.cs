using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;


namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles
{
    [UsedImplicitly]
    public class BalanceObserverRole : IBalanceObserverRole
    {
        private readonly IBalanceObserverTaskRepository _balanceObserverTaskRepository;
        private readonly IBlockchainService _blockchainService;
        private readonly IObservableBalanceRepository _observableBalanceRepository;


        public BalanceObserverRole(
            IBalanceObserverTaskRepository balanceObserverTaskRepository,
            IBlockchainService blockchainService,
            IObservableBalanceRepository observableBalanceRepository)
        {
            _balanceObserverTaskRepository = balanceObserverTaskRepository;
            _blockchainService = blockchainService;
            _observableBalanceRepository = observableBalanceRepository;
        }


        public async Task CheckAndUpdateBalanceAsync(CheckAndUpdateBalance message)
        {
            var observableBalance = await _observableBalanceRepository.TryGetAsync(message.Address);

            if (observableBalance != null && observableBalance.BlockNumber < message.BlockNumber)
            {
                var amount = await _blockchainService.GetBalanceAsync(message.Address, message.BlockNumber);

                await _observableBalanceRepository.UpdateAmountAsync(message.Address, amount, message.BlockNumber);
            }

            await _balanceObserverTaskRepository.CompleteAsync(message.CompletionToken);
        }
    }
}
