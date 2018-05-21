using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.Web3;


namespace Lykke.Service.GenericEthereumIntegration.Common.Tests.Services
{
    [TestClass]
    public class BlockchainServiceBaseTests
    {
        private const string InvalidAddress = "0xea674fdDe714fd979de3EdF0F56AA9716B898ec8";
        private const string ValidAddress = "0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8";

        
        private readonly BlockchainService _blockchainService;
        
        
        public BlockchainServiceBaseTests()
        {
            var rpcHostUrl = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build()
                .GetValue<string>("RopstenRpcHostUrl");
            
            _blockchainService = new BlockchainService("http://104.40.144.206:8000");            
        }

        #region BuildTransaction
        
        private string BuildTransaction(string to = ValidAddress, int amount = 2, int nonce = 0, int gasPrice = 2, int gasAmount = 2)
        {
            return _blockchainService.BuildTransaction(to, amount, nonce, gasPrice, gasAmount);
        }
        
        [TestMethod]
        public void BuildTransaction__ValidParametersPassed__TransactionBuilt()
        {
            var actualResult = BuildTransaction();

            Assert.AreEqual("dd80020294ea674fdde714fd979de3edf0f56aa9716b898ec80280808080", actualResult);
        }
        
        [TestMethod]
        public void BuildTransaction__ToAddressIsNullOrEmpty__ExceptionThrown()
        {
            Assert.ThrowsException<ArgumentException>
            (
                () => BuildTransaction(to: null)
            );
        }
        
        [TestMethod]
        public void BuildTransaction__ToAddressIsInvalid__ExceptionThrown()
        {
            Assert.ThrowsException<ArgumentException>
            (
                () => BuildTransaction(to: InvalidAddress)
            );
        }
        
        [TestMethod]
        public void BuildTransaction__AmounIsLowerOrEqualToZero__ExceptionThrown()
        {
            Assert.ThrowsException<ArgumentException>
            (
                () => BuildTransaction(amount: 0)
            );
            
            Assert.ThrowsException<ArgumentException>
            (
                () => BuildTransaction(amount: -1)
            );
        }
        
        [TestMethod]
        public void BuildTransaction__NonceIsLowerThanZero__ExceptionThrown()
        {
            Assert.ThrowsException<ArgumentException>
            (
                () => BuildTransaction(nonce: -1)
            );
        }
        
        [TestMethod]
        public void BuildTransaction__GasPriceIsLowerOrEqualToZero__ExceptionThrown()
        {
            Assert.ThrowsException<ArgumentException>
            (
                () => BuildTransaction(gasPrice: 0)
            );
            
            Assert.ThrowsException<ArgumentException>
            (
                () => BuildTransaction(gasPrice: -1)
            );
        }
        
        [TestMethod]
        public void BuildTransaction__GasAmountIsLowerOrEqualToZero__ExceptionThrown()
        {
            Assert.ThrowsException<ArgumentException>
            (
                () => BuildTransaction(gasAmount: 0)
            );
            
            Assert.ThrowsException<ArgumentException>
            (
                () => BuildTransaction(gasAmount: -1)
            );
        }
        
        #endregion

        #region EstimateGasPriceAsync

        private Task<BigInteger> EstimateGasPriceAsync(string to = ValidAddress, int amount = 2)
        {
            return _blockchainService.EstimateGasPriceAsync(to, amount);
        }
        
        [TestMethod]
        public async Task EstimateGasPriceAsync__ValidParametersPassed__TransactionBuilt()
        {
            var actualResult = await EstimateGasPriceAsync();

            Assert.IsTrue(actualResult > 0);
        }
        
        [TestMethod]
        public async Task EstimateGasPriceAsync__ToAddressIsNullOrEmpty__ExceptionThrown()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>
            (
                () => EstimateGasPriceAsync(to: null)
            );
        }
        
        [TestMethod]
        public async Task EstimateGasPriceAsync__ToAddressIsInvalid__ExceptionThrown()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>
            (
                () => EstimateGasPriceAsync(to: InvalidAddress)
            );
        }
        
        [TestMethod]
        public async Task EstimateGasPriceAsync__AmounIsLowerOrEqualToZero__ExceptionThrown()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>
            (
                () => EstimateGasPriceAsync(amount: 0)
            );
            
            await Assert.ThrowsExceptionAsync<ArgumentException>
            (
                () => EstimateGasPriceAsync(amount: -1)
            );
        }
        
        #endregion
        
        #region GetBalanceAsync
        
        private Task<BigInteger> GetBalanceAsync(string address = "0x65513Ecd11FD3a5b1FefdCc6A500B025008405A2", int blockNumber = 3281940)
        {
            return _blockchainService.GetBalanceAsync(address, blockNumber);
        }
        
        [TestMethod]
        public async Task GetBalanceAsync__ValidParametersPassed_And_BlockExists__ValidBalanceReturned()
        {
            var expectedResult = BigInteger.Parse("3000000000000000000");
            var actualResult = await GetBalanceAsync();
            
            Assert.AreEqual(expectedResult, actualResult);
        }
        
        [TestMethod]
        public async Task GetBalanceAsync__ValidParametersPassed_And_BlockDoesNotExist__ExceptionThrown()
        {
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>
            (
                () => GetBalanceAsync(blockNumber: 90000000)
            );
        }
        
        
        #endregion

        private class BlockchainService : BlockchainServiceBase
        {
            public BlockchainService(string rpcHostUrl) 
                : base(new Web3(rpcHostUrl))
            {
                
            }

            public override Task<BigInteger> GetNextNonceAsync(string address)
            {
                throw new NotSupportedException();
            }

            public override Task<string> GetTransactionErrorAsync(string txHash)
            {
                throw new NotSupportedException();
            }

            public override Task<IEnumerable<TransactionDto>> GetTransactionsAsync(BigInteger blockNumber)
            {
                throw new NotSupportedException();
            }
        }
    }
}
