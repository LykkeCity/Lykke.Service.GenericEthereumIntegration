﻿using Autofac;
using Common.Log;
using Lykke.Service.GenericEthereumIntegration.Api.Repositories.Factories;
using Lykke.SettingsReader;


namespace Lykke.Service.GenericEthereumIntegration.Api.Repositories.Modules
{
    public class RepositoriesModule : Module
    {
        private readonly RepositoryFactory _repositoryFactory;
        

        public RepositoriesModule(
            IReloadingManager<string> connectionString,
            ILog log)
        {
            _repositoryFactory = new RepositoryFactory(connectionString, log);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(x => _repositoryFactory.BuildGasPriceRepository())
                .AsSelf()
                .SingleInstance();

            builder
                .Register(x => _repositoryFactory.BuildHistoricalTransactionRepository())
                .AsSelf()
                .SingleInstance();

            builder
                .Register(x => _repositoryFactory.BuildObservableAddressRepository())
                .AsSelf()
                .SingleInstance();

            builder
                .Register(x => _repositoryFactory.BuildObservableBalanceRepository())
                .AsSelf()
                .SingleInstance();

            builder
                .Register(x => _repositoryFactory.BuildTransactionRepository())
                .AsSelf()
                .SingleInstance();
        }
    }
}
