using System;
using System.Numerics;
using Lykke.AzureStorage.Tables;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;

namespace Lykke.Service.GenericEthereumIntegration.Common.Repositories.Entities
{
    public class TransactionEntity : AzureTableEntity
    {
        private BigInteger _amount;
        private BigInteger? _blockNumber;
        private DateTime? _broadcastedOn;
        private DateTime _builtOn;
        private DateTime? _completedOn;
        private BigInteger _fee;
        private BigInteger _gasPrice;
        private bool _includeFee;
        private BigInteger _nonce;
        private Guid _operationId;
        private TransactionState _state;

        public BigInteger Amount
        {
            get 
                => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;

                    MarkValueTypePropertyAsDirty(nameof(Amount));
                }
            }
        }

        public BigInteger? BlockNumber
        {
            get 
                => _blockNumber;
            set
            {
                if (_blockNumber != value)
                {
                    _blockNumber = value;

                    MarkValueTypePropertyAsDirty(nameof(BlockNumber));
                }
            }
        }

        public DateTime? BroadcastedOn
        {
            get 
                => _broadcastedOn;
            set
            {
                if (_broadcastedOn != value)
                {
                    _broadcastedOn = value;

                    MarkValueTypePropertyAsDirty(nameof(BroadcastedOn));
                }
            }
        }

        public DateTime BuiltOn
        {
            get 
                => _builtOn;
            set
            {
                if (_builtOn != value)
                {
                    _builtOn = value;

                    MarkValueTypePropertyAsDirty(nameof(BuiltOn));
                }
            }
        }

        public DateTime? CompletedOn
        {
            get 
                => _completedOn;
            set
            {
                if (_completedOn != value)
                {
                    _completedOn = value;

                    MarkValueTypePropertyAsDirty(nameof(CompletedOn));
                }
            }
        }

        public string Error { get; set; }

        public BigInteger Fee
        {
            get 
                => _fee;
            set
            {
                if (_fee != value)
                {
                    _fee = value;

                    MarkValueTypePropertyAsDirty(nameof(Fee));
                }
            }
        }

        public string FromAddress { get; set; }

        public BigInteger GasPrice
        {
            get 
                => _gasPrice;
            set
            {
                if (_gasPrice != value)
                {
                    _gasPrice = value;

                    MarkValueTypePropertyAsDirty(nameof(GasPrice));
                }
            }
        }

        public bool IncludeFee
        {
            get 
                => _includeFee;
            set
            {
                if (_includeFee != value)
                {
                    _includeFee = value;

                    MarkValueTypePropertyAsDirty(nameof(IncludeFee));
                }
            }
        }

        public BigInteger Nonce
        {
            get 
                => _nonce;
            set
            {
                if (_nonce != value)
                {
                    _nonce = value;

                    MarkValueTypePropertyAsDirty(nameof(Nonce));
                }
            }
        }

        public Guid OperationId
        {
            get 
                => _operationId;
            set
            {
                if (_operationId != value)
                {
                    _operationId = value;

                    MarkValueTypePropertyAsDirty(nameof(OperationId));
                }
            }
        }

        public string SignedTxData { get; set; }

        public string SignedTxHash { get; set; }

        public TransactionState State
        {
            get 
                => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;

                    MarkValueTypePropertyAsDirty(nameof(State));
                }
            }
        }

        public string ToAddress { get; set; }

        public string TxData { get; set; }
    }
}
