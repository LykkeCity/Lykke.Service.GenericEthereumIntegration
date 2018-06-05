using System;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Api.Services;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.TDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services
{
    [TestClass]
    public class GasPriceOracleServiceTest
    {
        private const string MinGasPrice = "20000000000";
        private const string MaxGasPrice = "80000000000";


        [TestMethod]
        public async Task CalculateGasPriceAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string to = nameof(to);
            const string amount = nameof(amount);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(to)
                .RegisterParameter(amount, new[]
                {
                    (-1, false),
                    (0, false),
                    (1, true)
                });

            var serviceBuilder = new GasPriceOracleServiceBuilder
            {
                DefultMaxGasPrice = MaxGasPrice,
                DefaultMinGasPrice = MinGasPrice
            };
            
            var service = serviceBuilder.Build();

            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.CalculateGasPriceAsync
                    (
                        to: testCase.GetParameterValue<string>(to),
                        amount: testCase.GetParameterValue<int>(amount)
                    )
                );
            }
        }
        

        [DataTestMethod]
        [DataRow("10000000000", MinGasPrice)]
        [DataRow("70000000000", "70000000000")]
        [DataRow("90000000000", MaxGasPrice)]

        public async Task CalculateGasPriceAsync__ValidArgumentsPassed__ValidResultReturned(
            string estimatedGasPrice,
            string expectedResult)
        {

            var serviceBuilder = new GasPriceOracleServiceBuilder
            {
                DefultMaxGasPrice = MaxGasPrice,
                DefaultMinGasPrice = MinGasPrice,
                EstimateGasPriceResult = BigInteger.Parse(estimatedGasPrice),
                GetOrAddDefaultGaspriceResult = (BigInteger.Parse(MinGasPrice), BigInteger.Parse(MaxGasPrice))
            };

            var service = serviceBuilder.Build();
            
            var actualResult = await service.CalculateGasPriceAsync
            (
                to: TestValues.ValidAddress1,
                amount: 1000000000
            );
            
            Assert.AreEqual(BigInteger.Parse(expectedResult), actualResult);
        }


        [PublicAPI]
        private class GasPriceOracleServiceBuilder
        {
            private BigInteger _estimateGasPriceResult;
            private (BigInteger, BigInteger) _getOrAddDefaultGaspriceResult;
            
            
            public GasPriceOracleServiceBuilder()
            {
                BlockchainService = new Mock<IBlockchainService>();
                GasPriceRepository = new Mock<IGasPriceRepository>();
                ApiSettings = new ApiSettings();
            }
            
            
            public Mock<IBlockchainService> BlockchainService { get; }
            
            public Mock<IGasPriceRepository> GasPriceRepository { get; }
            
            public ApiSettings ApiSettings { get; }


            public string DefultMaxGasPrice
            {
                get => ApiSettings.DefaultMaxGasPrice;
                set => ApiSettings.DefaultMaxGasPrice = value;
            }

            public string DefaultMinGasPrice
            {
                get => ApiSettings.DefaultMinGasPrice;
                set => ApiSettings.DefaultMinGasPrice = value;
            }

            public BigInteger EstimateGasPriceResult
            {
                get => _estimateGasPriceResult;
                set
                {
                    _estimateGasPriceResult = value;
                    
                    BlockchainService
                        .Setup(x => x.EstimateGasPriceAsync(It.IsAny<string>(), It.IsAny<BigInteger>()))
                        .ReturnsAsync(value);
                }
            }
            
            public (BigInteger, BigInteger) GetOrAddDefaultGaspriceResult
            {
                get => _getOrAddDefaultGaspriceResult;
                set
                {
                    _getOrAddDefaultGaspriceResult = value;

                    GasPriceRepository
                        .Setup(x => x.GetOrAddAsync(It.IsAny<BigInteger>(), It.IsAny<BigInteger>()))
                        .ReturnsAsync(value);
                }
            }
            

            public GasPriceOracleService Build()
            {
                return new GasPriceOracleService
                (
                    BlockchainService.Object,
                    GasPriceRepository.Object,
                    ApiSettings
                );
            }
        }
    }
}
