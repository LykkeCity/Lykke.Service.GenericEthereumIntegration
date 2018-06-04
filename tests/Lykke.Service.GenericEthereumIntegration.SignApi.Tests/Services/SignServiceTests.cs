using System;
using System.Linq;
using Lykke.Service.GenericEthereumIntegration.SignApi.Services;
using Lykke.Service.GenericEthereumIntegration.TDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Tests.Services
{
    [TestClass]
    public class SignServiceTests
    {
        [TestMethod]
        public void SignTransaction__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string transactionHex = nameof(transactionHex);
            const string privateKey = nameof(privateKey);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterHexStringParameter(transactionHex);

            testCasesGenerator
                .RegisterHexStringParameter(privateKey);

            var service = new SignService();
            
            foreach (var testCase in testCasesGenerator.Generate().Where(x => !x.IsValid))
            {
                Assert.ThrowsException<ArgumentException>
                (
                    () => service.SignTransaction
                    (
                        testCase.GetParameterValue<string>(transactionHex),
                        testCase.GetParameterValue<string>(privateKey)
                    )
                );
            }
        }

        [TestMethod]
        public void SignTransaction__ValidArgumentsPassed__ValidTxDataReturned()
        {
            const string transactionHex = "0x95c40102c40102c40102c40100d92a307845413637346664446537313466643937396465334564463046353641413937313642383938656338";
            const string privateKey = "0x008d62016b2d3fc7353785736df6ec1ae90a5953be4402e59b98352b80189583";
            const string expectedResult = "0xf85d80020294ea674fdde714fd979de3edf0f56aa9716b898ec802801ca043d278a942c00b03c700c49d72d4f156fa672f9bf00f133393573236b7f5d008a00da8f47050ddc2d83dc858be2e2497e214fb05681d0f90963b21a30a85abb919";
            
            var service = new SignService();

            var actualResult = service.SignTransaction(transactionHex, privateKey);
            
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
