using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Extensions
{
    public static class ActorSystemExtensions
    {
        public static IActorRef Create<T>(this ActorSystem actorSystem, string name)
            where T : ActorBase
        {
            return actorSystem.ActorOf
            (
                actorSystem.DI().Props<T>(),
                name
            );
        }

        public static ActorSystem WithContainer(this ActorSystem actorSystem, IContainer container)
        {
            // It's the way, how we add dependency injection to Akka.net
            // ReSharper disable once ObjectCreationAsStatement
            new AutoFacDependencyResolver(container, actorSystem);

            return actorSystem;
        }
    }
}
