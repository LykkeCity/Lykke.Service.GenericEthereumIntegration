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
    public class BalanceObserverActor : ReceiveActor
    {
        private readonly IBalanceObserverRole _role;

        public BalanceObserverActor(
            IBalanceObserverRole role)
        {
            _role = role;


            ReceiveAsync<CheckAndUpdateBalance>(
                ProcessMessageAsync);
        }
        
        public static IActorRef Create(IActorContext parentContext, int nrOfInstances)
        {
            return parentContext.ActorOf
            (
                parentContext.DI()
                    .Props<BalanceObserverActor>()
                    .WithRouter(new SmallestMailboxPool(nrOfInstances)),
                "balance-observer"
            );
        }


        private async Task ProcessMessageAsync(CheckAndUpdateBalance message)
        {
            try
            {
                await _role.CheckAndUpdateBalanceAsync(message);
            }
            finally
            {
                Sender.Tell(new TaskCompleted());
            }
        }
    }
}
