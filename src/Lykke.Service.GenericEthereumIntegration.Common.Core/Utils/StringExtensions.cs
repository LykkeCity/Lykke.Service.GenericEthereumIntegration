using System.Text.RegularExpressions;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Utils
{
    public static class StringExtensions
    {
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
        
        public static bool IsNotNullOrWhiteSpace(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static bool Match(this string str, Regex expression)
        {
            return expression.IsMatch(str);
        }
    }
}
