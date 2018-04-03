using System.Numerics;
using AzureStorage;
using AzureStorage.Blob;
using AzureStorage.Queue;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using Common.Log;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Metamodel;
using Lykke.AzureStorage.Tables.Entity.Metamodel.Providers;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Repositories.Entities;
using Lykke.Service.GenericEthereumIntegration.Common.Repositories.Serializers;
using Lykke.SettingsReader;


namespace Lykke.Service.GenericEthereumIntegration.Common.Repositories.Factories
{
    public abstract class RepositoryFactoryBase
    {
        private const string HistoricalTransactionTable = "HistoricalTransactions";
        private const string HistoricalTransactionBlockIndexTable = "HistoricalTransactionsBlockIndex";
        private const string HistoricalTransactionFromAddressIndexTable = "HistoricalTransactionsFromAddressIndex";
        private const string HistoricalTransactionToAddressIndexTable = "HistoricalTransactionsToAddressIndex";
        private const string ObservableAddressTable = "ObservableAddresses";
        private const string ObservableBalanceTable = "ObservableBalances";
        private const string TransactionTable = "Transactions";
        private const string TransactionIndexSignedTxHashIndex = "TransactionIndexSignedTxHashIndex";

        protected const string DynamicSettingsTable = "DynamicSettings";


        private readonly IReloadingManager<string> _connectionString;
        private readonly ILog _log;


        protected RepositoryFactoryBase(
            IReloadingManager<string> connectionString,
            ILog log)
        {
            _connectionString = connectionString;
            _log = log;

            var provider = new CompositeMetamodelProvider()
                .AddProvider
                (
                    new AnnotationsBasedMetamodelProvider()
                )
                .AddProvider
                (
                    new ConventionBasedMetamodelProvider()
                        .AddTypeSerializerRule
                        (
                            t => t == typeof(BigInteger),
                            s => new BigIntegerSerializer()
                        )
                );

            EntityMetamodel.Configure(provider);
        }


        protected IBlobStorage CreateBlobStorage()
        {
            return AzureBlobStorage.Create(_connectionString);
        }

        protected INoSQLTableStorage<AzureIndex> CreateIndexTable(string indexTableName)
        {
            return AzureTableStorage<AzureIndex>.Create(_connectionString, indexTableName, _log);
        }

        protected INoSQLTableStorage<T> CreateTable<T>(string tableName)
            where T : AzureTableEntity, new()
        {
            return AzureTableStorage<T>.Create(_connectionString, tableName, _log);
        }

        protected IQueueExt CreateQueue(string queueName)
        {
            return AzureQueueExt.Create(_connectionString, queueName);
        }

        public IHistoricalTransactionRepository BuildHistoricalTransactionRepository()
        {
            return new HistoricalTransactionRepository
            (
                blockIndexTable: CreateIndexTable(HistoricalTransactionBlockIndexTable),
                fromAddressIndexTable: CreateIndexTable(HistoricalTransactionFromAddressIndexTable),
                table: CreateTable<HistoricalTransactionEntity>(HistoricalTransactionTable),
                toAddressIndexTable: CreateIndexTable(HistoricalTransactionToAddressIndexTable)
            );
        }

        public IObservableAddressRepository BuildObservableAddressRepository()
        {
            return new ObservableAddressRepository
            (
                table: CreateTable<ObservableAddressEntity>(ObservableAddressTable)
            );
        }

        public IObservableBalanceRepository BuildObservableBalanceRepository()
        {
            return new ObservableBalanceRepository
            (
                table: CreateTable<ObservableBalanceEntity>(ObservableBalanceTable)
            );
        }

        public ITransactionRepository BuildTransactionRepository()
        {
            return new TransactionRepository
            (
                table: CreateTable<TransactionEntity>(TransactionTable),
                signedTxHashIndexTable: CreateIndexTable(TransactionIndexSignedTxHashIndex)
            );
        }
    }
}
