using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.TDK;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Tests.Actors.Roles
{
    [TestClass]
    public class OperationMonitorRoleTests
    {
        [TestMethod]
        public async Task CheckAndUpdateOperationAsync__TransactionDoesNotExist__TaskMarkedAsCompleted()
        {
            var message = CreateCheckAndUpdateOperationMessage(transactionFailed: false);
            
            var roleBuilder = new OperationMonitorRoleBuilder
            {
                TryGetTransactionResult = null
            };

            var role = roleBuilder.Build();
            
            await role.CheckAndUpdateOperationAsync(message);
            
            roleBuilder
                .OperationMonitorTaskRepository
                .Verify(x => x.CompleteAsync(message.CompletionToken), Times.Once);
        }
        
        [TestMethod]
        public async Task CheckAndUpdateOperationAsync__TransactionSucceeded__TransactionUpdated_And_TaskMarkedAsCompleted()
        {
            var message = CreateCheckAndUpdateOperationMessage(transactionFailed: false);
            var transaction = new Mock<ITransactionAggregate>();

            transaction
                .SetupGet(x => x.State)
                .Returns(TransactionState.InProgress);

            var roleBuilder = new OperationMonitorRoleBuilder
            {
                TryGetTransactionResult = transaction.Object
            };

            var role = roleBuilder.Build();
            
            await role.CheckAndUpdateOperationAsync(message);
            
            transaction
                .Verify(x => x.OnCompleted(message.TransactionBlock), Times.Once);
            
            roleBuilder
                .TransactionRepository
                .Verify(x => x.UpdateAsync(transaction.Object), Times.Once);
            
            roleBuilder
                .OperationMonitorTaskRepository
                .Verify(x => x.CompleteAsync(message.CompletionToken), Times.Once);
        }
        
        [TestMethod]
        public async Task CheckAndUpdateOperationAsync__TransactionFailed__TransactionUpdated_And_TaskMarkedAsCompleted()
        {
            var message = CreateCheckAndUpdateOperationMessage(transactionFailed: true);
            var transaction = new Mock<ITransactionAggregate>();

            transaction
                .SetupGet(x => x.State)
                .Returns(TransactionState.InProgress);

            var roleBuilder = new OperationMonitorRoleBuilder
            {
                TryGetTransactionResult = transaction.Object
            };

            var role = roleBuilder.Build();
            
            await role.CheckAndUpdateOperationAsync(message);
            
            transaction
                .Verify(x => x.OnFailed(message.TransactionBlock, message.TransactionError), Times.Once);
            
            roleBuilder
                .TransactionRepository
                .Verify(x => x.UpdateAsync(transaction.Object), Times.Once);
            
            roleBuilder
                .OperationMonitorTaskRepository
                .Verify(x => x.CompleteAsync(message.CompletionToken), Times.Once);
        }
        
        [TestMethod]
        public async Task CheckAndUpdateOperationAsync__TransactionHasAlreadyBeenUpdated__TaskMarkedAsCompleted()
        {
            var message = CreateCheckAndUpdateOperationMessage(transactionFailed: false);
            var transaction = new Mock<ITransactionAggregate>();

            transaction
                .SetupGet(x => x.IsCompleted)
                .Returns(true);

            var roleBuilder = new OperationMonitorRoleBuilder
            {
                TryGetTransactionResult = transaction.Object
            };

            var role = roleBuilder.Build();
            
            await role.CheckAndUpdateOperationAsync(message);
            
            roleBuilder
                .TransactionRepository
                .Verify(x => x.UpdateAsync(It.IsAny<ITransactionAggregate>()), Times.Never);
            
            roleBuilder
                .OperationMonitorTaskRepository
                .Verify(x => x.CompleteAsync(message.CompletionToken), Times.Once);
        }

        private static CheckAndUpdateOperation CreateCheckAndUpdateOperationMessage(bool transactionFailed)
        {
            var random = new Random();
            
            return new CheckAndUpdateOperation
            (
                transactionBlock: random.Next(0, 3377450),
                transactionError: transactionFailed ? "Transaction error description" : null,
                transactionFailed: transactionFailed,
                transactionHash: TestValues.ValidTransactionHash1,
                completionToken: $"{Guid.NewGuid()}"
            );
        }


        [PublicAPI]
        private class OperationMonitorRoleBuilder
        {
            private ITransactionAggregate _tryGetTransactionResult;
            

            public OperationMonitorRoleBuilder()
            {
                OperationMonitorTaskRepository = new Mock<IOperationMonitorTaskRepository>();
                TransactionRepository = new Mock<ITransactionRepository>();
            }
            
            public Mock<IOperationMonitorTaskRepository> OperationMonitorTaskRepository { get; }
            
            public Mock<ITransactionRepository> TransactionRepository { get; }

            public ITransactionAggregate TryGetTransactionResult
            {
                get => _tryGetTransactionResult;
                set
                {
                    _tryGetTransactionResult = value;

                    TransactionRepository
                        .Setup(x => x.TryGetAsync(It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }


            public OperationMonitorRole Build()
            {
                return new OperationMonitorRole
                (
                    OperationMonitorTaskRepository.Object,
                    TransactionRepository.Object
                );
            }
        }
    }
}
