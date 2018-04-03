using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.GenericEthereumIntegration.Common
{
    public abstract class ProgramBase
    {
        public static string EnvInfo 
            => Environment.GetEnvironmentVariable("ENV_INFO");


        protected static async Task RunAsync(Func<IWebHostBuilder, IWebHostBuilder> configureWebHost)
        {
            Console.WriteLine($"{PlatformServices.Default.Application.ApplicationName} version {PlatformServices.Default.Application.ApplicationVersion}");

#if DEBUG
            Console.WriteLine("Is DEBUG");
#else
            Console.WriteLine("Is RELEASE");
#endif

            Console.WriteLine($"ENV_INFO: {EnvInfo}");

            try
            {
                var webHostBuilder = configureWebHost(new WebHostBuilder());
                var webHost = webHostBuilder.Build();

                await webHost.RunAsync();
            }
            catch (Exception e)
            {
                var delay = TimeSpan.FromMinutes(1);

                Console.WriteLine("Fatal error:");
                Console.WriteLine(e);
                Console.WriteLine();
                Console.WriteLine($"Process will be terminated in {delay}. Press any key to terminate immediately.");
                Console.WriteLine();

                await Task.WhenAny
                (
                    Task.Delay(delay),
                    Task.Run(() => { Console.ReadKey(true); })
                );
            }

            Console.WriteLine("Terminated");
        }
    }
}
