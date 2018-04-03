using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Services
{
    [UsedImplicitly]
    public class HealthService : IHealthService
    {
        public string GetHealthViolationMessage()
        {
            return null;
        }

        public IEnumerable<HealthIssue> GetHealthIssues()
        {
            var issues = new HealthIssueCollection();



            return issues;
        }
    }
}
