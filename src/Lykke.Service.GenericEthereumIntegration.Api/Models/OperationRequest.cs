using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.Api.Models
{
    [UsedImplicitly]
    public class OperationRequest
    {
        [FromRoute, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public Guid OperationId { get; set; }
    }
}
