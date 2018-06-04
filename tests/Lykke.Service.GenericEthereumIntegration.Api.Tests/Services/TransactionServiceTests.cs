using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Api.Services;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.TDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services
{
    [TestClass]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    public class TransactionServiceTests
    {
        #region BroadcastTransactionAsync

        [TestMethod]
        public async Task BroadcastTransactionAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string signedTxData = nameof(signedTxData);
            
            var serviceBuilder = new TransactionServiceBuilder();
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterHexStringParameter(signedTxData);

            var service = serviceBuilder.Build();
            
            foreach (var testCase in testCasesGenerator.Generate().Where(x => !x.IsValid))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.BroadcastTransactionAsync(Guid.NewGuid(), testCase.GetParameterValue<string>(signedTxData))
                );
            }
        }

        [TestMethod]
        public async Task BroadcastTransactionAsync__TransactionHasAlreadyBeenBroadcasted__ExceptionThrown()
        {
            var signedTxData = CreateValidHexString();
            
            var serviceBuilder = new TransactionServiceBuilder
            {
                
                OperationTransactions = new[]
                {
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.SignedTxData == signedTxData)
                }
                
            };

            var service = serviceBuilder.Build();
            
            await Assert.ThrowsExceptionAsync<ConflictException>
            (
                () => service.BroadcastTransactionAsync(Guid.NewGuid(), signedTxData)
            );
        }

        [TestMethod]
        public async Task BroadcastTransactionAsync__TransactionHasNotBeenBuiltForOperation__ExceptionThrown()
        {
            var txData = CreateValidHexString();

            var serviceBuilder = new TransactionServiceBuilder
            {
                
                OperationTransactions = Enum.GetValues(typeof(TransactionState))
                    .Cast<TransactionState>()
                    .Where(x => x != TransactionState.Built)
                    .Select(x => 
                        Mock.Of<ITransactionAggregate>(ctx => 
                            ctx.TxData == txData && ctx.State == x))
                    .ToArray(),
                
                UnsignTransactionResult = txData
                
            };

            var service = serviceBuilder.Build();
            
            await Assert.ThrowsExceptionAsync<BadRequestException>
            (
                () => service.BroadcastTransactionAsync(Guid.NewGuid(), CreateValidHexString())
            );
        }
        
        [TestMethod]
        public async Task BroadcastTransactionAsync__TransactionHasBeenSignedForDifferentSender__ExceptionThrown()
        {
            var txData = CreateValidHexString();
            
            var serviceBuilder = new TransactionServiceBuilder
            {
                GetTransactionSignerResult = CreateValidHexString(),
                
                OperationTransactions = new[]
                {
                    Mock.Of<ITransactionAggregate>(ctx => 
                        ctx.TxData == txData && ctx.State == TransactionState.Built && ctx.FromAddress == CreateValidHexString())
                },
                
                UnsignTransactionResult = txData
            };

            var service = serviceBuilder.Build();
            
            
            await Assert.ThrowsExceptionAsync<BadRequestException>
            (
                () => service.BroadcastTransactionAsync(Guid.NewGuid(), CreateValidHexString())
            );
        }
        
        [TestMethod]
        public async Task BroadcastTransactionAsync__ValidArgumentsPassed__AggregateUpdated_And_ValidTxHashReturned()
        {
            var fromAddress = CreateValidHexString();
            var signedTxData = CreateValidHexString();
            var signedTxHash = CreateValidHexString();
            var txData = CreateValidHexString();
            
            var operationTransaction = new Mock<ITransactionAggregate>();

            operationTransaction
                .SetupGet(x => x.TxData)
                .Returns(txData);
            
            operationTransaction
                .SetupGet(x => x.State)
                .Returns(TransactionState.Built);
            
            operationTransaction
                .SetupGet(x => x.FromAddress)
                .Returns(fromAddress);
            
            var serviceBuilder = new TransactionServiceBuilder
            {
                GetTransactionSignerResult = fromAddress,
                
                OperationTransactions = new[] { operationTransaction.Object },
                
                SendRawTransactionOrGetTxHashResult = signedTxHash,
                
                UnsignTransactionResult = txData
            };

            var service = serviceBuilder.Build();

            var actualResult = await service.BroadcastTransactionAsync(Guid.NewGuid(), signedTxData);
            
            serviceBuilder
                .WaitUntilTransactionIsInPoolStrategy
                .Verify(x => x.ExecuteAsync(signedTxHash), Times.Once);
            
            operationTransaction
                .Verify(x => x.OnBroadcasted(signedTxData, signedTxHash), Times.Once);
            
            serviceBuilder
                .TransactionRepository
                .Verify(x => x.UpdateAsync(operationTransaction.Object), Times.Once);
            
            Assert.AreEqual(signedTxHash, actualResult);
        }
        
        #endregion
        
        #region BuildTransactionAsync
        
        [TestMethod]
        public async Task BuildTransactionAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string amount = nameof(amount);
            const string fromAddress = nameof(fromAddress);
            const string toAddress = nameof(toAddress);
            
            var serviceBuilder = new TransactionServiceBuilder();
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterParameter(amount, new []
                {
                    (-1, false),
                    ( 0, false),
                    ( 1, true)
                })
                .RegisterAddressParameter(fromAddress)
                .RegisterAddressParameter(toAddress);

            var service = serviceBuilder.Build();
            
            foreach (var testCase in testCasesGenerator.Generate().Where(x => !x.IsValid))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.BuildTransactionAsync
                    (
                        testCase.GetParameterValue<int>(amount),
                        testCase.GetParameterValue<string>(fromAddress),
                        true,
                        Guid.NewGuid(),
                        testCase.GetParameterValue<string>(toAddress)
                    )
                );
            }
        }

        [TestMethod]
        public async Task BuildTransactionAsync__FromAddressAndToAddressAreEqual__ExceptionThrown()
        {
            var serviceBuilder = new TransactionServiceBuilder();
            
            var service = serviceBuilder.Build();
            
            await Assert.ThrowsExceptionAsync<ArgumentException>
            (
                () => service.BuildTransactionAsync
                (
                    1,
                    TestValues.ValidAddress1,
                    true,
                    Guid.NewGuid(),
                    TestValues.ValidAddress1
                )
            );
        }
        
        [TestMethod]
        public async Task BuildTransactionAsync__TransactionHasAlreadyBeenBuilt__InitialTransactionTxDataReturned()
        {
            var txData = CreateValidHexString();
            
            var serviceBuilder = new TransactionServiceBuilder
            {
                OperationTransactions = new[]
                {
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.TxData == CreateValidHexString() && ctx.BuiltOn == DateTime.UtcNow),
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.TxData == txData && ctx.BuiltOn == DateTime.UtcNow.AddDays(-1))
                }
            };


            var service = serviceBuilder.Build();

            var actualResult = await service.BuildTransactionAsync
            (
                1,
                TestValues.ValidAddress1,
                true,
                Guid.NewGuid(),
                TestValues.ValidAddress2
            );
            
            Assert.AreEqual(txData, actualResult);
        }

        [TestMethod]
        public async Task BuildTransactionAsync__TransactionHasNotBeenBuilt__TransactionRegistered_And_ValidTxDataReturned()
        {
            var operationId = Guid.NewGuid();
            var txData = CreateValidHexString();
            
            var serviceBuilder = new TransactionServiceBuilder
            {
                BuildTransactionResult = txData,
                CalculateTransactionParamsResult = (2, 3, 4),
                GetNextNonceResult = 5,
                OperationTransactions = new ITransactionAggregate[0]
            };
            
            var service = serviceBuilder.Build();

            var actualResult = await service.BuildTransactionAsync
            (
                1,
                TestValues.ValidAddress1,
                true,
                operationId,
                TestValues.ValidAddress2
            );
            
            serviceBuilder.RegisterTransactionStrategy
                .Verify(x => x.ExecuteAsync(2, 3, TestValues.ValidAddress1, 4, true, 5, operationId, TestValues.ValidAddress2, txData), Times.Once);
            
            Assert.AreEqual(txData, actualResult);
        }
        
        #endregion

        #region DeleteTransactionAsync

        [TestMethod]
        public async Task DeleteTransactionAsync__TransactionExists__Returns()
        {
            var serviceBuilder = new TransactionServiceBuilder();

            serviceBuilder.TransactionRepository
                .Setup(x => x.DeleteIfExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            
            var service = serviceBuilder.Build();

            await service.DeleteTransactionAsync(Guid.NewGuid());
        }
        
        [TestMethod]
        public async Task DeleteTransactionAsync__TransactionDoesNotExist__ExceptionThrown()
        {
            var serviceBuilder = new TransactionServiceBuilder();
            
            serviceBuilder
                .TransactionRepository
                .Setup(x => x.DeleteIfExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);
            
            var service = serviceBuilder.Build();
            
            await Assert.ThrowsExceptionAsync<NotFoundException>
            (
                () => service.DeleteTransactionAsync(Guid.NewGuid())
            );
        }
        
        #endregion
        
        #region GetTransactionAsync

        [TestMethod]
        public async Task GetTransactionAsync__TansactionDoesNotExist__ExceptionThrown()
        {
            var serviceBuilder = new TransactionServiceBuilder
            {
                OperationTransactions = new ITransactionAggregate[0]
            };
            
            var service = serviceBuilder.Build();
            
            await Assert.ThrowsExceptionAsync<NotFoundException>
            (
                () => service.GetTransactionAsync(Guid.NewGuid())
            );
        }
        
        [DataTestMethod]
        [DataRow(TransactionState.Completed)]
        [DataRow(TransactionState.Failed)]
        public async Task GetTransactionAsync__CompletedTansactionExists__ValidTransactionReturned(TransactionState state)
        {
            var completedTransaction = Mock.Of<ITransactionAggregate>(ctx =>
                ctx.State == state);
            
            var serviceBuilder = new TransactionServiceBuilder
            {
                OperationTransactions = new []
                {
                    completedTransaction,
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.State == TransactionState.InProgress),
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.State == TransactionState.Built)
                }
            };
            
            var service = serviceBuilder.Build();

            var actualResult = await service.GetTransactionAsync(Guid.NewGuid());
            
            Assert.AreEqual(completedTransaction, actualResult);
        }
        
        [TestMethod]
        public async Task GetTransactionAsync__MultipleCompletedTansactionsExist__ExceptionThrown()
        {
            var serviceBuilder = new TransactionServiceBuilder
            {
                OperationTransactions = new []
                {
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.State == TransactionState.Completed),
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.State == TransactionState.Failed),
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.State == TransactionState.Built),
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.State == TransactionState.InProgress)
                }
            };
            
            var service = serviceBuilder.Build();

            await Assert.ThrowsExceptionAsync<UnsupportedEdgeCaseException>
            (
                () => service.GetTransactionAsync(Guid.NewGuid())
            );
        }
        
        [TestMethod]
        public async Task GetTransactionAsync__InProgressTansactionsExist__LatestTransactionReturned()
        {
            var latestTransaction = Mock.Of<ITransactionAggregate>(ctx =>
                ctx.State == TransactionState.InProgress && ctx.BroadcastedOn == DateTime.UtcNow);
            
            var serviceBuilder = new TransactionServiceBuilder
            {
                OperationTransactions = new []
                {
                    latestTransaction,
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.State == TransactionState.InProgress && ctx.BroadcastedOn == DateTime.UtcNow.AddDays(-1)),
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.State == TransactionState.Built)
                }
            };
            
            var service = serviceBuilder.Build();

            var actualResult = await service.GetTransactionAsync(Guid.NewGuid());
            
            Assert.AreEqual(latestTransaction, actualResult);
        }
        
        [TestMethod]
        public async Task GetTransactionAsync__BuiltTansactionsExist__LatestTransactionReturned()
        {
            var latestTransaction = Mock.Of<ITransactionAggregate>(ctx =>
                ctx.State == TransactionState.Built && ctx.BuiltOn == DateTime.UtcNow);
            
            var serviceBuilder = new TransactionServiceBuilder
            {
                OperationTransactions = new []
                {
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.State == TransactionState.Built && ctx.BuiltOn == DateTime.UtcNow.AddDays(1)),
                    latestTransaction
                }
            };
            
            var service = serviceBuilder.Build();

            var actualResult = await service.GetTransactionAsync(Guid.NewGuid());
            
            Assert.AreEqual(latestTransaction, actualResult);
        }
        
        #endregion
        
        #region RebuildTransactionAsync

        [TestMethod]
        public async Task RebuildTransactionAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string feeFactor = nameof(feeFactor);
            
            var serviceBuilder = new TransactionServiceBuilder();
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterParameter(feeFactor, new []
                {
                    (1m,    false),
                    (0.99m, false),
                    (0,     false),
                    (-1m,   false),
                });

            var service = serviceBuilder.Build();
            
            foreach (var testCase in testCasesGenerator.Generate().Where(x => !x.IsValid))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.RebuildTransactionAsync
                    (
                        testCase.GetParameterValue<decimal>(feeFactor),
                        Guid.NewGuid()
                    )
                );
            }
        }

        [TestMethod]
        public async Task RebuildTransactionAsync__TransactionsDoNotExist__ExceptionThrown()
        {
            var serviceBuilder = new TransactionServiceBuilder
            {
                OperationTransactions = new ITransactionAggregate[0]
            };
            
            var service = serviceBuilder.Build();

            await Assert.ThrowsExceptionAsync<NotFoundException>
            (
                () => service.RebuildTransactionAsync(1.1m, Guid.NewGuid())                    
            );
        }

        [TestMethod]
        public async Task RebuildTransactionAsync__SameTransactionHasAlreadyBeenBuilt__ValidTxDataReturned()
        {
            var txData = CreateValidHexString();
            
            var serviceBuilder = new TransactionServiceBuilder
            {
                BuildTransactionResult = txData,
                CalculateTransactionParamsResult = (2, 3, 4),
                GetNextNonceResult = 5,
                OperationTransactions = new []
                {
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.TxData == txData)
                }
            };
            
            var service = serviceBuilder.Build();

            var actualResult = await service.RebuildTransactionAsync(1.1m, Guid.NewGuid());
            
            serviceBuilder.RegisterTransactionStrategy
                .Verify(x => x.ExecuteAsync(
                    It.IsAny<BigInteger>(),
                    It.IsAny<BigInteger>(),
                    It.IsAny<string>(),
                    It.IsAny<BigInteger>(),
                    It.IsAny<bool>(),
                    It.IsAny<BigInteger>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()), Times.Never);
            
            Assert.AreEqual(txData, actualResult);
        }
        
        [TestMethod]
        public async Task RebuildTransactionAsync__SameTransactionHasNotBeenBuilt__TransactionRegistered_And_ValidTxDataReturned()
        {
            var operationId = Guid.NewGuid();
            var txData = CreateValidHexString();
            
            var serviceBuilder = new TransactionServiceBuilder
            {
                BuildTransactionResult = txData,
                OperationTransactions = new []
                {
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.BuiltOn == DateTime.UtcNow &&
                        ctx.FromAddress == TestValues.ValidAddress1 &&
                        ctx.IncludeFee == true &&
                        ctx.OperationId == operationId &&
                        ctx.ToAddress == TestValues.ValidAddress2 &&
                        ctx.TxData == CreateValidHexString()
                    ),
                    Mock.Of<ITransactionAggregate>(ctx =>
                        ctx.BuiltOn == DateTime.UtcNow.AddDays(-1) &&
                        ctx.FromAddress == TestValues.ValidAddress3 &&
                        ctx.IncludeFee == true &&
                        ctx.OperationId == operationId &&
                        ctx.ToAddress == TestValues.ValidAddress4 &&
                        ctx.TxData == CreateValidHexString()
                    )
                }
            };
            
            var service = serviceBuilder.Build();

            var actualResult = await service.RebuildTransactionAsync(1.1m, operationId);
            
            serviceBuilder.RegisterTransactionStrategy
                .Verify(x => x.ExecuteAsync(2, 3, TestValues.ValidAddress3, 4, true, 5, operationId, TestValues.ValidAddress4, txData), Times.Never);
            
            Assert.AreEqual(txData, actualResult);
        }
        
        #endregion
        
        private static string CreateValidHexString()
        {
            return $"0x{Guid.NewGuid():N}";
        }
        
        [PublicAPI]
        private class TransactionServiceBuilder
        {
            private string _buildTransactionResult;
            private (BigInteger, BigInteger, BigInteger) _calculateTransactionParamsResult;
            private BigInteger _getNextNonceResult;
            private string _getTransactionSignerResult;
            private string _sendRawTransactionOrGetTxHashResult;
            private IReadOnlyList<ITransactionAggregate> _transactions;
            private string _unsignTransactionResult;
            
            
            public TransactionServiceBuilder()
            {
                BlockchainService = new Mock<IBlockchainService>();
                CalculateTransactionParamsStrategy = new Mock<ICalculateTransactionParamsStrategy>();
                RegisterTransactionStrategy = new Mock<IRegisterTransactionStrategy>();
                SendRawTransactionOrGetTxHashStrategy = new Mock<ISendRawTransactionOrGetTxHashStrategy>();
                TransactionRepository = new Mock<ITransactionRepository>();
                WaitUntilTransactionIsInPoolStrategy = new Mock<IWaitUntilTransactionIsInPoolStrategy>();
            }

            
            public Mock<IBlockchainService> BlockchainService { get; }

            public Mock<ICalculateTransactionParamsStrategy> CalculateTransactionParamsStrategy { get; }
            
            public Mock<IRegisterTransactionStrategy> RegisterTransactionStrategy { get; }
            
            public Mock<ISendRawTransactionOrGetTxHashStrategy> SendRawTransactionOrGetTxHashStrategy { get; }
            
            public Mock<ITransactionRepository> TransactionRepository { get; }
            
            public Mock<IWaitUntilTransactionIsInPoolStrategy> WaitUntilTransactionIsInPoolStrategy { get; }



            public string BuildTransactionResult
            {
                get => _buildTransactionResult;
                set
                {
                    _buildTransactionResult = value;
                    
                    BlockchainService
                        .Setup(x => x.BuildTransaction
                        (
                            It.IsAny<string>(),
                            It.IsAny<BigInteger>(),
                            It.IsAny<BigInteger>(),
                            It.IsAny<BigInteger>(),
                            It.IsAny<BigInteger>()
                        ))
                        .Returns(value);
                }
            }
            
            public (BigInteger, BigInteger, BigInteger) CalculateTransactionParamsResult
            {
                get => _calculateTransactionParamsResult;
                set
                {
                    _calculateTransactionParamsResult = value;
                    
                    CalculateTransactionParamsStrategy
                        .Setup(x => x.ExecuteAsync(It.IsAny<BigInteger>(), It.IsAny<bool>(), It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }

            public BigInteger GetNextNonceResult
            {
                get => _getNextNonceResult;
                set
                {
                    _getNextNonceResult = value;
                    
                    BlockchainService
                        .Setup(x => x.GetNextNonceAsync(It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }
            
            public string GetTransactionSignerResult
            {
                get => _getTransactionSignerResult;
                set
                {
                    _getTransactionSignerResult = value;

                    BlockchainService
                        .Setup(x => x.GetTransactionSigner(It.IsAny<string>()))
                        .Returns(value);
                }
            }
            
            public IReadOnlyList<ITransactionAggregate> OperationTransactions
            {
                get => _transactions;
                set
                {
                    _transactions = value;
                    
                    TransactionRepository
                        .Setup(x => x.GetAllForOperationAsync(It.IsAny<Guid>()))
                        .ReturnsAsync(value);
                }
            }

            public string SendRawTransactionOrGetTxHashResult
            {
                get => _sendRawTransactionOrGetTxHashResult;
                set
                {
                    _sendRawTransactionOrGetTxHashResult = value;
                    
                    SendRawTransactionOrGetTxHashStrategy
                        .Setup(x => x.ExecuteAsync(It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }
            
            public string UnsignTransactionResult
            {
                get => _unsignTransactionResult;
                set
                {
                    _unsignTransactionResult = value;

                    BlockchainService
                        .Setup(x => x.UnsignTransactionAsync(It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }


            public TransactionService Build()
            {
                return new TransactionService
                (
                    BlockchainService.Object,
                    CalculateTransactionParamsStrategy.Object,
                    RegisterTransactionStrategy.Object,
                    SendRawTransactionOrGetTxHashStrategy.Object,
                    new ApiSettings { GasAmount = "21000" },
                    TransactionRepository.Object,
                    WaitUntilTransactionIsInPoolStrategy.Object,
                    new Mock<IChaosKitty>(MockBehavior.Loose).Object
                );
            }
        }
        
    }
    
}
