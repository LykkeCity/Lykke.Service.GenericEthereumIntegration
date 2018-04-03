using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Domain;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;


namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles
{
    [UsedImplicitly]
    public class TransactionIndexerDispatcherRole : ITransactionIndexerDispatcherRole
    {
        private readonly IBlockchainService _blockchainService;
        private readonly IIndexedBlockRepository _indexedBlockRepository;
        private readonly IndexationStateAggregate _indexationState;
        private readonly IIndexationStateRepository _indexationStateRepository;


        private BigInteger[] _headBlockNumbersInBatch;
        
        public TransactionIndexerDispatcherRole(
            IBlockchainService blockchainService,
            IIndexedBlockRepository indexedBlockRepository,
            IIndexationStateRepository indexationStateRepository)
        {
            _blockchainService = blockchainService;
            _indexedBlockRepository = indexedBlockRepository;
            _indexationState = indexationStateRepository.GetOrCreateAsync().Result;
            _indexationStateRepository = indexationStateRepository;
            
            _headBlockNumbersInBatch = Array.Empty<BigInteger>();
        }


        public int RemainingBatchSize { get; private set; }

        
        public async Task<IEnumerable<IndexBlock>> BeginBatchIndexationAsync()
        {
            var nonIndexedBlockNumbers = new List<BigInteger>();
            var latestBlockNumberInState = _indexationState.LatestBlockNumber;
            var latestBlockNumberInBlockchain = await _blockchainService.GetLatestBlockNumberAsync();
            
            // TODO: Add confirmation level

            if (latestBlockNumberInState < latestBlockNumberInBlockchain)
            {
                _indexationState.Extend(to: latestBlockNumberInBlockchain);

                nonIndexedBlockNumbers = _indexationState.GetLatestNonIndexedBlockNumbers().ToList();

                _headBlockNumbersInBatch = nonIndexedBlockNumbers.Where(x => x >= latestBlockNumberInState).ToArray();
            }
            
            RemainingBatchSize = nonIndexedBlockNumbers.Count;
            
            return nonIndexedBlockNumbers.Select(x => new IndexBlock(x));
        }

        public async Task CompleteBatchIndexationAsync()
        {
            await _indexationStateRepository.UpdateAsync(_indexationState);
        }

        public async Task<(bool ForkDetected, BigInteger LatestTrustedBlockNumber)> DetectForkAsync()
        {
            var forkDetected = false;
            var latestTrustedBlockNumber = _indexationState.LatestBlockNumber;

            if (_headBlockNumbersInBatch.Any())
            {
                var headBlocks = (await _indexedBlockRepository.GetAsync(_headBlockNumbersInBatch))
                    .OrderBy(x => x.BlockNumber)
                    .ToList();

                var firstHeadBlock = headBlocks.First();
                var headParentBlockNumber = firstHeadBlock.BlockNumber - 1;
                var headParentBlock = await _indexedBlockRepository.TryGetAsync(headParentBlockNumber);

                if (headParentBlock == null || headParentBlock.BlockHash == firstHeadBlock.ParentHash)
                {
                    for (var i = 0; i < headBlocks.Count - 1; i++)
                    {
                        var parentBlock = headBlocks[i];
                        var childBlock = headBlocks[i + 1];

                        if (parentBlock.BlockHash != childBlock.ParentHash)
                        {
                            forkDetected = true;
                            latestTrustedBlockNumber = parentBlock.BlockNumber;
                            
                            break;
                        }
                    }
                }
                else
                {
                    forkDetected = true;
                    latestTrustedBlockNumber = 0;

                    for (var i = headParentBlockNumber; i > 0; i--)
                    {
                        var block = i == headParentBlockNumber
                            ? await _indexedBlockRepository.TryGetAsync(i)
                            : headParentBlock;

                        if (block != null)
                        {
                            var trustedHash = await _blockchainService.GetBlockHashAsync(i);

                            if (block.BlockHash != trustedHash)
                            {
                                continue;
                            }
                        }

                        latestTrustedBlockNumber = i;

                        break;
                    }
                }
            }
            
            return (forkDetected, latestTrustedBlockNumber);
        }
        
        public void MarkBlockAsIndexed(BigInteger blockNumber)
        {
            _indexationState.MarkBlockAsIndexed(blockNumber);

            RemainingBatchSize--;
        }

        public void ProcessFork(BigInteger latestTrustedBlockNumber)
        {
            _indexationState.ProcessFork(latestTrustedBlockNumber);
        }
    }
}
