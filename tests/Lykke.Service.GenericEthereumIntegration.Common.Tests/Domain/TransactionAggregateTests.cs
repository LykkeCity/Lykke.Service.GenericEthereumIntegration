using System;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;
using Lykke.Service.GenericEthereumIntegration.TDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.GenericEthereumIntegration.Common.Tests.Domain
{
    [TestClass]
    public class TransactionAggregateTests
    {
        [TestMethod]
        public void Build__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string amount = nameof(amount);
            const string fee = nameof(fee);
            const string fromAddress = nameof(fromAddress);
            const string gasPrice = nameof(gasPrice);
            const string includeFee = nameof(includeFee);
            const string nonce = nameof(nonce);
            const string operationId = nameof(operationId);
            const string toAddress = nameof(toAddress);
            const string txData = nameof(txData);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterParameter(amount, new[] {(-1, false), (0, false), (1, true)})
                .RegisterParameter(fee, new[] {(-1, false), (0, false), (1, true)})
                .RegisterAddressParameter(fromAddress)
                .RegisterParameter(gasPrice, new[] {(-1, false), (0, false), (1, true)})
                .RegisterParameter(includeFee, new[] {(true, true), (true, true)})
                .RegisterParameter(nonce, new[] {(-1, false), (0, true)})
                .RegisterParameter(operationId, new[] {(Guid.NewGuid(), true)})
                .RegisterAddressParameter(toAddress)
                .RegisterHexStringParameter(txData);
                
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                Assert.ThrowsException<ArgumentException>
                (
                    () => TransactionAggregate.Build
                    (
                        amount: testCase.GetParameterValue<int>(amount),
                        fee: testCase.GetParameterValue<int>(fee),
                        fromAddress: testCase.GetParameterValue<string>(fromAddress),
                        gasPrice: testCase.GetParameterValue<int>(gasPrice),
                        includeFee: testCase.GetParameterValue<bool>(includeFee),
                        nonce: testCase.GetParameterValue<int>(nonce),
                        operationId: testCase.GetParameterValue<Guid>(operationId),
                        toAddress: testCase.GetParameterValue<string>(toAddress),
                        txData: testCase.GetParameterValue<string>(txData)
                    )
                );
            }
        }

        [TestMethod]
        public void OnBroadcasted__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string signedTxData = nameof(signedTxData);
            const string signedTxHash = nameof(signedTxHash);
            
            var transaction = BuildTransaction(broadcast: true);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterHexStringParameter(signedTxData)
                .RegisterHexStringParameter(signedTxHash);

            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                Assert.ThrowsException<ArgumentException>
                (
                    () => transaction.OnBroadcasted
                    (
                        testCase.GetParameterValue<string>(signedTxData),
                        testCase.GetParameterValue<string>(signedTxHash)
                    )
                );
            }
        }
        
        [TestMethod]
        public async Task OnBroadcasted__ValidArgumentsPassed__PropertiesUpdated()
        {
            var signedTxData = $"0x{Guid.NewGuid():N}";
            var signedTxHash = $"0x{Guid.NewGuid():N}";
            
            var transaction = BuildTransaction(broadcast: false);

            await Task.Delay(100);
            
            transaction.OnBroadcasted(signedTxData, signedTxHash);

            Assert.AreEqual(signedTxData, transaction.SignedTxData);
            Assert.AreEqual(signedTxHash, transaction.SignedTxHash);
            Assert.AreEqual(TransactionState.InProgress, transaction.State);
            Assert.IsTrue(transaction.BroadcastedOn.HasValue);
            Assert.IsTrue(transaction.BroadcastedOn.Value > transaction.BuiltOn);
        }
        
        [TestMethod]
        public void OnCompleted__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string blockNumber = nameof(blockNumber);
            
            var transaction = BuildTransaction(broadcast: true);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterParameter(blockNumber, new[]
                {
                    (-1, false),
                    (0, true)
                });

            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                Assert.ThrowsException<ArgumentException>
                (
                    () => transaction.OnCompleted
                    (
                        testCase.GetParameterValue<int>(blockNumber)
                    )
                );
            }
        }
        
        [TestMethod]
        public async Task OnCompleted__ValidArgumentsPassed__PropertiesUpdated()
        {
            const int blockNumber = 42;
            
            var transaction = BuildTransaction(broadcast: true);

            await Task.Delay(100);
            
            transaction.OnCompleted(blockNumber);

            Assert.AreEqual(blockNumber, transaction.BlockNumber);
            Assert.AreEqual(TransactionState.Completed, transaction.State);
            Assert.IsTrue(transaction.BroadcastedOn.HasValue);
            Assert.IsTrue(transaction.CompletedOn.HasValue);
            Assert.IsTrue(transaction.CompletedOn.Value > transaction.BroadcastedOn.Value);
        }
        
        [TestMethod]
        public void OnFailed__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string blockNumber = nameof(blockNumber);
            const string error = nameof(error);
            
            var transaction = BuildTransaction(broadcast: true);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterParameter(blockNumber, new[]
                {
                    (-1, false),
                    (0, true)
                })
                .RegisterParameter(error, new[]
                {
                    (string.Empty, false),
                    (null, false),
                    ("Error description", true)
                });

            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                Assert.ThrowsException<ArgumentException>
                (
                    () => transaction.OnFailed
                    (
                        testCase.GetParameterValue<int>(blockNumber),
                        testCase.GetParameterValue<string>(error)
                    )
                );
            }
        }
        
        [TestMethod]
        public async Task OnFailed__ValidArgumentsPassed__PropertiesUpdated()
        {
            const int blockNumber = 42;
            const string error = "Error description";
            
            var transaction = BuildTransaction(broadcast: true);

            await Task.Delay(100);
            
            transaction.OnFailed(blockNumber, error);

            Assert.AreEqual(blockNumber, transaction.BlockNumber);
            Assert.AreEqual(TransactionState.Failed, transaction.State);
            Assert.IsTrue(transaction.BroadcastedOn.HasValue);
            Assert.IsTrue(transaction.CompletedOn.HasValue);
            Assert.IsTrue(transaction.CompletedOn.Value > transaction.BroadcastedOn.Value);
        }

        private static TransactionAggregate BuildTransaction(bool broadcast)
        {
            var transaction = TransactionAggregate.Build
            (
                amount: 1,
                fee: 1,
                fromAddress: TestValues.ValidAddress1,
                gasPrice: 1,
                includeFee: false,
                nonce: 0,
                operationId: Guid.NewGuid(),
                toAddress: TestValues.ValidAddress2,
                txData: $"0x{Guid.NewGuid():N}"
            );

            if (broadcast)
            {
                transaction.OnBroadcasted
                (
                    $"0x{Guid.NewGuid():N}",
                    TestValues.ValidTransactionHash1
                );
            }

            return transaction;
        }
    }
}
