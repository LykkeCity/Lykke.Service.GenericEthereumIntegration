using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IAddressValidationService
    {
        Task<bool> ValidateAsync([NotNull] string address);
    }
}
