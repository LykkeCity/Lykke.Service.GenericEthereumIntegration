using Autofac;
using FluentValidation.AspNetCore;
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

        protected override string ServiceType
            => "SignApi";


        protected override void ConfigureMvc(IMvcBuilder builder)
        {
            builder
                .AddFluentValidation(cfg =>
                {
                    cfg.RegisterValidatorsFromAssemblyContaining<Startup>();
                });
        }
        
        protected override void PreBuildContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterModule<ServicesModule>();
        }
    }
}
