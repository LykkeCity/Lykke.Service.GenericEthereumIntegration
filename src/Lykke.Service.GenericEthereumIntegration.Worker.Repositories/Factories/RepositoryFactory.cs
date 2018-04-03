using Common.Log;
using Lykke.Service.GenericEthereumIntegration.Common.Repositories.Factories;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Repositories.Entities;
using Lykke.SettingsReader;


namespace Lykke.Service.GenericEthereumIntegration.Worker.Repositories.Factories
{
    public class RepositoryFactory : RepositoryFactoryBase
    {
        private const string BalanceObserverTaskQueue = "BalanceObserverTasks";
        private const string IndexedBlockTable = "IndexedBlocks";
        private const string OperationMonitorTaskQueue = "OperationMonitorTasks";

        public RepositoryFactory(IReloadingManager<string> connectionString, ILog log) 
            : base(connectionString, log)
        {

        }

        public IBalanceObserverTaskRepository BuildBalanceObserverTaskRepository()
        {
            return new BalanceObserverTaskRepository
            (
                queue: CreateQueue(BalanceObserverTaskQueue)
            );
        }

        public IIndexationStateRepository BuildIndexationStateRepository()
        {
            return new IndexationStateRepository
            (
                blobStorage: CreateBlobStorage()
            );
        }

        public IIndexedBlockRepository BuildIndexedBlockRepository()
        {
            return new IndexedBlockRepository
            (
                table: CreateTable<IndexedBlockEntity>(IndexedBlockTable)
            );
        }

        public IOperationMonitorTaskRepository BuildOperationMonitorTaskRepository()
        {
            return new OperationMonitorTaskRepository
            (
                queue: CreateQueue(OperationMonitorTaskQueue)
            );
        }
    }
}
