using System;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
            : base(message)
        {
        }
    }
}
