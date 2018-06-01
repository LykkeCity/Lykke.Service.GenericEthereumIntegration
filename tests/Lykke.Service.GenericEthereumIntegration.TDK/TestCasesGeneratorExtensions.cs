using System;
using System.Collections.Generic;

namespace Lykke.Service.GenericEthereumIntegration.TDK
{
    public static class TestCasesGeneratorExtensions
    {
        public static TestCasesGenerator RegisterAddressParameter(
            this TestCasesGenerator generator,
            string name,
            bool addNullValue = true,
            bool addEmptyValue = true,
            bool addValidValue = true,
            bool addInvalidValues = true)
        {
            var values = new List<(string, bool)>();

            if (addNullValue)
            {
                values.Add((null, false));
            }

            if (addEmptyValue)
            {
                values.Add((string.Empty, false));
            }
            
            if (addInvalidValues)
            {
                values.Add(("0xea674fdde714fd979de3edf0f56aa9716b898ec8", false));
                values.Add(("0xEA674FDDE714FD979DE3EDF0F56AA9716B898EC8", false));
                values.Add(("0xEA674fdDe714fd979de3EdF0F56aa9716B898EC8", false));
                values.Add(("EA674fdDe714fd979de3EdF0F56aa9716B898EC8",   false));
            }
            
            if (addValidValue)
            {
                values.Add(("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8", true));
            }
            
            return generator
                .RegisterParameter(name, values);
        }
        
        public static TestCasesGenerator RegisterHexStringParameter(
            this TestCasesGenerator generator,
            string name,
            bool addNullValue = true,
            bool addEmptyValue = true,
            bool addValidValue = true,
            bool addInvalidValues = true)
        {
            var values = new List<(string, bool)>();

            if (addNullValue)
            {
                values.Add((null, false));
            }

            if (addEmptyValue)
            {
                values.Add((string.Empty, false));
            }

            if (addValidValue)
            {
                values.Add(($"0x{Guid.NewGuid():N}", true));
            }

            if (addInvalidValues)
            {
                values.Add(($"0x{Guid.NewGuid():N}".Substring(0, 33), false)); // Odd number of symbols
                values.Add(($"{Guid.NewGuid():N}", false));                    // No 0x prefix
                values.Add(($"0x{Guid.NewGuid()}", false));                    // Contains non-hex charactes
            }
            
            return generator
                .RegisterParameter(name, values);
        }
    }
}
