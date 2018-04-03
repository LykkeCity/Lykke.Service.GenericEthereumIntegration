using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors
{
    [UsedImplicitly]
    public class TransactionIndexerActor : ReceiveActor
    {
        private readonly ITransactionIndexerRole _role;


        public TransactionIndexerActor(
            ITransactionIndexerRole role)
        {
            _role = role;

            
            ReceiveAsync<IndexBlock>(
                ProcessMessageAsync);
        }

        public static IActorRef Create(IActorContext parentContext, int nrOfInstances)
        {
            return parentContext.ActorOf
            (
                parentContext.DI()
                    .Props<OperationMonitorActor>()
                    .WithRouter(new SmallestMailboxPool(nrOfInstances)),
                "transaction-indexer"
            );
        }


        private async Task ProcessMessageAsync(IndexBlock message)
        {
            try
            {
                await _role.IndexBlockAsync(message);
                
                Sender.Tell(new BlockIndexed
                (
                    blockNumber: message.BlockNumber
                ));
            }
            catch (Exception e)
            {
                // TODO: Log error

                Context.System.Scheduler.ScheduleTellOnce
                (
                    delay: TimeSpan.FromSeconds(10),
                    receiver: Self,
                    message: message,
                    sender: Sender
                );
            }
        }
    }
}
