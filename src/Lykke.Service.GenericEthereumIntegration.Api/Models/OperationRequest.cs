using System;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.Api.Models
{
    public class OperationRequest
    {
        [FromRoute]
        public Guid OperationId { get; set; }
    }
}
