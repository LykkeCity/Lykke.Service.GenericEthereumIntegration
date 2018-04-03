using Autofac;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Common.Services.Factories;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services.Modules
{
    public class ServicesModule : Module
    {
        private readonly BlockchainServiceFactory _blockchainServiceFactory;


        public ServicesModule(
            RpcNodeSettings rpcNodeSettings)
        {
            _blockchainServiceFactory = new BlockchainServiceFactory(rpcNodeSettings);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(x => _blockchainServiceFactory.Build())
                .AsSelf()
                .SingleInstance();
            
            builder
                .RegisterType<AddressValidationService>()
                .As<IAddressValidationService>()
                .SingleInstance();

            builder
                .RegisterType<AssetService>()
                .As<IAssetService>()
                .SingleInstance();

            builder
                .RegisterType<GasPriceOracleService>()
                .As<IGasPriceOracleService>()
                .SingleInstance();


            builder
                .RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder
                .RegisterType<HistoricalTransactionService>()
                .As<IHistoricalTransactionService>()
                .SingleInstance();

            builder
                .RegisterType<ObservableAddressService>()
                .As<IObservableAddressService>()
                .SingleInstance();

            builder
                .RegisterType<ObservableBalanceService>()
                .As<IObservableBalanceService>()
                .SingleInstance();

            builder
                .RegisterType<TransactionService>()
                .As<ITransactionService>()
                .SingleInstance();
        }
    }
}
