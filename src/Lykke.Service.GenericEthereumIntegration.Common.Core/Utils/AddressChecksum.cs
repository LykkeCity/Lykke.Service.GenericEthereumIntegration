using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Multiformats.Hash;
using Multiformats.Hash.Algorithms;

using static System.Char;


namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Utils
{
    public static class AddressChecksum
    {
        private static readonly Regex AddressExpression;

        static AddressChecksum()
        {
            AddressExpression = new Regex
            (
                @"^0x[0-9a-f]{40}$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled
            );
        }

        [Pure, NotNull]
        public static string Encode([NotNull] string address)
        {
            return EncodeAsync(address).Result;
        }
        
        [Pure, ItemNotNull]
        public static async Task<string> EncodeAsync([NotNull] string address)
        {
            #region Validation

            if (!ValidateBasicRequirements(address))
            {
                throw new ArgumentException("Should be in propert format.", nameof(address));
            }
            
            #endregion
            
            
            address = address.Remove(0, 2).ToLowerInvariant();

            var addressBytes = Encoding.UTF8.GetBytes(address);
            var caseMapBytes = (await Multihash.SumAsync<KECCAK_256>(addressBytes)).Digest;
                
            var addressBuilder = new StringBuilder("0x");
                
            for (var i = 0; i < 40; i++)
            {
                var addressChar = address[i];
                
                if (IsLetter(addressChar))
                {
                    var leftShift = i % 2 == 0 ? 7 : 3;
                    var shouldBeUpper = (caseMapBytes[i / 2] & (1 << leftShift)) != 0;

                    if (shouldBeUpper)
                    {
                        addressChar = ToUpper(addressChar);
                    }
                }
                    
                addressBuilder.Append(addressChar);
            }

            return addressBuilder.ToString();
        }
        
        [Pure]
        public static bool Validate([NotNull] string address)
        {
            return ValidateAsync(address).Result;
        }
        
        [Pure]
        public static async Task<bool> ValidateAsync([NotNull] string address)
        {
            return ValidateBasicRequirements(address)
                && await ValidateChecksumAsync(address);
        }

        private static bool ValidateBasicRequirements(string address)
        {
            return address.IsNotNullOrWhiteSpace()
                && address.Match(AddressExpression);
        }

        private static async Task<bool> ValidateChecksumAsync(string address)
        {
            address = address.Remove(0, 2);
        
            var addressBytes = Encoding.UTF8.GetBytes(address.ToLowerInvariant());
            var caseMapBytes = (await Multihash.SumAsync<KECCAK_256>(addressBytes)).Digest;
        
            for (var i = 0; i < 40; i++)
            {
                var addressChar = address[i];
        
                if (!IsLetter(addressChar))
                {
                    continue;
                }
        
                var leftShift = i % 2 == 0 ? 7 : 3;
                var shouldBeUpper = (caseMapBytes[i / 2] & (1 << leftShift)) != 0;
                var shouldBeLower = !shouldBeUpper;
        
                if (shouldBeUpper && IsLower(addressChar) ||
                    shouldBeLower && IsUpper(addressChar))
                {
                    return false;
                }
            }
        
            return true;
        }
    }
}
