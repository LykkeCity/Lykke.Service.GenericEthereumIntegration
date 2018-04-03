using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Common;
using Microsoft.AspNetCore.Hosting;

namespace Lykke.Service.GenericEthereumIntegration.Api
{
    [UsedImplicitly]
    internal sealed class Program : ProgramBase
    {
        private static async Task Main()
        {
            await RunAsync
            (
                builder => builder
                    .UseKestrel()
                    .UseUrls("http://*:5000")
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>()
                    .UseApplicationInsights()
            );
        }
    }
}
