using Autofac;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Repositories.Factories;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.SettingsReader;


namespace Lykke.Service.GenericEthereumIntegration.Api.Repositories.Modules
{
    public class RepositoriesModule : Module
    {
        private readonly RepositoryFactory _repositoryFactory;
        

        public RepositoriesModule(
            [NotNull] IReloadingManager<string> connectionString,
            [NotNull] ILog log)
        {
            _repositoryFactory = new RepositoryFactory(connectionString, log);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(x => _repositoryFactory.BuildGasPriceRepository())
                .As<IGasPriceRepository>()
                .SingleInstance();

            builder
                .Register(x => _repositoryFactory.BuildHistoricalTransactionRepository())
                .As<IHistoricalTransactionRepository>()
                .SingleInstance();

            builder
                .Register(x => _repositoryFactory.BuildObservableAddressRepository())
                .As<IObservableAddressRepository>()
                .SingleInstance();

            builder
                .Register(x => _repositoryFactory.BuildObservableBalanceRepository())
                .As<IObservableBalanceRepository>()
                .SingleInstance();

            builder
                .Register(x => _repositoryFactory.BuildTransactionRepository())
                .As<ITransactionRepository>()
                .SingleInstance();
        }
    }
}
