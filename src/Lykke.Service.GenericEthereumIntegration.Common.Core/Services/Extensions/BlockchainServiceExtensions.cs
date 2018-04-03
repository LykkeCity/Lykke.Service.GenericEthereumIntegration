using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Extensions
{
    public static class BlockchainServiceExtensions
    {
        public static async Task<bool> IsWalletAsync(this IBlockchainService service, string address)
        {
            return (await service.GetCodeAsync(address)) == "0x";
        }
    }
}
