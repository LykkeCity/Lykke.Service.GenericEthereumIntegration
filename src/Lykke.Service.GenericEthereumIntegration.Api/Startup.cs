using Autofac;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings;
using Lykke.Service.GenericEthereumIntegration.Api.Filters;
using Lykke.Service.GenericEthereumIntegration.Api.Modules;
using Lykke.Service.GenericEthereumIntegration.Api.Repositories.Modules;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Modules;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.GenericEthereumIntegration.Api
{
    [UsedImplicitly]
    public sealed class Startup : Common.StartupBase<AppSettings>
    {
        public Startup(IHostingEnvironment env) : base(env)
        {

        }

        protected override IReloadingManager<string> DbLogConnectionStringManager
            => AppSettings.Nested(x => x.GenericEthereumIntegrationApi.Db.LogsConnString);

        protected override string ServiceName
            => "GenericEthereumIntegrationApi";

        protected override string SlackNotificationsAzureQueueConnectionString
            => AppSettings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString;

        protected override string SlackNotificationsAzureQueueName
            => AppSettings.CurrentValue.SlackNotifications.AzureQueue.QueueName;


        protected override void ConfigureApp(IApplicationBuilder app)
        {
            
        }

        protected override void ConfigureMvc(IMvcBuilder builder)
        {
            builder
                .AddFluentValidation(cfg =>
                {
                    cfg.RegisterValidatorsFromAssemblyContaining<Startup>();
                });
        }

        protected override void ConfigureMvcOptions(MvcOptions options)
        {
            options.Filters.Add
            (
                new ExceptionFilter
                (
                    (typeof(BadRequestException), StatusCodes.Status400BadRequest),
                    (typeof(ConflictException), StatusCodes.Status409Conflict),
                    (typeof(ChaosException), StatusCodes.Status500InternalServerError)
                )
            );
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
                .RegisterModule(new GenericEthereumIntegrationApiModule(AppSettings.CurrentValue.GenericEthereumIntegrationApi))
                .RegisterModule(new RepositoriesModule(AppSettings.Nested(x => x.GenericEthereumIntegrationApi.Db.DataConnString), Log))
                .RegisterModule(new ServicesModule(AppSettings.CurrentValue.GenericEthereumIntegrationApi.RpcNode));
        }

        protected override void PreConfigureServices(IServiceCollection services)
        {
            
        }
    }
}
