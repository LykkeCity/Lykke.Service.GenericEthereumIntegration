using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Models;
using Lykke.Service.GenericEthereumIntegration.Api.Services;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Extensions;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.TDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services
{
    [TestClass]
    public class AddressValidationServiceTests
    {
        [TestMethod]
        public async Task ValidateAsync__InvalidArgumentsPassed__ExcpetionThrown()
        {
            const string address = nameof(address);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterParameter(address, new[]
                {
                    (string.Empty, false),
                    (null, false)
                });
            
            var serviceBuilder = new AddressValidationServiceBuilder();

            var service = serviceBuilder.Build();

            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.ValidateAsync
                    (
                        address: testCase.GetParameterValue<string>(address)
                    )
                );
            }
        }
        
        [TestMethod]
        public async Task ValidateAsync__AddressIsInvalid__FalseReturned()
        {
            var serviceBuilder = new AddressValidationServiceBuilder();

            var service = serviceBuilder.Build();
            
            Assert.IsFalse(await service.ValidateAsync(TestValues.InvalidAddress1));
            
            serviceBuilder
                .BlockchainService
                .Verify(x => x.GetCodeAsync(It.IsAny<string>()), Times.Never);
        }
        
        [TestMethod]
        public async Task ValidateAsync__AddressIsValid_And_IsWallet__TrueReturned()
        {
            var serviceBuilder = new AddressValidationServiceBuilder
            {
                GetCodeResult = "0x"
            };

            var service = serviceBuilder.Build();
            
            Assert.IsTrue(await service.ValidateAsync(TestValues.ValidAddress1));
        }
        
        [TestMethod]
        public async Task ValidateAsync__AddressIsValid_And_IsContract__FalseReturned()
        {
            var serviceBuilder = new AddressValidationServiceBuilder
            {
                GetCodeResult = "0xAe"
            };

            var service = serviceBuilder.Build();
            
            Assert.IsFalse(await service.ValidateAsync(TestValues.ValidAddress1));
        }


        [PublicAPI]
        private class AddressValidationServiceBuilder
        {
            private string _getCodeResult;
            
            public AddressValidationServiceBuilder()
            {
                BlockchainService = new Mock<IBlockchainService>();
            }
            
            
            public Mock<IBlockchainService> BlockchainService { get; }

            public string GetCodeResult
            {
                get => _getCodeResult;
                set
                {
                    _getCodeResult = value;
                    
                    BlockchainService
                        .Setup(x => x.GetCodeAsync(It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }
            
            
            public AddressValidationService Build()
            {
                return new AddressValidationService
                (
                    BlockchainService.Object
                );
            }
        }
    }
}
