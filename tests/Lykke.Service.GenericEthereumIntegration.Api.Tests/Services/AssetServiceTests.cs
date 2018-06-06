using System;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Services;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings.Integration;
using Lykke.Service.GenericEthereumIntegration.TDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Lykke.Service.GenericEthereumIntegration.Api.Tests.Services
{
    [TestClass]
    public class AssetServiceTests
    {
        [TestMethod]
        public void GetAsset__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string assetId = nameof(assetId);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterParameter(assetId, new[]
                {
                    (string.Empty, false),
                    (null, false)
                });

            var serviceBuilder = new AssetServiceBuilder();

            var service = serviceBuilder.Build();
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                Assert.ThrowsException<ArgumentException>
                (
                    () => service.GetAsset
                    (
                        assetId: testCase.GetParameterValue<string>(assetId)
                    )
                );
            }
        }

        [TestMethod]
        public void GetAsset__ValidArgumentsPassed__ValidDataReturned()
        {
            var serviceBuilder = new AssetServiceBuilder
            {
                Accuracy = 42,
                Id = $"{Guid.NewGuid()}",
                Name = $"{Guid.NewGuid()}"
            };

            var service = serviceBuilder.Build();

            var actualResult = service.GetAsset(serviceBuilder.Id);

            Assert.AreEqual(serviceBuilder.Accuracy, actualResult.Accuracy);
            Assert.AreEqual(serviceBuilder.Id, actualResult.AssetId);
            Assert.AreEqual(serviceBuilder.Name, actualResult.Name);
        }

        [TestMethod]
        public void GetAsset__AssetIdIsInvalid__ExceptionThrown()
        {
            var serviceBuilder = new AssetServiceBuilder
            {
                Id = $"{Guid.NewGuid()}"
            };

            var service = serviceBuilder.Build();

            Assert.ThrowsException<NotFoundException>
            (
                () => service.GetAsset($"{Guid.NewGuid()}")
            );
        }
        
        [TestMethod]
        public void GetAssets__ValidDataReturned()
        {
            var serviceBuilder = new AssetServiceBuilder
            {
                Accuracy = 42,
                Id = $"{Guid.NewGuid()}",
                Name = $"{Guid.NewGuid()}"
            };

            var service = serviceBuilder.Build();

            var actualResult = service.GetAssets().ToList();
            
            Assert.AreEqual(1, actualResult.Count);
            Assert.AreEqual(serviceBuilder.Accuracy, actualResult[0].Accuracy);
            Assert.AreEqual(serviceBuilder.Id, actualResult[0].AssetId);
            Assert.AreEqual(serviceBuilder.Name, actualResult[0].Name);
        }
        
        [PublicAPI]
        private class AssetServiceBuilder
        {
            public AssetServiceBuilder()
            {
                AssetSettings = new AssetSettings();
            }
            
            public AssetSettings AssetSettings { get; }


            public int Accuracy
            {
                get => AssetSettings.Accuracy;
                set => AssetSettings.Accuracy = value;
            }

            public string Id
            {
                get => AssetSettings.Id;
                set => AssetSettings.Id = value;
            }

            public string Name
            {
                get => AssetSettings.Name;
                set => AssetSettings.Name = value;
            }

            
            public AssetService Build()
            {
                return new AssetService
                (
                    AssetSettings
                );
            }
        }
    }
}
