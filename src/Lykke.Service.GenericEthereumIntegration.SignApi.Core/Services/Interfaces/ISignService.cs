using JetBrains.Annotations;

namespace Lykke.Service.GenericEthereumIntegration.SignApi.Core.Services.Interfaces
{
    public interface ISignService
    {
        [Pure, NotNull]
        string SignTransaction([NotNull] string transactionHex, [NotNull] string privateKey);
    }
}
