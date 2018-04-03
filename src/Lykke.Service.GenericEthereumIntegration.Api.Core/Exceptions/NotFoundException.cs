using System;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
