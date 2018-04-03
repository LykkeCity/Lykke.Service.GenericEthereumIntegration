using System;
using System.Globalization;
using System.Numerics;
using Lykke.Service.GenericEthereumIntegration.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services
{
    [TestClass]
    public class TransactionServiceTests
    {
        private const int GasAmount = 21000;

        [DataTestMethod]
        [DataRow("1", "1.1", "42000")]
        [DataRow("10", "1.1", "231000")]
        public void Apply__ExpectedResultReturned(string gasPriceString, string feeFactorString, string expectedResultString)
        {
            var gasPrice = BigInteger.Parse(gasPriceString);
            var feeFactor = decimal.Parse(feeFactorString, CultureInfo.InvariantCulture);
            var expectedResult = BigInteger.Parse(expectedResultString);
            
            var actualResult = TransactionService.CalculateFeeWithFeeFactor(gasPrice, GasAmount, feeFactor);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Apply__GasPrice_Lower_Then_One_Passed__ArgumentAxceptionThrown()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var gasPrice = new BigInteger(0);
                var feeFactor = 1.1m;

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                TransactionService.CalculateFeeWithFeeFactor(gasPrice, GasAmount, feeFactor);
            });
        }

        [TestMethod]
        public void Apply__FeeFactor_Lower_Then_One_Passed__ArgumentAxceptionThrown()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var gasPrice = new BigInteger(1);
                var feeFactor = 0.9m;

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                TransactionService.CalculateFeeWithFeeFactor(gasPrice, GasAmount, feeFactor);
            });
        }
    }
}
