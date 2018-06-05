using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Services;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration;
using Lykke.Service.GenericEthereumIntegration.TDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services
{
    [TestClass]
    public class HistoricalTransactionServiceTests
    {
        [TestMethod]
        public async Task GetIncomingHistoryAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string address = nameof(address);
            const string take = nameof(take);

            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(address)
                .RegisterParameter(take, new[]
                {
                    (-1, false),
                    (0, false),
                    (1, false),
                    (2, true)
                });
            
            var serviceBuilder = new HistoricalTransactionServiceBuilder();

            var service = serviceBuilder.Build();
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.GetIncomingHistoryAsync
                    (
                        address: testCase.GetParameterValue<string>(address),
                        take: testCase.GetParameterValue<int>(take),
                        afterHash: null
                    )
                );
            }
        }
        
        [TestMethod]
        public async Task GetIncomingHistoryAsync__ValidArgumentsPassed__ValidDataReturned()
        {
            var serviceBuilder = new HistoricalTransactionServiceBuilder
            {
                AssetId = $"{Guid.NewGuid()}",
                GetIncomingHistoryResult = new HistoricalTransactionDto[0]
            };

            var service = serviceBuilder.Build();

            var actualResult = await service.GetIncomingHistoryAsync(TestValues.ValidAddress1, 2, null);
            
            Assert.AreEqual(serviceBuilder.AssetId, actualResult.AssetId);
            Assert.AreEqual(serviceBuilder.GetIncomingHistoryResult, actualResult.Transactions);
        }
        
        [TestMethod]
        public async Task GetOutgoingHistoryAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string address = nameof(address);
            const string take = nameof(take);

            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(address)
                .RegisterParameter(take, new[]
                {
                    (-1, false),
                    (0, false),
                    (1, false),
                    (2, true)
                });
            
            var serviceBuilder = new HistoricalTransactionServiceBuilder();

            var service = serviceBuilder.Build();
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.GetOutgoingHistoryAsync
                    (
                        address: testCase.GetParameterValue<string>(address),
                        take: testCase.GetParameterValue<int>(take),
                        afterHash: null
                    )
                );
            }
        }
        
        [TestMethod]
        public async Task GetOutgoingHistoryAsync__ValidArgumentsPassed__ValidDataReturned()
        {
            var serviceBuilder = new HistoricalTransactionServiceBuilder
            {
                AssetId = $"{Guid.NewGuid()}",
                GetOutgoingHistoryResult = new HistoricalTransactionDto[0]
            };

            var service = serviceBuilder.Build();

            var actualResult = await service.GetOutgoingHistoryAsync(TestValues.ValidAddress1, 2, null);
            
            Assert.AreEqual(serviceBuilder.AssetId, actualResult.AssetId);
            Assert.AreEqual(serviceBuilder.GetOutgoingHistoryResult, actualResult.Transactions);
        }

        [PublicAPI]
        private class HistoricalTransactionServiceBuilder
        {
            private IEnumerable<HistoricalTransactionDto> _getIncomingHistoryResult;
            private IEnumerable<HistoricalTransactionDto> _getOutgoingHistoryResult;
            

            public HistoricalTransactionServiceBuilder()
            {
                AssetSettings = new AssetSettings();
                HistoricalTransactionRepository = new Mock<IHistoricalTransactionRepository>();
            }

            
            public AssetSettings AssetSettings { get; }
            
            public Mock<IHistoricalTransactionRepository> HistoricalTransactionRepository { get; }


            public string AssetId
            {
                get => AssetSettings.Id;
                set => AssetSettings.Id = value;
            }
            
            public IEnumerable<HistoricalTransactionDto> GetIncomingHistoryResult
            {
                get => _getIncomingHistoryResult;
                set
                {
                    _getIncomingHistoryResult = value;

                    HistoricalTransactionRepository
                        .Setup(x => x.GetIncomingHistoryAsync
                        (
                            It.IsAny<string>(),
                            It.IsAny<int>(),
                            It.IsAny<string>()
                        ))
                        .ReturnsAsync(value);
                }
            }

            public IEnumerable<HistoricalTransactionDto> GetOutgoingHistoryResult
            {
                get => _getOutgoingHistoryResult;
                set
                {
                    _getOutgoingHistoryResult = value;
                    
                    HistoricalTransactionRepository
                        .Setup(x => x.GetOutgoingHistoryAsync
                        (
                            It.IsAny<string>(),
                            It.IsAny<int>(),
                            It.IsAny<string>()
                        ))
                        .ReturnsAsync(value);
                }
            }


            public HistoricalTransactionService Build()
            {
                return new HistoricalTransactionService
                (
                    AssetSettings,
                    HistoricalTransactionRepository.Object
                );
            }
        }
    }
}
