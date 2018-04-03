using System.Collections.Generic;
using System.Linq;
using System.Net;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.GenericEthereumIntegration.Common;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Controllers
{
    [Route("api/isalive")]
    public class IsAliveController : Controller
    {
#if DEBUG
        private const bool IsDebug = true;
#else
        private const bool IsDebug = false;
#endif


        private readonly IHealthService _healthService;

        public IsAliveController(
            IHealthService healthService)
        {
            _healthService = healthService;
        }

        /// <summary>
        ///    Should return some general service info. Used to check if the service is running.
        /// </summary>
        /// <response code="200">
        ///    Returns <see cref="IsAliveResponse"/> if service is healthy.
        /// </response>
        [HttpGet]
        public IActionResult GetHealthStatus()
        {
            var healthViloationMessage = _healthService.GetHealthViolationMessage();
            if (healthViloationMessage != null)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse
                {
                    ErrorMessage = $"Service is unhealthy: {healthViloationMessage}"
                });
            }


            return Ok(new IsAliveResponse
            {
                Name = PlatformServices.Default.Application.ApplicationName,
                Version = PlatformServices.Default.Application.ApplicationVersion,
                Env = ProgramBase.EnvInfo,
                IsDebug = IsDebug,
                IssueIndicators = GetHealthIssues()
            });
        }

        private IEnumerable<IsAliveResponse.IssueIndicator> GetHealthIssues()
        {
            return _healthService
                .GetHealthIssues()
                .Select(i => new IsAliveResponse.IssueIndicator
                {
                    Type = i.Type,
                    Value = i.Value
                });
        }
    }
}
