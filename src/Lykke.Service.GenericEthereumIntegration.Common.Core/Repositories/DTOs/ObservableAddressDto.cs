namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Repositories.DTOs
{
    public class ObservableAddressDto
    {
        public string Address { get; set; }

        public bool ObserveIncomingTransactions { get; set; }

        public bool ObserveOutgoingTransactions { get; set; }
    }
}
