using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Settings;


namespace Lykke.Service.GenericEthereumIntegration.Worker
{
    [UsedImplicitly]
    internal sealed class Program : ProgramBase
    {
        private static Task Main()
            => RunAsync<Startup, AppSettings>();
    }
}
