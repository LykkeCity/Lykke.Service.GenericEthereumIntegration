using Common.Log;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Repositories.Entities;
using Lykke.Service.GenericEthereumIntegration.Common.Repositories.Factories;
using Lykke.SettingsReader;

namespace Lykke.Service.GenericEthereumIntegration.Api.Repositories.Factories
{
    internal class RepositoryFactory : RepositoryFactoryBase
    {
        internal RepositoryFactory(
            [NotNull] IReloadingManager<string> connectionString,
            [NotNull] ILog log) 
            : base(
                connectionString,
                log)
        {

        }

        [NotNull]
        public IGasPriceRepository BuildGasPriceRepository()
        {
            return new GasPriceRepository
            (
                table: CreateTable<GasPriceEntity>(DynamicSettingsTable)
            );
        }
    }
}
