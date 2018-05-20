using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiformats.Hash;
using Multiformats.Hash.Algorithms;

using static System.Char;


namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Utils
{
    public static class AddressConverter
    {
        public static async Task<string> AddChecksumIfNecessaryAsync(string address)
        {
            if (address.All(IsLower) || address.All(IsUpper))
            {
                address = address.Remove(0, 2).ToLowerInvariant();
                
                var addressBytes = Encoding.UTF8.GetBytes(address);
                var caseMapBytes = (await Multihash.SumAsync<KECCAK_256>(addressBytes)) .Digest;
                
                var addressBuilder = new StringBuilder("0x");
                
                for (var i = 0; i < 40; i++)
                {
                    var addressChar = address[i];

                    if (!IsLetter(addressChar))
                    {
                        continue;
                    }

                    var leftShift = i % 2 == 0 ? 7 : 3;
                    var shouldBeUpper = (caseMapBytes[i / 2] & (1 << leftShift)) != 0;
                    
                    if (shouldBeUpper)
                    {
                        addressChar = ToUpperInvariant(addressChar);
                    }
                    
                    addressBuilder.Append(addressChar);
                }

                address = addressBuilder.ToString();
            }

            return address;
        }
    }
}
