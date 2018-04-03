using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.DTOs;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces
{
    public interface IWalletService
    {
        WalletDto CreateWallet();
    }
}
