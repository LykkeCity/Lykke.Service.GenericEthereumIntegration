using System.Threading.Tasks;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
