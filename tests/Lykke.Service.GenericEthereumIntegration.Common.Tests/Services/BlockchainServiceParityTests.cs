using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.Parity;

namespace Lykke.Service.GenericEthereumIntegration.Common.Tests.Services
{
    [TestClass]
    [TestCategory("Integration")]
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
        
        [TestMethod]
        public override void BuildTransaction__InvalidArgumentsPassed__ExceptionThrown()
        {
            base.BuildTransaction__InvalidArgumentsPassed__ExceptionThrown();
        }
        
        [TestMethod]
        public override async Task CheckIfBroadcastedAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            await base.CheckIfBroadcastedAsync__InvalidArgumentsPassed__ExceptionThrown();
        }

        [TestMethod]
        public override async Task EstimateGasPriceAsync__ValidArgumentsPassed__TransactionBuilt()
        {
            await base.EstimateGasPriceAsync__ValidArgumentsPassed__TransactionBuilt();
        }

        [TestMethod]
        public override async Task EstimateGasPriceAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            await base.EstimateGasPriceAsync__InvalidArgumentsPassed__ExceptionThrown();
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

        [TestMethod]
        public override async Task GetBalanceAsync__InvalidArguemtnsPassed__ExceptionThrown()
        {
            await base.GetBalanceAsync__InvalidArguemtnsPassed__ExceptionThrown();
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

        [TestMethod]
        public override async Task GetBlockHashAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            await base.GetBlockHashAsync__InvalidArgumentsPassed__ExceptionThrown();
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

        [TestMethod]
        public override async Task GetCodeAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            await base.GetCodeAsync__InvalidArgumentsPassed__ExceptionThrown();
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

        [TestMethod]
        public override async Task GetTimestampAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            await base.GetTimestampAsync__InvalidArgumentsPassed__ExceptionThrown();
        }

        [TestMethod]
        public override void GetTransactionHash__ValidTransactionDataPassed__ValidTransactionHashReturned()
        {
            base.GetTransactionHash__ValidTransactionDataPassed__ValidTransactionHashReturned();
        }
        
        [TestMethod]
        public override void GetTransactionHash__InvalidArgumentsPassed__ExceptionThrown()
        {
            base.GetTransactionHash__InvalidArgumentsPassed__ExceptionThrown();
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

        [TestMethod]
        public override async Task GetTransactionsAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            await base.GetTransactionsAsync__InvalidArgumentsPassed__ExceptionThrown();
        }


        [TestMethod]
        public override void GetTransactionSigner__ValidArgumentsPassed__ValidTransactionSignerReturned()
        {
            base.GetTransactionSigner__ValidArgumentsPassed__ValidTransactionSignerReturned();
        }

        [TestMethod]
        public override void GetTransactionSigner__InvalidArgumentsPassed__ExceptionThrown()
        {
            base.GetTransactionSigner__InvalidArgumentsPassed__ExceptionThrown();
        }

        [TestMethod]
        public override void GetTransactionSigner__TransactionHasNotBeenSigned__ExceptionThrown()
        {
            base.GetTransactionSigner__TransactionHasNotBeenSigned__ExceptionThrown();
        }

        [TestMethod]
        public override async Task SendRawTransactionAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            await base.SendRawTransactionAsync__InvalidArgumentsPassed__ExceptionThrown();
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

        [TestMethod]
        public override async Task TryGetTransactionErrorAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            await base.TryGetTransactionErrorAsync__InvalidArgumentsPassed__ExceptionThrown();
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

        [TestMethod]
        public override async Task TryGetTransactionReceiptAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            await base.TryGetTransactionReceiptAsync__InvalidArgumentsPassed__ExceptionThrown();
        }

        [TestMethod]
        public override async Task UnsignTransactionAsync__ValidArgumentsPassed__ValidUnsignedTxDataReturned()
        {
            await base.UnsignTransactionAsync__ValidArgumentsPassed__ValidUnsignedTxDataReturned();
        }

        [TestMethod]
        public override async Task UnsignTransactionAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            await base.UnsignTransactionAsync__InvalidArgumentsPassed__ExceptionThrown();
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
