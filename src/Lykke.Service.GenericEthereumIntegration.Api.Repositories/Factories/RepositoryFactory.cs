using Common.Log;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Repositories.Entities;
using Lykke.Service.GenericEthereumIntegration.Common.Repositories.Factories;
using Lykke.SettingsReader;

namespace Lykke.Service.GenericEthereumIntegration.Api.Repositories.Factories
{
    public class RepositoryFactory : RepositoryFactoryBase
    {
        public RepositoryFactory(
            IReloadingManager<string> connectionString,
            ILog log) 
            : base(
                connectionString,
                log)
        {

        }


        public IGasPriceRepository BuildGasPriceRepository()
        {
            return new GasPriceRepository
            (
                table: CreateTable<GasPriceEntity>(DynamicSettingsTable)
            );
        }
    }
}
