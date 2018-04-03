using System.Threading.Tasks;

namespace Lykke.Service.GenericEthereumIntegration.Api.Core.Services.Interfaces
{
    public interface IAddressValidationService
    {
        Task<bool> ValidateAsync(string address);
    }
}
