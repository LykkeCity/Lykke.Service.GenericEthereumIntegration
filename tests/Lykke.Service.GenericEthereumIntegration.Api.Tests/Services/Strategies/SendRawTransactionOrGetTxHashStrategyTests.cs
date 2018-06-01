using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services.Strategies
{
    [TestClass]
    public class SendRawTransactionOrGetTxHashStrategyTests
    {
        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task SendRawTransactionOrGetTxHashAsync__ValidHashReturned(bool transactionHasBeenSent)
        {
            const string expectedResult = "0xa6fe420e1dff51d3453724404e69aecb8332cac796156708c76b152fc486c2eb";
            
            var blockchainService = new Mock<IBlockchainService>();

            blockchainService
                .Setup(x => x.GetTransactionHash(It.IsAny<string>()))
                .Returns(expectedResult);

            blockchainService
                .Setup(x => x.TryGetTransactionReceiptAsync(It.IsAny<string>()))
                .ReturnsAsync(transactionHasBeenSent ? new TransactionReceiptDto() : null);

            var sendRawTransactionAsyncCalled = false;

            blockchainService
                .Setup(x => x.SendRawTransactionAsync(It.IsAny<string>()))
                .Callback(() => sendRawTransactionAsyncCalled = true)
                .ReturnsAsync(It.IsAny<string>());


            var strategy = new SendRawTransactionOrGetTxHashStrategy(blockchainService.Object);
            
            var actualResult = await strategy.ExecuteAsync(It.IsAny<string>());
            
            Assert.AreEqual(expectedResult, actualResult);

            if (transactionHasBeenSent)
            {
                Assert.IsFalse(sendRawTransactionAsyncCalled);
            }
            else
            {
                Assert.IsTrue(sendRawTransactionAsyncCalled);
            }
        }
    }
}
