using JetBrains.Annotations;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces
{
    public interface ISignService
    {
        [NotNull]
        string SignTransaction([NotNull] string transactionHex, [NotNull] string privateKey);
    }
}
