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
    public class OperationMonitorActor : ReceiveActor
    {
        private readonly IOperationMonitorRole _role;

        public OperationMonitorActor(
            IOperationMonitorRole role)
        {
            _role = role;


            ReceiveAsync<CheckAndUpdateOperation>(
                ProcessMessageAsync);
        }

        public static IActorRef Create(IActorContext parentContext, int nrOfInstances)
        {
            return parentContext.ActorOf
            (
                parentContext.DI()
                    .Props<OperationMonitorActor>()
                    .WithRouter(new SmallestMailboxPool(nrOfInstances)),
                "balance-observer"
            );
        }


        private async Task ProcessMessageAsync(CheckAndUpdateOperation message)
        {
            try
            {
                await _role.CheckAndUpdateOperationAsync(message);
            }
            finally
            {
                Sender.Tell(new TaskCompleted());
            }
        }
    }
}
