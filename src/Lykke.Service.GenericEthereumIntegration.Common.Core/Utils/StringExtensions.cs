using System.Text.RegularExpressions;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Utils
{
    public static class StringExtensions
    {
        private static readonly Regex HexStringExpression 
            = new Regex(@"^(?:0x){0,1}(?:[0-9a-fA-F][0-9a-fA-F])+$", RegexOptions.Compiled);
        
        
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhitespace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
        
        public static bool IsNotNullOrWhiteSpace(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotHexString(this string str)
        {
            return !str.Match(HexStringExpression);
        }

        public static bool Match(this string str, Regex expression)
        {
            return expression.IsMatch(str);
        }
    }
}
