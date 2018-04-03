using System.Numerics;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs
{
    public class ObservableBalanceDto
    {
        public string Address { get; set; }

        public BigInteger Amount { get; set; }

        public BigInteger BlockNumber { get; set; }
    }
}
