using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.GenericEthereumIntegration.Common.Tests.Core.Utils
{
    [TestClass]
    public class AddressChecksumTests
    {
        [DataTestMethod]
        [DataRow("0x5aaeb6053f3e94c9b9a09f33669435e7ef1beaed", "0x5aAeb6053F3E94C9b9A09f33669435E7Ef1BeAed")]
        [DataRow("0xfb6916095ca1df60bb79ce92ce3ea74c37c5d359", "0xfB6916095ca1df60bB79Ce92cE3Ea74c37c5d359")]
        [DataRow("0xdbf03b407c01e7cd3cbea99509d93f8dddc8c6fb", "0xdbF03B407c01E7cD3CBea99509d93f8DDDC8C6FB")]
        [DataRow("0xd1220a0cf47c7b9be7a2e6ba89f429762e7b9adb", "0xD1220A0cf47c7B9Be7A2E6BA89F429762e7b9aDb")]
        [DataRow("0x5AAEB6053F3E94C9B9A09F33669435E7EF1BEAED", "0x5aAeb6053F3E94C9b9A09f33669435E7Ef1BeAed")]
        [DataRow("0xFB6916095CA1DF60BB79CE92CE3EA74C37C5D359", "0xfB6916095ca1df60bB79Ce92cE3Ea74c37c5d359")]
        [DataRow("0xDBF03B407C01E7CD3CBEA99509D93F8DDDC8C6FB", "0xdbF03B407c01E7cD3CBea99509d93f8DDDC8C6FB")]
        [DataRow("0xD1220A0CF47C7B9BE7A2E6BA89F429762E7B9ADB", "0xD1220A0cf47c7B9Be7A2E6BA89F429762e7b9aDb")]
        [DataRow("0x5aAeb6053F3E94C9b9A09f33669435E7Ef1BeAed", "0x5aAeb6053F3E94C9b9A09f33669435E7Ef1BeAed")]
        public async Task Encode_And_EncodeAsync__ExpectedResultReturned(string addressSample, string expectedResult)
        {
            var actualResult = AddressChecksum.Encode(addressSample);
            var actualAsyncResult = await AddressChecksum.EncodeAsync(addressSample);
            
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedResult, actualAsyncResult);
        }
        
        [DataTestMethod]
        [DataRow(null, false)]                                         // Null
        [DataRow("",   false)]                                         // Empty
        [DataRow(" ",  false)]                                         // Whitespace
        [DataRow("0xea674fdde714fd979de3edf0f56aa9716b898ec8", false)] // All characters are lowercase
        [DataRow("0xEA674FDDE714FD979DE3EDF0F56AA9716B898EC8", false)] // All characters areuppercase
        [DataRow("0xEA674fdDe714fd979de3EdF0F56aa9716B898EC8", false)] // Checksum is invalid
        [DataRow("ea674fdde714fd979de3edf0f56aa9716b898ec8",   false)] // Format is invalid
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8", true )] // Checksum is valid
        public async Task Validate_And_ValidateAsync__ExpectedResultReturned(string addressSample, bool expectedResult)
        {
            var actualResult = AddressChecksum.Validate(addressSample);
            var actualAsyncResult = await AddressChecksum.ValidateAsync(addressSample);

            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedResult, actualAsyncResult);
        }
    }
}
