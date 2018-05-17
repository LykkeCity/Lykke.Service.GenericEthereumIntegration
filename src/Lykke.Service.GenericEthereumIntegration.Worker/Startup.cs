using System;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Configuration;
using Autofac;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Modules;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Settings;
using Lykke.Service.GenericEthereumIntegration.Worker.Extensions;
using Lykke.Service.GenericEthereumIntegration.Worker.Modules;
using Lykke.Service.GenericEthereumIntegration.Worker.Services.Modules;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.GenericEthereumIntegration.Worker
{
    [UsedImplicitly]
    public sealed class Startup : Common.StartupBase<AppSettings>
    {
        private ActorSystem _actorSystem;
        private IActorRef _balanceObserverDispatcher;
        private IActorRef _transactionIndexerDispatcher;
        private IActorRef _operationMonitorDispatcher;


        public Startup(IHostingEnvironment env) 
            : base(env)
        {

        }

        protected override IReloadingManager<string> DbLogConnectionStringManager
            => AppSettings.Nested(x => x.GenericEthereumIntegrationWorker.Db.LogsConnString);

        protected override string ServiceType
            => "Worker";


        protected override Task OnStartApplication()
        {
            var systemConfig = ConfigurationFactory.FromResource
            (
                "Lykke.Service.GenericEthereumIntegration.Worker.SystemConfig.json",
                typeof(Startup).Assembly
            );

            _actorSystem = ActorSystem
                .Create("generic-ethereum-integration", systemConfig)
                .WithContainer(AppContainer);


            _balanceObserverDispatcher = 
                _actorSystem.Create<BalanceObserverDispatcherActor>("balance-observer-dispatcher");

            _operationMonitorDispatcher =
                _actorSystem.Create<OperationMonitorDispatcherActor>("operation-monitor-dispatcher");

            _transactionIndexerDispatcher =
                _actorSystem.Create<TransactionIndexerDispatcherActor>("transaction-indexer-dispatcher");
            

            return Task.CompletedTask;
        }

        protected override async Task OnStopApplication()
        {
            var coordinatedShtudown = CoordinatedShutdown.Get(_actorSystem);

            coordinatedShtudown.AddTask(CoordinatedShutdown.PhaseServiceStop, "shutdown-actors", async () =>
            {
                var shutdownMessage = new Shutdown();
                var shutdownTimeout = TimeSpan.FromMinutes(1);
                
                await Task.WhenAll
                (
                    _balanceObserverDispatcher.GracefulStop(shutdownTimeout),
                    _operationMonitorDispatcher.GracefulStop(shutdownTimeout),
                    _transactionIndexerDispatcher.GracefulStop(shutdownTimeout, shutdownMessage)
                );

                return Done.Instance;
            });

            await coordinatedShtudown.Run();
        }

        protected override void PreBuildContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterModule(new GenericEthereumIntegrationWorkerModule(AppSettings.CurrentValue.GenericEthereumIntegrationWorker))
                .RegisterModule<CoreModule>()
                .RegisterModule<ServicesModule>();
        }
    }
}
