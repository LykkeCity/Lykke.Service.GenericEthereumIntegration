using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Settings;

namespace Lykke.Service.GenericEthereumIntegration.SignApi
{
    [UsedImplicitly]
    internal sealed class Program : ProgramBase
    {
        private static Task Main()
            => RunAsync<Startup, AppSettings>();
    }
}
