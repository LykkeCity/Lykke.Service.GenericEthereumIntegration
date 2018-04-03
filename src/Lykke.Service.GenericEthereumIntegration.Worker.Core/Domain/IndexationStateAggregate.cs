using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using MessagePack;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Core.Domain
{
    public class IndexationStateAggregate
    {
        private readonly List<Range> _ranges;


        internal IndexationStateAggregate()
            : this(new [] { new Range() })
        {

        }

        internal IndexationStateAggregate(
            IEnumerable<Range> ranges)
        {
            _ranges = ranges.ToList();
        }


        public BigInteger LatestBlockNumber
            => _ranges.Last().Max;

        public IEnumerable<Range> Ranges
            => _ranges;

        
        public void Extend(BigInteger to)
        {
            if (to <= LatestBlockNumber)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(to),
                    $"Value should be greater then last block number [{LatestBlockNumber}]."
                );
            }

            var lastRange = _ranges.Last();

            if (lastRange.Indexed)
            {
                _ranges.Add(new Range
                {
                    Min = lastRange.Max + 1,
                    Max = to
                });
            }
            else
            {
                lastRange.Max = to;
            }
        }

        /// <summary>
        ///    Get latest non indexed block numbers.
        /// </summary>
        /// <returns>
        ///    A list of latest non indexed block number in reverse order.
        /// </returns>
        [Pure]
        public IEnumerable<BigInteger> GetLatestNonIndexedBlockNumbers()
        {
            for (var i = _ranges.Count - 1; i >= 0; i--)
            {
                var range = _ranges[i];

                if (!range.Indexed)
                {
                    for (var j = range.Max; j >= range.Min; j--)
                    {
                        yield return j;
                    }
                }
            }
        }

        public void MarkBlockAsIndexed(BigInteger blockNumber)
        {
            var containingRange = FindContainingRange(blockNumber);

            if (containingRange == null)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(blockNumber),
                    $"Specified block number [{blockNumber}] if out of the indexation range [0..{LatestBlockNumber}]"
                );
            }

            if (containingRange.Indexed)
            {
                return;
            }


            var containingRangeIndex = _ranges.IndexOf(containingRange);
            var newRanges = SplitRange(containingRange, blockNumber);

            _ranges.InsertRange(containingRangeIndex, newRanges);
            _ranges.Remove(containingRange);


            var leftRange = newRanges.First();
            if (leftRange != _ranges.First())
            {
                var leftRangeIndex = _ranges.IndexOf(leftRange);
                var leftLeftRange = _ranges[leftRangeIndex - 1];

                if (leftRange.Indexed == leftLeftRange.Indexed)
                {
                    leftRange.Min = leftLeftRange.Min;

                    _ranges.Remove(leftLeftRange);
                }
            }


            var rightRange = newRanges.Last();
            if (rightRange != _ranges.Last())
            {
                var rightRangeIndex = _ranges.IndexOf(rightRange);
                var rightRightRange = _ranges[rightRangeIndex + 1];


                if (rightRange.Indexed == rightRightRange.Indexed)
                {
                    rightRange.Max = rightRightRange.Max;

                    _ranges.Remove(rightRightRange);
                }
            }
        }

        /// <summary>
        ///    Removes forked blocks from the state.
        /// </summary>
        /// <param name="from">
        ///    Last non-forked block number.
        /// </param>
        public void ProcessFork(BigInteger from)
        {
            if (from > LatestBlockNumber)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(from),
                    $"Value should be lower then the last block number [{LatestBlockNumber}]."
                );
            }

            do
            {

                var lastRange = _ranges.Last();

                if (!lastRange.Contains(from))
                {
                    _ranges.Remove(lastRange);
                }
                else
                {
                    lastRange.Max = from;

                    break;
                }

            } while (true);
        }
        
        [Pure]
        private Range FindContainingRange(BigInteger blockNumber)
        {
            var min = 0;
            var max = _ranges.Count;

            do
            {

                var mid = (min + max) / 2;

                if (blockNumber > _ranges[mid].Max)
                {
                    min = mid + 1;
                }
                else if (blockNumber < _ranges[mid].Min)
                {
                    max = mid - 1;
                }
                else
                {
                    return _ranges[mid];
                }

            } while (min <= max);

            return null;
        }

        [Pure]
        private static IList<Range> SplitRange(Range range, BigInteger indexedBlockNumber)
        {
            var result = new List<Range>
            {
                new Range
                {
                    Indexed = true,
                    Min = indexedBlockNumber,
                    Max = indexedBlockNumber
                }
            };

            if (indexedBlockNumber != range.Min)
            {
                result.Insert(0, new Range
                {
                    Min = range.Min,
                    Max = indexedBlockNumber - 1
                });
            }

            if (indexedBlockNumber != range.Max)
            {
                result.Insert(result.Count, new Range
                {
                    Min = indexedBlockNumber + 1,
                    Max = range.Max
                });
            }

            return result;
        }


        [MessagePackObject]
        public class Range
        {
            [Key(0)]
            public bool Indexed { get; internal set; }

            [Key(1)]
            public BigInteger Max { get; internal set; }

            [Key(2)]
            public BigInteger Min { get; internal set; }


            public bool Contains(BigInteger blockNumber)
            {
                return blockNumber >= Min
                       && blockNumber <= Max;
            }
        }
    }
}
