using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.GenericEthereumIntegration.Common;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.GenericEthereumIntegration.Api.Controllers
{
    [PublicAPI, Route("api/isalive")]
    public class IsAliveController : ControllerBase
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


        [HttpGet]
        public IActionResult GetHealthStatus()
        {
            var healthViloationMessage = _healthService.GetHealthViolationMessage();
            if (healthViloationMessage != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
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
