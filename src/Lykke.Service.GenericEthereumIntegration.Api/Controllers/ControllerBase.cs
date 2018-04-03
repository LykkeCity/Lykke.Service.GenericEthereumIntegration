using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.GenericEthereumIntegration.Api.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected IActionResult NotImplemented()
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}
