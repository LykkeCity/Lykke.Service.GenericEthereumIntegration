using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.GenericEthereumIntegration.Common.Controllers
{
    public abstract class IsAliveControllerBase : IntegrationControllerBase
    {
#if DEBUG
        private const bool IsDebug = true;
#else
        private const bool IsDebug = false;
#endif

        private readonly IHealthService _healthService;

        protected IsAliveControllerBase(
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
                return StatusCode
                (
                    StatusCodes.Status500InternalServerError,
                    BlockchainErrorResponse.FromUnknownError(healthViloationMessage)
                );
            }


            return Ok(new BlockchainIsAliveResponse
            {
                Name = PlatformServices.Default.Application.ApplicationName,
                Version = PlatformServices.Default.Application.ApplicationVersion,
                Env = ProgramBase.EnvInfo,
                IsDebug = IsDebug,
                IssueIndicators = GetHealthIssues(),
                ContractVersion = new Version(1, 1, 0)
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
