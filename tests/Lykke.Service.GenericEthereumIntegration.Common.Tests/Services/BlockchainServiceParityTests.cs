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
        public override void BuildTransaction__ValidParametersPassed__TransactionBuilt()
        {
            base.BuildTransaction__ValidParametersPassed__TransactionBuilt();
        }

        [TestMethod]
        public override void BuildTransaction__ToAddressIsNullOrEmpty__ExceptionThrown()
        {
            base.BuildTransaction__ToAddressIsNullOrEmpty__ExceptionThrown();
        }

        [TestMethod]
        public override void BuildTransaction__ToAddressIsInvalid__ExceptionThrown()
        {
            base.BuildTransaction__ToAddressIsInvalid__ExceptionThrown();
        }

        [TestMethod]
        public override void BuildTransaction__AmounIsLowerOrEqualToZero__ExceptionThrown()
        {
            base.BuildTransaction__AmounIsLowerOrEqualToZero__ExceptionThrown();
        }

        [TestMethod]
        public override void BuildTransaction__NonceIsLowerThanZero__ExceptionThrown()
        {
            base.BuildTransaction__NonceIsLowerThanZero__ExceptionThrown();
        }

        [TestMethod]
        public override void BuildTransaction__GasPriceIsLowerOrEqualToZero__ExceptionThrown()
        {
            base.BuildTransaction__GasPriceIsLowerOrEqualToZero__ExceptionThrown();
        }

        [TestMethod]
        public override void BuildTransaction__GasAmountIsLowerOrEqualToZero__ExceptionThrown()
        {
            base.BuildTransaction__GasAmountIsLowerOrEqualToZero__ExceptionThrown();
        }

        [TestMethod]
        public override async Task CheckIfBroadcastedAsync__TxHashIsNullOrEmpty__ExceptionThrown()
        {
            await base.CheckIfBroadcastedAsync__TxHashIsNullOrEmpty__ExceptionThrown();
        }

        [TestMethod]
        public override async Task CheckIfBroadcastedAsync__TxHashIsNotHexString__ExceptionThrown()
        {
            await base.CheckIfBroadcastedAsync__TxHashIsNotHexString__ExceptionThrown();
        }

        [TestMethod]
        public override async Task EstimateGasPriceAsync__ValidParametersPassed__TransactionBuilt()
        {
            await base.EstimateGasPriceAsync__ValidParametersPassed__TransactionBuilt();
        }

        [TestMethod]
        public override async Task EstimateGasPriceAsync__ToAddressIsNullOrEmpty__ExceptionThrown()
        {
            await base.EstimateGasPriceAsync__ToAddressIsNullOrEmpty__ExceptionThrown();
        }

        [TestMethod]
        public override async Task EstimateGasPriceAsync__ToAddressIsInvalid__ExceptionThrown()
        {
            await base.EstimateGasPriceAsync__ToAddressIsInvalid__ExceptionThrown();
        }

        [TestMethod]
        public override async Task EstimateGasPriceAsync__AmounIsLowerOrEqualToZero__ExceptionThrown()
        {
            await base.EstimateGasPriceAsync__AmounIsLowerOrEqualToZero__ExceptionThrown();
        }

        [TestMethod]
        public override async Task GetBalanceAsync__ValidParametersPassed_And_BlockExists__ValidBalanceReturned()
        {
            await base.GetBalanceAsync__ValidParametersPassed_And_BlockExists__ValidBalanceReturned();
        }

        [TestMethod]
        public override async Task GetBalanceAsync__ValidParametersPassed_And_BlockDoesNotExist__ExceptionThrown()
        {
            await base.GetBalanceAsync__ValidParametersPassed_And_BlockDoesNotExist__ExceptionThrown();
        }

        [TestMethod]
        public override async Task GetBalanceAsync__AddressIsNullOrEmpty__ExceptionThrown()
        {
            await base.GetBalanceAsync__AddressIsNullOrEmpty__ExceptionThrown();
        }

        [TestMethod]
        public override async Task GetBalanceAsync__AddressIsInvalid__ExceptionThrown()
        {
            await base.GetBalanceAsync__AddressIsInvalid__ExceptionThrown();
        }

        [TestMethod]
        public override async Task GetBalanceAsync__BlockNumberIsLowerThanZero__ExceptionThrown()
        {
            await base.GetBalanceAsync__BlockNumberIsLowerThanZero__ExceptionThrown();
        }

        [TestMethod]
        public override async Task GetBlockHashAsync__ValidParametersPassed_And_BlockExists__ValidBlockHashReturned()
        {
            await base.GetBlockHashAsync__ValidParametersPassed_And_BlockExists__ValidBlockHashReturned();
        }

        [TestMethod]
        public override async Task GetBlockHashAsync__ValidParametersPassed_And_BlockDoesNotExist__ExceptionThrown()
        {
            await base.GetBlockHashAsync__ValidParametersPassed_And_BlockDoesNotExist__ExceptionThrown();
        }

        [TestMethod]
        public override async Task GetBlockHashAsync__BlockNumberIsLowerThanZero__ExceptionThrown()
        {
            await base.GetBlockHashAsync__BlockNumberIsLowerThanZero__ExceptionThrown();
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
        public override async Task GetCodeAsync__AddressIsNullOrEmpty__ExceptionThrown()
        {
            await base.GetCodeAsync__AddressIsNullOrEmpty__ExceptionThrown();
        }

        [TestMethod]
        public override async Task GetCodeAsync__AddressIsInvalid__ExceptionThrown()
        {
            await base.GetCodeAsync__AddressIsInvalid__ExceptionThrown();
        }

        [TestMethod]
        public override async Task GetLatestBlockNumberAsync__BlockNumberReturned()
        {
            await base.GetLatestBlockNumberAsync__BlockNumberReturned();
        }

        [TestMethod]
        public override async Task GetTimestampAsync__BlockExists__ValidTimestampReturned()
        {
            await base.GetTimestampAsync__BlockExists__ValidTimestampReturned();
        }

        [TestMethod]
        public override async Task GetTimestampAsync__BlockDoesNotExist__ValidTimestampReturned()
        {
            await base.GetTimestampAsync__BlockDoesNotExist__ValidTimestampReturned();
        }

        [TestMethod]
        public override async Task GetTimestampAsync__BlockNumberIsLowerThanZero__ExceptionThrown()
        {
            await base.GetTimestampAsync__BlockNumberIsLowerThanZero__ExceptionThrown();
        }

        [TestMethod]
        public override void GetTransactionHash__ValidTransactionDataPassed__ValidTransactionHashReturned()
        {
            base.GetTransactionHash__ValidTransactionDataPassed__ValidTransactionHashReturned();
        }

        [TestMethod]
        public override void GetTransactionHash__TransactionDataIsNullOrEmpty__ExceptionThrown()
        {
            base.GetTransactionHash__TransactionDataIsNullOrEmpty__ExceptionThrown();
        }

        [TestMethod]
        public override void GetTransactionHash__TransactionDataIsNotHexString__ExceptionThrown()
        {
            base.GetTransactionHash__TransactionDataIsNotHexString__ExceptionThrown();
        }

        [TestMethod]
        public override async Task GetTransactionsAsync__BlockIsEmpty__EmptyTransactionListReturned()
        {
            await base.GetTransactionsAsync__BlockIsEmpty__EmptyTransactionListReturned();
        }

        [TestMethod]
        public override async Task GetTransactionsAsync__BlockNumberIsLowerThanZero__ExceptionThrown()
        {
            await base.GetTransactionsAsync__BlockNumberIsLowerThanZero__ExceptionThrown();
        }

        [TestMethod]
        public override void GetTransactionSigner__ValidTransactionDataPassed__ValidTransactionSignerReturned()
        {
            base.GetTransactionSigner__ValidTransactionDataPassed__ValidTransactionSignerReturned();
        }

        [TestMethod]
        public override void GetTransactionSigner__TransactionDataIsNullOrEmpty__ExceptionThrown()
        {
            base.GetTransactionSigner__TransactionDataIsNullOrEmpty__ExceptionThrown();
        }

        [TestMethod]
        public override void GetTransactionSigner__TransactionDataIsNotHexString__ExceptionThrown()
        {
            base.GetTransactionSigner__TransactionDataIsNotHexString__ExceptionThrown();
        }

        [TestMethod]
        public override void GetTransactionSigner__TransactionHasNotBeenSigned__EWxceptionThrown()
        {
            base.GetTransactionSigner__TransactionHasNotBeenSigned__EWxceptionThrown();
        }

        [TestMethod]
        public override async Task SendRawTransactionAsync__TxHashIsNullOrEmpty__ExceptionThrown()
        {
            await base.SendRawTransactionAsync__TxHashIsNullOrEmpty__ExceptionThrown();
        }

        [TestMethod]
        public override async Task SendRawTransactionAsync__TxHashIsNotHexString__ExceptionThrown()
        {
            await base.SendRawTransactionAsync__TxHashIsNotHexString__ExceptionThrown();
        }

        [TestMethod]
        public override async Task TryGetTransactionErrorAsync__FailedTxHashPassed__ValidErrorListReturned()
        {
            await base.TryGetTransactionErrorAsync__FailedTxHashPassed__ValidErrorListReturned();
        }

        [TestMethod]
        public override async Task TryGetTransactionErrorAsync__SuccessfulTxHashPassed__EmptyErrorListReturned()
        {
            await base.TryGetTransactionErrorAsync__SuccessfulTxHashPassed__EmptyErrorListReturned();
        }

        [TestMethod]
        public override async Task TryGetTransactionErrorAsync__NonExistingTxHashPassed__NullReturned()
        {
            await base.TryGetTransactionErrorAsync__NonExistingTxHashPassed__NullReturned();
        }

        [TestMethod]
        public override async Task TryGetTransactionErrorAsync__TransactionHashIsNullOrEmpty__ExceptionThrown()
        {
            await base.TryGetTransactionErrorAsync__TransactionHashIsNullOrEmpty__ExceptionThrown();
        }

        [TestMethod]
        public override async Task TryGetTransactionErrorAsync__TransactionHashIsNotHexString__ExceptionThrown()
        {
            await base.TryGetTransactionErrorAsync__TransactionHashIsNotHexString__ExceptionThrown();
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
        public override async Task TryGetTransactionReceiptAsync__TransactionHashIsNullOrEmpty__ExceptionThrown()
        {
            await base.TryGetTransactionReceiptAsync__TransactionHashIsNullOrEmpty__ExceptionThrown();
        }

        [TestMethod]
        public override async Task TryGetTransactionReceiptAsync__TransactionHashIsNotHexString__ExceptionThrown()
        {
            await base.TryGetTransactionReceiptAsync__TransactionHashIsNotHexString__ExceptionThrown();
        }

        [TestMethod]
        public override async Task UnsignTransactionAsync__ValidSignedTxDataPassed__ValidUnsignedTxDataReturned()
        {
            await base.UnsignTransactionAsync__ValidSignedTxDataPassed__ValidUnsignedTxDataReturned();
        }

        [TestMethod]
        public override async Task UnsignTransactionAsync__TxHashIsNullOrEmpty__ExceptionThrown()
        {
            await base.UnsignTransactionAsync__TxHashIsNullOrEmpty__ExceptionThrown();
        }

        [TestMethod]
        public override async Task UnsignTransactionAsync__TxHashIsNotHexString__ExceptionThrown()
        {
            await base.UnsignTransactionAsync__TxHashIsNotHexString__ExceptionThrown();
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
