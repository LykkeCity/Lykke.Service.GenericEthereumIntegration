using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;
using Lykke.Service.GenericEthereumIntegration.SignApi.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Tests.Services
{
    [TestClass]
    public class WalletServiceTests
    {
        [TestMethod]
        public async Task CreateWallet__ReturnValidWalletDto()
        {
            var walletService = new WalletService();

            var wallet = walletService.CreateWallet();
            
            Assert.AreEqual(66, wallet.PrivateKey.Length);
            Assert.IsTrue(wallet.PrivateKey.IsValidHexString());
            Assert.IsTrue(await AddressChecksum.ValidateAsync(wallet.PublicAddress));
        }
    }
}
