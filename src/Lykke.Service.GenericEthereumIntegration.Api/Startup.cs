using Autofac;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Exceptions;
using Lykke.Service.GenericEthereumIntegration.Api.Core.Settings;
using Lykke.Service.GenericEthereumIntegration.Api.Modules;
using Lykke.Service.GenericEthereumIntegration.Api.Repositories.Modules;
using Lykke.Service.GenericEthereumIntegration.Api.Services.Modules;
using Lykke.Service.GenericEthereumIntegration.Common.Filters;
using Lykke.SettingsReader;
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
            => AppSettings.Nested(x => x.Api.Db.LogsConnString);

        protected override string ServiceType
            => "Api";
        

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
                    (typeof(NotFoundException), StatusCodes.Status204NoContent)
                )
            );
        }

        protected override void PreBuildContainer(ContainerBuilder containerBuilder)
        {
            var apiSettings = AppSettings.CurrentValue.Api;
            var integrationSettings = AppSettings.CurrentValue.Integration;
            
            containerBuilder
                .RegisterModule(new GenericEthereumIntegrationApiModule(apiSettings, integrationSettings))
                .RegisterModule(new RepositoriesModule(AppSettings.Nested(x => x.Api.Db.DataConnString), Log))
                .RegisterModule(new ServicesModule(integrationSettings.RpcNode));
        }
    }
}
