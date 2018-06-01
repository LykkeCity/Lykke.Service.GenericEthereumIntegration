using System;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Strategies;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services.Strategies
{
    [TestClass]
    public class WaitUntilTransactionIsInPoolStrategyTests
    {
        [DataTestMethod]
        [DataRow(true,  false, 1, 0, false)]
        [DataRow(true,  true,  1, 0, false)]
        [DataRow(false, true,  1, 1, false)]
        [DataRow(false, false, 5, 5, true )]
        public async Task ExecuteAsync__Returned_Or_EctceptionThrown(
            bool txBroadcasted,
            bool txMined,
            int expectedCheckIfBroadcastedAsyncCallsCount,
            int expectedTryGetTransactionReceiptAsyncCallsCount,
            bool exceptionThrown)
        {
            var blockchainService = new Mock<IBlockchainService>();

            var actualCheckIfBroadcastedAsyncCallsCount = 0;
            var actualTryGetTransactionReceiptAsyncCallsCount = 0;


            blockchainService
                .Setup(x => x.CheckIfBroadcastedAsync(It.IsAny<string>()))
                .Callback(() => actualCheckIfBroadcastedAsyncCallsCount++)
                .ReturnsAsync(txBroadcasted);
                
            blockchainService
                .Setup(x => x.TryGetTransactionReceiptAsync(It.IsAny<string>()))
                .Callback(() => actualTryGetTransactionReceiptAsyncCallsCount++)
                .ReturnsAsync(txMined ? new TransactionReceiptDto() : null);
            
            var strategy = new WaitUntilTransactionIsInPoolStrategy(blockchainService.Object, 1);

            if (!exceptionThrown)
            {
                await strategy.ExecuteAsync(It.IsAny<string>());
            }
            else
            {
                await Assert.ThrowsExceptionAsync<UnsupportedEdgeCaseException>
                (
                    () => strategy.ExecuteAsync(It.IsAny<string>())
                );
            }
            
            Assert.AreEqual(expectedCheckIfBroadcastedAsyncCallsCount, actualCheckIfBroadcastedAsyncCallsCount);
            Assert.AreEqual(expectedTryGetTransactionReceiptAsyncCallsCount, actualTryGetTransactionReceiptAsyncCallsCount);
        }
        
        [DataTestMethod]
        [DataRow(true, true, 5, 0)]
        [DataRow(true, false, 5, 0)]
        [DataRow(false, true, 5, 5)]
        public async Task ExecuteAsync__BlockchainService_Throws_Exception__ExceptionThrown(
            bool checkIfBroadcastedAsyncThrows,
            bool tryGetTransactionReceiptAsyncThrows,
            int expectedCheckIfBroadcastedAsyncCallsCount,
            int expectedTryGetTransactionReceiptAsyncCallsCount)
        {
            var blockchainService = new Mock<IBlockchainService>();

            var actualCheckIfBroadcastedAsyncCallsCount = 0;
            var actualTryGetTransactionReceiptAsyncCallsCount = 0;


            var checkIfBroadcastedAsync = blockchainService
                .Setup(x => x.CheckIfBroadcastedAsync(It.IsAny<string>()))
                .Callback(() => actualCheckIfBroadcastedAsyncCallsCount++); 
            
            if (checkIfBroadcastedAsyncThrows)
            {
                checkIfBroadcastedAsync
                    .Throws<Exception>();
            }
            else
            {
                checkIfBroadcastedAsync
                    .ReturnsAsync(false);
            }

            
            var tryGetTransactionReceiptAsync = blockchainService
                .Setup(x => x.TryGetTransactionReceiptAsync(It.IsAny<string>()))
                .Callback(() => actualTryGetTransactionReceiptAsyncCallsCount++); 
            
            if (tryGetTransactionReceiptAsyncThrows)
            {
                tryGetTransactionReceiptAsync
                    .Throws<Exception>();
            }
            else
            {
                tryGetTransactionReceiptAsync
                    .ReturnsAsync(new TransactionReceiptDto());
            }

            
            var strategy = new WaitUntilTransactionIsInPoolStrategy(blockchainService.Object, 1);

            await Assert.ThrowsExceptionAsync<UnsupportedEdgeCaseException>
            (
                () => strategy.ExecuteAsync(It.IsAny<string>())
            );
            
            Assert.AreEqual(expectedCheckIfBroadcastedAsyncCallsCount, actualCheckIfBroadcastedAsyncCallsCount);
            Assert.AreEqual(expectedTryGetTransactionReceiptAsyncCallsCount, actualTryGetTransactionReceiptAsyncCallsCount);
        }
    }
}
