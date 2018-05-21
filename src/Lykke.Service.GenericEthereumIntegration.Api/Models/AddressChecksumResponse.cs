using JetBrains.Annotations;

namespace Lykke.Service.GenericEthereumIntegration.Api.Models
{
    [PublicAPI]
    public class AddressChecksumResponse
    {
        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public string AddressWithChecksum { get; set; }
    }
}
