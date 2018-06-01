using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Api.Services;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services
{
    [TestClass]
    public class GasPriceOracleServiceTest
    {
        private const string MinGasPrice = "20000000000";
        private const string MaxGasPrice = "80000000000";



        [DataTestMethod]
        [DataRow("10000000000", MinGasPrice)]
        [DataRow("70000000000", "70000000000")]
        [DataRow("90000000000", MaxGasPrice)]

        public async Task CalculateGasPriceAsync__ValidResultReturned(
            string estimatedGasPrice,
            string expectedResult)
        {
            var blockchainService = new Mock<IBlockchainService>();
            
            blockchainService
                .Setup(x => x.EstimateGasPriceAsync(It.IsAny<string>(), It.IsAny<BigInteger>()))
                .ReturnsAsync(BigInteger.Parse(estimatedGasPrice));

            
            var gasPriceRepository = new Mock<IGasPriceRepository>();
            
            gasPriceRepository
                .Setup(x => x.GetOrAddAsync(It.IsAny<BigInteger>(), It.IsAny<BigInteger>()))
                .ReturnsAsync((BigInteger.Parse(MinGasPrice), BigInteger.Parse(MaxGasPrice)));


            var settings = new ApiSettings
            {
                DefaultMaxGasPrice = MaxGasPrice,
                DefaultMinGasPrice = MinGasPrice
            };

            var gasPriceOracleService = new GasPriceOracleService
            (
                blockchainService.Object,
                gasPriceRepository.Object,
                settings
            );
            
            var actualResult = await gasPriceOracleService.CalculateGasPriceAsync
            (
                to: "0x83F0726180Cf3964b69f62AC063C5Cb9A66B3bE5",
                amount: 1000000000
            );
            

            Assert.AreEqual(BigInteger.Parse(expectedResult), actualResult);
        }
    }
}
