using System.Collections.Generic;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces
{
    public interface IHealthService
    {
        string GetHealthViolationMessage();

        IEnumerable<HealthIssue> GetHealthIssues();
    }
}
