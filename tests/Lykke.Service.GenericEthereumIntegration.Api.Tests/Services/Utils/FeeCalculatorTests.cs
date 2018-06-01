using System.Globalization;
using System.Numerics;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services.Utils
{
    [TestClass]
    public class FeeCalculatorTests
    {
        [DataTestMethod]
        [DataRow("1", "1.1", "42000")]
        [DataRow("10", "1.1", "231000")]
        public void CalculateFeeWithFeeFactor__ValidResultReturned(string gasPriceString, string feeFactorString, string expectedResultString)
        {
            var gasPrice = BigInteger.Parse(gasPriceString);
            var feeFactor = decimal.Parse(feeFactorString, CultureInfo.InvariantCulture);
            var expectedResult = BigInteger.Parse(expectedResultString);
            
            var actualResult = FeeCalculator.Calculate(gasPrice, 21000, feeFactor);

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
