using Autofac;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.SignApi.Core.Settings;
using Lykke.Service.GenericEthereumIntegration.SignApi.Services.Modules;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.GenericEthereumIntegration.SignApi
{
    [UsedImplicitly]
    public sealed class Startup : Common.StartupBase<AppSettings>
    {
        public Startup(IHostingEnvironment env) 
            : base(env)
        {

        }


        protected override IReloadingManager<string> DbLogConnectionStringManager
            => AppSettings.Nested(x => x.GenericEthereumIntegrationSignApi.Db.LogsConnString);

        protected override string ServiceName
            => "GenericEthereumIntegrationSignApi";

        protected override string SlackNotificationsAzureQueueConnectionString
            => AppSettings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString;

        protected override string SlackNotificationsAzureQueueName
            => AppSettings.CurrentValue.SlackNotifications.AzureQueue.QueueName;


        protected override void ConfigureApp(IApplicationBuilder app)
        {

        }

        protected override void ConfigureMvc(IMvcBuilder builder)
        {

        }

        protected override void ConfigureMvcOptions(MvcOptions options)
        {

        }

        protected override void PostBuildContainer(IContainer container)
        {

        }

        protected override void PostConfigureServices(IServiceCollection services)
        {

        }

        protected override void PreBuildContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterModule<ServicesModule>();
        }

        protected override void PreConfigureServices(IServiceCollection services)
        {

        }
    }
}
