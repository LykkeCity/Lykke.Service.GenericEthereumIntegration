using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Services;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.Interfaces;
using Lykke.Service.GenericEthereumIntegration.TDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services
{
    [TestClass]
    public class ObservableAddressServiceTests
    {
        [TestMethod]
        public async Task AddToIncomingObservationListAsync___InvalidArgumentsPassed___ExcpetionThrown()
        {
            const string address = nameof(address);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(address);
            
            var serviceBuilder = new ObservableAddressServiceBuilder();
            
            var service = serviceBuilder.Build();
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.AddToIncomingObservationListAsync
                    (
                        address: testCase.GetParameterValue<string>(address)
                    )
                );
            }
        }
        
        [TestMethod]
        public async Task AddToIncomingObservationListAsync___AddressIsAlreadyObserved___ExceptionThrown()
        {
            var serviceBuilder = new ObservableAddressServiceBuilder
            {
                TryAddToIncomingObservationListResult = false
            };
            
            var service = serviceBuilder.Build();

            await Assert.ThrowsExceptionAsync<ConflictException>
            (
                () => service.AddToIncomingObservationListAsync(TestValues.ValidAddress1)
            );
        }
        
        [TestMethod]
        public async Task AddToIncomingObservationListAsync___AddressAddedToObservationList___Returned()
        {
            var serviceBuilder = new ObservableAddressServiceBuilder
            {
                TryAddToIncomingObservationListResult = true
            };
            
            var service = serviceBuilder.Build();

            await service.AddToIncomingObservationListAsync(TestValues.ValidAddress1);
        }
        
        
        [TestMethod]
        public async Task AddToOutgoingObservationListAsync___InvalidArgumentsPassed___ExcpetionThrown()
        {
            const string address = nameof(address);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(address);
            
            var serviceBuilder = new ObservableAddressServiceBuilder();
            
            var service = serviceBuilder.Build();
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.AddToOutgoingObservationListAsync
                    (
                        address: testCase.GetParameterValue<string>(address)
                    )
                );
            }
        }
        
        [TestMethod]
        public async Task AddToOutgoingObservationListAsync___AAddressIsAlreadyObserved___ExceptionThrown()
        {
            var serviceBuilder = new ObservableAddressServiceBuilder
            {
                TryAddToOutgoingObservationListResult = false
            };
            
            var service = serviceBuilder.Build();

            await Assert.ThrowsExceptionAsync<ConflictException>
            (
                () => service.AddToOutgoingObservationListAsync(TestValues.ValidAddress1)
            );
        }
        
        [TestMethod]
        public async Task AddToOutgoingObservationListAsync___AddressAddedToObservationList___Returned()
        {
            var serviceBuilder = new ObservableAddressServiceBuilder
            {
                TryAddToOutgoingObservationListResult = true
            };
            
            var service = serviceBuilder.Build();

            await service.AddToOutgoingObservationListAsync(TestValues.ValidAddress1);
        }
        
        
        [TestMethod]
        public async Task DeleteFromIncomingObservationListAsync___InvalidArgumentsPassed___ExcpetionThrown()
        {
            const string address = nameof(address);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(address);
            
            var serviceBuilder = new ObservableAddressServiceBuilder();
            
            var service = serviceBuilder.Build();
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.DeleteFromIncomingObservationListAsync
                    (
                        address: testCase.GetParameterValue<string>(address)
                    )
                );
            }
        }
        
        [TestMethod]
        public async Task DeleteFromIncomingObservationListAsync___AddressDoesNotExistInObservationList___ExceptionThrown()
        {
            var serviceBuilder = new ObservableAddressServiceBuilder
            {
                DeleteFromIncomingObservationListResult = false
            };
            
            var service = serviceBuilder.Build();

            await Assert.ThrowsExceptionAsync<NotFoundException>
            (
                () => service.DeleteFromIncomingObservationListAsync(TestValues.ValidAddress1)
            );
        }
        
        [TestMethod]
        public async Task DeleteFromIncomingObservationListAsync___AddressRemovedFromObservationList___Returned()
        {
            var serviceBuilder = new ObservableAddressServiceBuilder
            {
                DeleteFromIncomingObservationListResult = true
            };
            
            var service = serviceBuilder.Build();

            await service.DeleteFromIncomingObservationListAsync(TestValues.ValidAddress1);
        }
        
        
        [TestMethod]
        public async Task DeleteFromOutgoingObservationListAsync___InvalidArgumentsPassed___ExcpetionThrown()
        {
            const string address = nameof(address);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(address);
            
            var serviceBuilder = new ObservableAddressServiceBuilder();
            
            var service = serviceBuilder.Build();
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => service.DeleteFromOutgoingObservationListAsync
                    (
                        address: testCase.GetParameterValue<string>(address)
                    )
                );
            }
        }
        
        [TestMethod]
        public async Task DeleteFromOutgoingObservationListAsync___AddressDoesNotExistInObservationList___ExceptionThrown()
        {
            var serviceBuilder = new ObservableAddressServiceBuilder
            {
                DeleteFromOutgoingObservationListResult = false
            };
            
            var service = serviceBuilder.Build();

            await Assert.ThrowsExceptionAsync<NotFoundException>
            (
                () => service.DeleteFromOutgoingObservationListAsync(TestValues.ValidAddress1)
            );
        }
        
        [TestMethod]
        public async Task DeleteFromOutgoingObservationListAsync___AddressRemovedFromObservationList___Returned()
        {
            var serviceBuilder = new ObservableAddressServiceBuilder
            {
                DeleteFromOutgoingObservationListResult = true
            };
            
            var service = serviceBuilder.Build();

            await service.DeleteFromOutgoingObservationListAsync(TestValues.ValidAddress1);
        }


        [PublicAPI]
        private class ObservableAddressServiceBuilder
        {
            private bool _deleteFromIncomingObservationListResult;
            private bool _deleteFromOutgoingObservationListResult;
            private bool _tryAddToIncomingObservationListResult;
            private bool _tryAddToOutgoingObservationListResult;
            
            
            public ObservableAddressServiceBuilder()
            {
                ObservableAddressRepository = new Mock<IObservableAddressRepository>();
            }

            
            public Mock<IObservableAddressRepository> ObservableAddressRepository { get; }


            public bool DeleteFromIncomingObservationListResult
            {
                get => _deleteFromIncomingObservationListResult;
                set
                {
                    _deleteFromIncomingObservationListResult = value;

                    ObservableAddressRepository
                        .Setup(x => x.TryDeleteFromIncomingObservationListAsync(It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }

            public bool DeleteFromOutgoingObservationListResult
            {
                get => _deleteFromOutgoingObservationListResult;
                set
                {
                    _deleteFromOutgoingObservationListResult = value;

                    ObservableAddressRepository
                        .Setup(x => x.TryDeleteFromOutgoingObservationListAsync(It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }

            public bool TryAddToIncomingObservationListResult
            {
                get => _tryAddToIncomingObservationListResult;
                set
                {
                    _tryAddToIncomingObservationListResult = value;

                    ObservableAddressRepository
                        .Setup(x => x.TryAddToIncomingObservationListAsync(It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }

            public bool TryAddToOutgoingObservationListResult
            {
                get => _tryAddToOutgoingObservationListResult;
                set
                {
                    _tryAddToOutgoingObservationListResult = value;

                    ObservableAddressRepository
                        .Setup(x => x.TryAddToOutgoingObservationListAsync(It.IsAny<string>()))
                        .ReturnsAsync(value);
                }
            }


            public ObservableAddressService Build()
            {
                return new ObservableAddressService
                (
                    ObservableAddressRepository.Object
                );
            }
        }
    }
}
