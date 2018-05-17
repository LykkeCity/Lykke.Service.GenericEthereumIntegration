using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings;
using Lykke.Service.GenericEthereumIntegration.Common;

namespace Lykke.Service.GenericEthereumIntegration.Api
{
    [UsedImplicitly]
    internal sealed class Program : ProgramBase
    {
        private static Task Main()
            => RunAsync<Startup, AppSettings>();
    }
}
