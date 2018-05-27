using System;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings.Service;
using Lykke.Service.GenericEthereumIntegration.Api.Services;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services
{
    [TestClass]
    public class TransactionServiceTests
    {
        private const int GasAmount = 21000;


        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("0xA")]
        [DataRow("0xZe")]
        public async Task BroadcastTransactionAsync__SignedTxDataIsInvalid__ExceptionThrown(string signedTxData)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            var transactionService = new TransactionService
            (
                null,
                null,
                new ApiSettings { GasAmount = "1" }, 
                null,
                null
            );
            // ReSharper restore AssignNullToNotNullAttribute

            await Assert.ThrowsExceptionAsync<ArgumentException>
            (
                () => transactionService.BroadcastTransactionAsync(Guid.NewGuid(), signedTxData)
            );
        }
            
        
        
        [DataTestMethod]
        [DataRow("1", "1.1", "42000")]
        [DataRow("10", "1.1", "231000")]
        public void CalculateFeeWithFeeFactor__ValidResultReturned(string gasPriceString, string feeFactorString, string expectedResultString)
        {
            var gasPrice = BigInteger.Parse(gasPriceString);
            var feeFactor = decimal.Parse(feeFactorString, CultureInfo.InvariantCulture);
            var expectedResult = BigInteger.Parse(expectedResultString);
            
            var actualResult = TransactionService.CalculateFeeWithFeeFactor(gasPrice, GasAmount, feeFactor);

            Assert.AreEqual(expectedResult, actualResult);
        }

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
                
            
            // ReSharper disable AssignNullToNotNullAttribute
            var transactionService = new TransactionService
            (
                blockchainService.Object,
                null,
                new ApiSettings { GasAmount = "1" }, 
                null,
                null
            );
            // ReSharper restore AssignNullToNotNullAttribute

            var actualResult = await transactionService.SendRawTransactionOrGetTxHashAsync(It.IsAny<string>());
            
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
        
        [DataTestMethod]
        [DataRow(true,  false, 1, 0, false)]
        [DataRow(true,  true,  1, 0, false)]
        [DataRow(false, true,  1, 1, false)]
        [DataRow(false, false, 5, 5, true )]
        public async Task WaitUntilTransactionIsInPoolAsync__Returned_Or_EctceptionThrown(
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
            
            // ReSharper disable AssignNullToNotNullAttribute
            var transactionService = new TransactionService
            (
                blockchainService.Object,
                null,
                new ApiSettings { GasAmount = "1" }, 
                null,
                null
            );
            // ReSharper restore AssignNullToNotNullAttribute

            if (!exceptionThrown)
            {
                await transactionService.WaitUntilTransactionIsInPoolAsync(It.IsAny<string>(), 1);
            }
            else
            {
                await Assert.ThrowsExceptionAsync<UnsupportedEdgeCaseException>
                (
                    () => transactionService.WaitUntilTransactionIsInPoolAsync(It.IsAny<string>(), 1)
                );
            }
            
            Assert.AreEqual(expectedCheckIfBroadcastedAsyncCallsCount, actualCheckIfBroadcastedAsyncCallsCount);
            Assert.AreEqual(expectedTryGetTransactionReceiptAsyncCallsCount, actualTryGetTransactionReceiptAsyncCallsCount);
        }
        
        [DataTestMethod]
        [DataRow(true, true, 5, 0)]
        [DataRow(true, false, 5, 0)]
        [DataRow(false, true, 5, 5)]
        public async Task WaitUntilTransactionIsInPoolAsync__BlockchainService_Throws_Exception__ExceptionThrown(
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

            
            // ReSharper disable AssignNullToNotNullAttribute
            var transactionService = new TransactionService
            (
                blockchainService.Object,
                null,
                new ApiSettings { GasAmount = "1" }, 
                null,
                null
            );
            // ReSharper restore AssignNullToNotNullAttribute

            await Assert.ThrowsExceptionAsync<UnsupportedEdgeCaseException>
            (
                () => transactionService.WaitUntilTransactionIsInPoolAsync(It.IsAny<string>(), 1)
            );
            
            Assert.AreEqual(expectedCheckIfBroadcastedAsyncCallsCount, actualCheckIfBroadcastedAsyncCallsCount);
            Assert.AreEqual(expectedTryGetTransactionReceiptAsyncCallsCount, actualTryGetTransactionReceiptAsyncCallsCount);
        }
    }
}
