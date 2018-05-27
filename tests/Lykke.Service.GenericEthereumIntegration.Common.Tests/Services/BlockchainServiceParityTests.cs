using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.Parity;

namespace Lykke.Service.GenericEthereumIntegration.Common.Tests.Services
{
    [TestClass]
    public class BlockchainServiceParityTests : BlockchainServiceTestsBase
    {
        public BlockchainServiceParityTests()
        {
            BlockchainService = new BlockchainServiceParity
            (
                new Web3Parity(RpcHostUrl)
            );
        }
        
        protected override IBlockchainService BlockchainService { get; }

        protected override string RpcHostUrlKey
            => "RopstenParityRpcHostUrl";

        
        [TestMethod]
        public override void BuildTransaction__ValidArgumentsPassed__TransactionBuilt()
        {
            base.BuildTransaction__ValidArgumentsPassed__TransactionBuilt();
        }
        
        [DataTestMethod]
        [DataRow(null, 1, 0, 1, 1)]                                             // to is null
        [DataRow("",   1, 0, 1, 1)]                                             // to is empty
        [DataRow("0xea674fdDe714fd979de3EdF0F56AA9716B898ec8",  1,  0,  1,  1)] // to is invalid
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8",  0,  0,  1,  1)] // amount == 0
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8", -1,  0,  1,  1)] // amount < 0
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8",  1, -1,  1,  1)] // none < 0
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8",  1,  0,  0,  1)] // gasPrice == 0
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8",  1,  0, -1,  1)] // gasPrice < 0
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8",  1,  0,  1,  0)] // gasAmount == 0
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8",  1,  0,  1, -1)] // gasAmount < 0
        public override void BuildTransaction__IncalidArgumentsPassed__ExceptionThrown(string to, int amount, int nonce, int gasPrice, int gasAmount)
        {
            base.BuildTransaction__IncalidArgumentsPassed__ExceptionThrown(to, amount, nonce, gasPrice, gasAmount);
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("0xA")]
        [DataRow("0xZe")]
        public override async Task CheckIfBroadcastedAsync__InvalidArgumentsPassed__ExceptionThrown(string txHash)
        {
            await base.CheckIfBroadcastedAsync__InvalidArgumentsPassed__ExceptionThrown(txHash);
        }

        [TestMethod]
        public override async Task EstimateGasPriceAsync__ValidArgumentsPassed__TransactionBuilt()
        {
            await base.EstimateGasPriceAsync__ValidArgumentsPassed__TransactionBuilt();
        }

        [DataTestMethod]
        [DataRow(null, 1)]                                          // to is null
        [DataRow("",   1)]                                          // to is empty
        [DataRow("0xea674fdDe714fd979de3EdF0F56AA9716B898ec8",  1)] // to is invalid
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8",  0)] // amount == 0
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8", -1)] // amount < 0
        public override async Task EstimateGasPriceAsync__InvalidArgumentsPassed__ExceptionThrown(string to, int amount)
        {
            await base.EstimateGasPriceAsync__InvalidArgumentsPassed__ExceptionThrown(to, amount);
        }
        
        [TestMethod]
        public override async Task GetBalanceAsync__ValidArgumentsPassed_And_BlockExists__ValidBalanceReturned()
        {
            await base.GetBalanceAsync__ValidArgumentsPassed_And_BlockExists__ValidBalanceReturned();
        }

        [TestMethod]
        public override async Task GetBalanceAsync__ValidArgumentsPassed_And_BlockDoesNotExist__ExceptionThrown()
        {
            await base.GetBalanceAsync__ValidArgumentsPassed_And_BlockDoesNotExist__ExceptionThrown();
        }

        [DataTestMethod]
        [DataRow(null, 1)]                                          // address is null
        [DataRow("",   1)]                                          // address is empty
        [DataRow("0xea674fdDe714fd979de3EdF0F56AA9716B898ec8",  1)] // address is invalid
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8", -1)] // blockNumber < 0
        public override async Task GetBalanceAsync__InvalidArguemtnsPassed__ExceptionThrown(string address, int blockNumber)
        {
            await base.GetBalanceAsync__InvalidArguemtnsPassed__ExceptionThrown(address, blockNumber);
        }

        [TestMethod]
        public override async Task GetBlockHashAsync__ValidArgumentsPassed_And_BlockExists__ValidBlockHashReturned()
        {
            await base.GetBlockHashAsync__ValidArgumentsPassed_And_BlockExists__ValidBlockHashReturned();
        }

        [TestMethod]
        public override async Task GetBlockHashAsync__ValidArgumentsPassed_And_BlockDoesNotExist__ExceptionThrown()
        {
            await base.GetBlockHashAsync__ValidArgumentsPassed_And_BlockDoesNotExist__ExceptionThrown();
        }

        [DataTestMethod]
        [DataRow(-1)] // blockNumber < 0
        public override async Task GetBlockHashAsync__InvalidArgumentsPassed__ExceptionThrown(int blockNumber)
        {
            await base.GetBlockHashAsync__InvalidArgumentsPassed__ExceptionThrown(blockNumber);
        }

        [TestMethod]
        public override async Task GetCodeAsync__AddressIsWallet__EmptyResultReturned()
        {
            await base.GetCodeAsync__AddressIsWallet__EmptyResultReturned();
        }

        [TestMethod]
        public override async Task GetCodeAsync__AddressIsContract__ValidCodeReturned()
        {
            await base.GetCodeAsync__AddressIsContract__ValidCodeReturned();
        }

        [DataTestMethod]
        [DataRow(null)]                                         // address is null
        [DataRow("")]                                           // address is empty
        [DataRow("0xea674fdDe714fd979de3EdF0F56AA9716B898ec8")] // address is invalid
        public override async Task GetCodeAsync__InvalidArgumentsPassed__ExceptionThrown(string address)
        {
            await base.GetCodeAsync__InvalidArgumentsPassed__ExceptionThrown(address);
        }

        [TestMethod]
        public override async Task GetLatestBlockNumberAsync__BlockNumberReturned()
        {
            await base.GetLatestBlockNumberAsync__BlockNumberReturned();
        }

        [TestMethod]
        public override async Task GetTimestampAsync__ValidArgumentsPassed_And_BlockExists__ValidTimestampReturned()
        {
            await base.GetTimestampAsync__ValidArgumentsPassed_And_BlockExists__ValidTimestampReturned();
        }

        [TestMethod]
        public override async Task GetTimestampAsync__ValidArgumentsPassed_And_BlockDoesNotExist__ExceptionThrown()
        {
            await base.GetTimestampAsync__ValidArgumentsPassed_And_BlockDoesNotExist__ExceptionThrown();
        }

        [DataTestMethod]
        [DataRow(-1)] // blockNumber < 0
        public override async Task GetTimestampAsync__InvalidArgumentsPassed__ExceptionThrown(int blockNumber)
        {
            await base.GetTimestampAsync__InvalidArgumentsPassed__ExceptionThrown(blockNumber);
        }

        [TestMethod]
        public override void GetTransactionHash__ValidTransactionDataPassed__ValidTransactionHashReturned()
        {
            base.GetTransactionHash__ValidTransactionDataPassed__ValidTransactionHashReturned();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("0xA")]
        [DataRow("0xZe")]
        public override void GetTransactionHash__InvalidArgumentsPassed__ExceptionThrown(string txData)
        {
            base.GetTransactionHash__InvalidArgumentsPassed__ExceptionThrown(txData);
        }

        [TestMethod]
        public override async Task GetTransactionsAsync__BlockIsNotEmpty__ValidTransactionListReturned()
        {
            await base.GetTransactionsAsync__BlockIsNotEmpty__ValidTransactionListReturned();
        }

        [TestMethod]
        public override async Task GetTransactionsAsync__BlockIsEmpty__EmptyTransactionListReturned()
        {
            await base.GetTransactionsAsync__BlockIsEmpty__EmptyTransactionListReturned();
        }

        [DataTestMethod]
        [DataRow(-1)] // blockNumber < 0
        public override async Task GetTransactionsAsync__InvalidArgumentsPassed__ExceptionThrown(int blockNumber)
        {
            await base.GetTransactionsAsync__InvalidArgumentsPassed__ExceptionThrown(blockNumber);
        }


        [TestMethod]
        public override void GetTransactionSigner__ValidArgumentsPassed__ValidTransactionSignerReturned()
        {
            base.GetTransactionSigner__ValidArgumentsPassed__ValidTransactionSignerReturned();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("0xA")]
        [DataRow("0xZe")]
        public override void GetTransactionSigner__InvalidArgumentsPassed__ExceptionThrown(string signedTxData)
        {
            base.GetTransactionSigner__InvalidArgumentsPassed__ExceptionThrown(signedTxData);
        }

        [TestMethod]
        public override void GetTransactionSigner__TransactionHasNotBeenSigned__ExceptionThrown()
        {
            base.GetTransactionSigner__TransactionHasNotBeenSigned__ExceptionThrown();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("0xA")]
        [DataRow("0xZe")]
        public override async Task SendRawTransactionAsync__InvalidArgumentsPassed__ExceptionThrown(string signedTxData)
        {
            await base.SendRawTransactionAsync__InvalidArgumentsPassed__ExceptionThrown(signedTxData);
        }

        [TestMethod]
        public override async Task TryGetTransactionErrorAsync__TransactionIsFailed__ValidErrorListReturned()
        {
            await base.TryGetTransactionErrorAsync__TransactionIsFailed__ValidErrorListReturned();
        }

        [TestMethod]
        public override async Task TryGetTransactionErrorAsync__TransactionIsSuccessful__EmptyErrorListReturned()
        {
            await base.TryGetTransactionErrorAsync__TransactionIsSuccessful__EmptyErrorListReturned();
        }

        [TestMethod]
        public override async Task TryGetTransactionErrorAsync__TransactionDoesNotExist__NullReturned()
        {
            await base.TryGetTransactionErrorAsync__TransactionDoesNotExist__NullReturned();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("0xA")]
        [DataRow("0xZe")]
        public override async Task TryGetTransactionErrorAsync__InvalidArgumentsPassed__ExceptionThrown(string txHash)
        {
            await base.TryGetTransactionErrorAsync__InvalidArgumentsPassed__ExceptionThrown(txHash);
        }

        [TestMethod]
        public override async Task TryGetTransactionReceiptAsync__ExistingTxHashPassed__ValidTransactionReceiptReturned()
        {
            await base.TryGetTransactionReceiptAsync__ExistingTxHashPassed__ValidTransactionReceiptReturned();
        }

        [TestMethod]
        public override async Task TryGetTransactionReceiptAsync__NonExistingTxHashPassed__NullReturned()
        {
            await base.TryGetTransactionReceiptAsync__NonExistingTxHashPassed__NullReturned();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("0xA")]
        [DataRow("0xZe")]
        public override async Task TryGetTransactionReceiptAsync__InvalidArgumentsPassed__ExceptionThrown(string txHash)
        {
            await base.TryGetTransactionReceiptAsync__InvalidArgumentsPassed__ExceptionThrown(txHash);
        }

        [TestMethod]
        public override async Task UnsignTransactionAsync__ValidArgumentsPassed__ValidUnsignedTxDataReturned()
        {
            await base.UnsignTransactionAsync__ValidArgumentsPassed__ValidUnsignedTxDataReturned();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("0xA")]
        [DataRow("0xZe")]
        public override async Task UnsignTransactionAsync__InvalidArgumentsPassed__ExceptionThrown(string signedTxData)
        {
            await base.UnsignTransactionAsync__InvalidArgumentsPassed__ExceptionThrown(signedTxData);
        }

        [TestMethod]
        public override async Task UnsignTransactionAsync__TransactionHasData__ExceptionThrown()
        {
            await base.UnsignTransactionAsync__TransactionHasData__ExceptionThrown();
        }

        [TestMethod]
        public override async Task UnsignTransactionAsync__TransactionHasNotBeenSigned__EWxceptionThrown()
        {
            await base.UnsignTransactionAsync__TransactionHasNotBeenSigned__EWxceptionThrown();
        }
    }
}
