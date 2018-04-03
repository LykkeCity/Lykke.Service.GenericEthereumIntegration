using Lykke.AzureStorage.Tables;

namespace Lykke.Service.GenericEthereumIntegration.Common.Repositories.Entities
{
    public class ObservableAddressEntity : AzureTableEntity
    {
        public string Address { get; set; }
    }
}
