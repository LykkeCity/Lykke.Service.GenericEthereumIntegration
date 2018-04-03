using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles.Interfaces
{
    public interface ITransactionIndexerDispatcherRole : IActorRole
    {
        int RemainingBatchSize { get; }

        Task<IEnumerable<IndexBlock>> BeginBatchIndexationAsync();

        Task CompleteBatchIndexationAsync();
        
        Task<(bool ForkDetected, BigInteger LatestTrustedBlockNumber)> DetectForkAsync();
        
        void MarkBlockAsIndexed(BigInteger blockNumber);

        void ProcessFork(BigInteger latestTrustedBlockNumber);
    }
}
