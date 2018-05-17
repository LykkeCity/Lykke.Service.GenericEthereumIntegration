using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.Common.Controllers
{
    public abstract class IntegrationControllerBase : Controller
    {
        protected IActionResult NotImplemented()
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}
