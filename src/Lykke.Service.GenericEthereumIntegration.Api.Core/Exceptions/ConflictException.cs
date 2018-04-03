using System;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string message)
            : base(message)
        {
        }
    }
}
