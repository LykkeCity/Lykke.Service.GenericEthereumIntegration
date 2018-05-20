using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Domain;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces
{
    public interface IIndexationStateRepository
    {
        [ItemNotNull]
        Task<IndexationStateAggregate> GetOrCreateAsync();

        Task UpdateAsync([NotNull] IndexationStateAggregate aggregate);
    }
}
