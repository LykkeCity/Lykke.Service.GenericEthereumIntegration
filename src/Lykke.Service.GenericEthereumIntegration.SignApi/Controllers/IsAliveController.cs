using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Controllers;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.GenericEthereumIntegration.SignApi.Controllers
{
    [PublicAPI, Route("api/isalive")]
    public class IsAliveController : IsAliveControllerBase
    {
        public IsAliveController(
            IHealthService healthService)
            : base(healthService)
        {

        }
    }
}
