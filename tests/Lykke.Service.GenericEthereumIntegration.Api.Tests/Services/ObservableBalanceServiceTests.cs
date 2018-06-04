using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
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
    public class ObservableBalanceServiceTests
    {
        [TestMethod]
        public async Task BeginObservationAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string address = nameof(address);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(address);
            
            var serviceBuilder = new ObservableBalanceServiceBuilder();
            
            var service = serviceBuilder.Build();

            foreach (var testCase in testCasesGenerator.Generate().Where(x => !x.IsValid))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.BeginObservationAsync
                    (
                        address: testCase.GetParameterValue<string>(address)
                    )
                );   
            }
        }
        
        [TestMethod]
        public async Task BeginObservationAsync__AddressIsAlreadyObserved__ExceptionThrown()
        {
            var serviceBuilder = new ObservableBalanceServiceBuilder
            {
                TryAddResult = false
            };


            var service = serviceBuilder.Build();

            await Assert.ThrowsExceptionAsync<ConflictException>
            (
                () => service.BeginObservationAsync(TestValues.ValidAddress1)
            );
        }
        
        [TestMethod]
        public async Task BeginObservationAsync__AddressAddedToObservationList__Returned()
        {
            var serviceBuilder = new ObservableBalanceServiceBuilder
            {
                TryAddResult = true
            };


            var service = serviceBuilder.Build();

            await service.BeginObservationAsync(TestValues.ValidAddress1);
        }
        
        [TestMethod]
        public async Task EndObservationAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string address = nameof(address);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(address);
            
            var serviceBuilder = new ObservableBalanceServiceBuilder();
            
            var service = serviceBuilder.Build();

            foreach (var testCase in testCasesGenerator.Generate().Where(x => !x.IsValid))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.EndObservationAsync
                    (
                        address: testCase.GetParameterValue<string>(address)
                    )
                );   
            }
        }
        
        [TestMethod]
        public async Task EndObservationAsync__AddressDoesNotExistInObservationList__ExceptionThrown()
        {
            var serviceBuilder = new ObservableBalanceServiceBuilder
            {
                DeleteIfExistsResult = false
            };


            var service = serviceBuilder.Build();

            await Assert.ThrowsExceptionAsync<NotFoundException>
            (
                () => service.EndObservationAsync(TestValues.ValidAddress1)
            );
        }
        
        [TestMethod]
        public async Task BeginObservationAsync__AddressRemovedFromObservationList__Returned()
        {
            var serviceBuilder = new ObservableBalanceServiceBuilder
            {
                DeleteIfExistsResult = true
            };


            var service = serviceBuilder.Build();

            await service.EndObservationAsync(TestValues.ValidAddress1);
        }

        [TestMethod]
        public async Task GetBalancesAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string take = nameof(take);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterParameter(take, new[]
                {
                    (-1, false),
                    (0, false),
                    (1, true)
                });
            
            var serviceBuilder = new ObservableBalanceServiceBuilder();
            
            var service = serviceBuilder.Build();

            foreach (var testCase in testCasesGenerator.Generate().Where(x => !x.IsValid))
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.GetBalancesAsync
                    (
                        take: testCase.GetParameterValue<int>(take),
                        continuationToken: null
                    )
                );
            }
        }

        [TestMethod]
        public async Task GetBalancesAsync__ValidArgumentsPassed__ValidBalancesReturned()
        {
            var assetId = $"{Guid.NewGuid()}";
            var observableBalances = new ObservableBalanceDto[0];
            var continuationToken = $"{Guid.NewGuid()}";

            var serviceBuilder = new ObservableBalanceServiceBuilder
            {
                GetAssetIdResult = assetId,
                GetAllWithNonZeroAmountResult = (observableBalances, continuationToken)
            };


            var service = serviceBuilder.Build();

            var actualResult = await service.GetBalancesAsync(1, null);
            
            Assert.AreEqual(assetId, actualResult.AssetId);
            Assert.AreEqual(observableBalances, actualResult.Balances);
            Assert.AreEqual(continuationToken, actualResult.ContinuationToken);
        }


        
        private class ObservableBalanceServiceBuilder
        {
            private bool _deleteIfExistsResult;
            private (IEnumerable<ObservableBalanceDto>, string) _getAllWithNonZeroAmountResult;
            private bool _tryAddResult;


            public ObservableBalanceServiceBuilder()
            {
                AssetSettings = new AssetSettings();
                ObservableBalanceRepository = new Mock<IObservableBalanceRepository>();
            }
            
            
            public AssetSettings AssetSettings { get; }
            
            public Mock<IObservableBalanceRepository> ObservableBalanceRepository { get; }
            
            


            public bool DeleteIfExistsResult
            {
                get => _deleteIfExistsResult;
                set
                {
                    _deleteIfExistsResult = value;

                    ObservableBalanceRepository
                        .Setup(x => x.DeleteIfExistsAsync(It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }

            public (IEnumerable<ObservableBalanceDto>, string) GetAllWithNonZeroAmountResult
            {
                get => _getAllWithNonZeroAmountResult;
                set
                {
                    _getAllWithNonZeroAmountResult = value;

                    ObservableBalanceRepository
                        .Setup(x => x.GetAllWithNonZeroAmountAsync(It.IsAny<int>(), It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }

            public string GetAssetIdResult
            {
                get => AssetSettings.Id;
                set => AssetSettings.Id = value;
            }

            public bool TryAddResult
            {
                get => _tryAddResult;
                set
                {
                    _tryAddResult = value;

                    ObservableBalanceRepository
                        .Setup(x => x.TryAddAsync(It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }

            public ObservableBalanceService Build()
            {
                return new ObservableBalanceService
                (
                    AssetSettings,
                    ObservableBalanceRepository.Object
                );
            }
        }
    }
}
