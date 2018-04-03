using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Domain;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces
{
    public interface IIndexationStateRepository
    {
        Task<IndexationStateAggregate> GetOrCreateAsync();

        Task UpdateAsync(IndexationStateAggregate aggregate);
    }
}
