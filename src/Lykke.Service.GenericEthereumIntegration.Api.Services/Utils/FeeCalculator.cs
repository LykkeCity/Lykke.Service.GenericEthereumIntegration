using System;
using System.Numerics;

namespace Lykke.Service.GenericEthereumIntegration.Api.Services.Utils
{
    internal static class FeeCalculator
    {
        public static BigInteger Calculate(BigInteger gasPrice, BigInteger gasAmount, decimal feeFactor)
        {
            var feeFactorBits = decimal.GetBits(feeFactor);
            var feeFactorMultiplier = new BigInteger(new decimal(feeFactorBits[0], feeFactorBits[1], feeFactorBits[2], false, 0));
            var decimalPlacesNumber = (int)BitConverter.GetBytes(feeFactorBits[3])[2];
            var feeFactorDivider = new BigInteger(Math.Pow(10, decimalPlacesNumber));
            var newGasPrice = gasPrice * feeFactorMultiplier / feeFactorDivider;
            
            if (newGasPrice > gasPrice)
            {
                return newGasPrice * gasAmount;
            }

            return (gasPrice + 1) * gasAmount;
        }
    }
}
