using System.Threading.Tasks;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces
{
    public interface ISignService
    {
        string SignTransaction(string transaction, string privateKey);
    }
}
