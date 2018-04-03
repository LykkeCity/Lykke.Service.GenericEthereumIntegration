using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Akka.Actor;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Settings.Service;


namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors
{
    [UsedImplicitly]
    public class TransactionIndexerDispatcherActor : ReceiveActor
    {
        private readonly ITransactionIndexerDispatcherRole _role;
        private readonly IActorRef _transactionIndexer;

        private bool _shutdownReceived;
        
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        public TransactionIndexerDispatcherActor(
            ITransactionIndexerDispatcherRole role,
            GenericEthereumIntegrationWorkerSettings settings)
        {
            _role = role;
            _transactionIndexer = TransactionIndexerActor.Create(Context, settings.NrOfTransactionIndexers);
            
            SendIndexNextBlocksBatchCommand(withDelay: false);

            Become(Idle);
        }

        private void SendIndexNextBlocksBatchCommand(bool withDelay)
        {
            if (withDelay)
            {
                Context.System.Scheduler.ScheduleTellOnce
                (
                    // TODO: Get delay from config
                    delay: TimeSpan.FromSeconds(5), 
                    receiver: Self,
                    message: new IndexNextBlocksBatch(),
                    sender: Self
                );
            }
            else
            {
                Self.Tell(new IndexNextBlocksBatch());
            }
        }

        #region Idle State

        private void Idle()
        {
            Receive<Shutdown>(
                msg => ProcessMessageWhenIdle(msg));

            ReceiveAsync<IndexNextBlocksBatch>(
                ProcessMessageWhenIdleAsync);
        }

        private void ProcessMessageWhenIdle(Shutdown message)
        {
            Context.Stop(Self);
        }

        private async Task ProcessMessageWhenIdleAsync(IndexNextBlocksBatch message)
        {
            var tasksBatch = await _role.BeginBatchIndexationAsync();
            
            if (_role.RemainingBatchSize != 0)
            {
                foreach (var task in tasksBatch)
                {
                    _transactionIndexer.Tell(task);
                }
                
                Become(Busy);
            }
            else
            {
                SendIndexNextBlocksBatchCommand(withDelay: true);
            }
        }


        #endregion

        #region Busy state

        private void Busy()
        {
            Receive<Shutdown>(
                msg => ProcessMessageWhenBusy(msg));

            ReceiveAsync<BlockIndexed>(
                ProcessMessageWhenBusyAsync);
        }

        private void ProcessMessageWhenBusy(Shutdown message)
        {
            _shutdownReceived = true;
        }

        private async Task ProcessMessageWhenBusyAsync(BlockIndexed message)
        {
            _role.MarkBlockAsIndexed(message.BlockNumber);
            
            if (_role.RemainingBatchSize == 0)
            {
                var (forkDetected, latestTrustedBlockNumber) = await _role.DetectForkAsync();

                if (forkDetected)
                {
                    // It is a very-very-very dangerous situation!
                    // TODO: Log message about fork

                    _role.ProcessFork(latestTrustedBlockNumber);
                }
                
                await _role.CompleteBatchIndexationAsync();
                
                if (!_shutdownReceived)
                {
                    SendIndexNextBlocksBatchCommand(withDelay: true);

                    Become(Idle);
                }
                else
                {
                    Context.Stop(Self);
                }
            }
        }

        #endregion
    }
}
