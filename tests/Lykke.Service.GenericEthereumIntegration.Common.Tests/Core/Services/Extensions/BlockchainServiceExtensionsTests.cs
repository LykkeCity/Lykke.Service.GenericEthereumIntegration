using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Extensions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.TDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.GenericEthereumIntegration.Common.Tests.Core.Services.Extensions
{
    [TestClass]
    public class BlockchainServiceExtensionsTests
    {
        [TestMethod]
        public async Task IsWalletAsync__GetCodeAsyncReturnsEmptyResult__TrueReturned()
        {
            var service = new Mock<IBlockchainService>();

            service
                .Setup(x => x.GetCodeAsync(It.IsAny<string>()))
                .ReturnsAsync("0x");
            
            Assert.IsTrue(await service.Object.IsWalletAsync(TestValues.ValidAddress1));
            
            service
                .Verify(x => x.GetCodeAsync(It.IsAny<string>()), Times.Once);
            
            service
                .VerifyNoOtherCalls();
        }
        
        [TestMethod]
        public async Task IsWalletAsync__GetCodeAsyncReturnsNonEmptyResult__FalseReturned()
        {
            var service = new Mock<IBlockchainService>();

            service
                .Setup(x => x.GetCodeAsync(It.IsAny<string>()))
                .ReturnsAsync("0xAe");
            
            Assert.IsFalse(await service.Object.IsWalletAsync(TestValues.ValidAddress1));
            
            service
                .Verify(x => x.GetCodeAsync(It.IsAny<string>()), Times.Once);
            
            service
                .VerifyNoOtherCalls();
        }
    }
}
