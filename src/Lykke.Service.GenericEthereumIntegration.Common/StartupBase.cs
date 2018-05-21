using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.Logs.Slack;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Settings;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace Lykke.Service.GenericEthereumIntegration.Common
{
    public abstract class StartupBase<T>
        where T : AppSettingsBase
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable NotAccessedField.Global

        protected readonly IReloadingManager<T> AppSettings;
        protected readonly IHostingEnvironment Environment;

        protected IContainer AppContainer;
        protected ILog Log;

        // ReSharper restore NotAccessedField.Global
        // ReSharper restore MemberCanBePrivate.Global


        protected StartupBase(IHostingEnvironment env)
        {
            Environment = env;
            AppSettings = CreateConfiguration(env).LoadSettings<T>();
            IntegrationName = AppSettings.CurrentValue.Integration.Name;
        }


        // ReSharper disable UnusedParameter.Global

        protected abstract IReloadingManager<string> DbLogConnectionStringManager { get; }
        
        protected abstract string ServiceType { get; }

        
        protected virtual void ConfigureApp(IApplicationBuilder app)
        {
            
        }

        protected virtual void ConfigureMvc(IMvcBuilder builder)
        {
            
        }

        protected virtual void ConfigureMvcOptions(MvcOptions options)
        {
            
        }

        protected virtual void PostBuildContainer(IContainer container)
        {
            
        }

        protected virtual void PostConfigureServices(IServiceCollection services)
        {
            
        }

        protected virtual void PreBuildContainer(ContainerBuilder containerBuilder)
        {
            
        }

        protected virtual void PreConfigureServices(IServiceCollection services)
        {
            
        }

        // ReSharper restore UnusedParameter.Global

        private string IntegrationName { get; }

        private string ServiceName
            => $"{IntegrationName}{ServiceType}";
        

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app
                    .UseLykkeForwardedHeaders();

                app
                    .UseLykkeMiddleware(ServiceName, ex => new { Message = "Technical problem" });

                app
                    .UseMvc();

                app
                    .UseSwagger(c =>
                    {
                        c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
                    });

                app
                    .UseSwaggerUI(x =>
                    {
                        x.RoutePrefix = "swagger/ui";
                        x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    });

                app
                    .UseStaticFiles();

                ConfigureApp(app);

                appLifetime.ApplicationStarted
                    .Register(() => StartApplication().GetAwaiter().GetResult());

                appLifetime.ApplicationStopping
                    .Register(() => StopApplication().GetAwaiter().GetResult());

                appLifetime.ApplicationStopped
                    .Register(() => CleanUp().GetAwaiter().GetResult());
            }
            catch (Exception ex)
            {
                Log?
                    .WriteFatalErrorAsync(nameof(StartupBase), nameof(Configure), "", ex)
                    .GetAwaiter()
                    .GetResult();

                throw;
            }
        }
        
        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                PreConfigureServices(services);

                var mvcBuilder = services
                    .AddMvc(ConfigureMvcOptions)
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    });

                ConfigureMvc(mvcBuilder);

                services
                    .AddSwaggerGen(options =>
                    {
                        options.DefaultLykkeConfiguration("v1", ServiceName);
                    });



                PostConfigureServices(services);

                #region Build app container

                var appContainerBuilder = new ContainerBuilder();

                Log = CreateLogWithSlack(services);

                appContainerBuilder
                    .RegisterInstance(Log)
                    .As<ILog>()
                    .SingleInstance();

                PreBuildContainer(appContainerBuilder);

                appContainerBuilder.Populate(services);

                AppContainer = appContainerBuilder.Build();

                PostBuildContainer(AppContainer);

                #endregion

                return new AutofacServiceProvider(AppContainer);
            }
            catch (Exception e)
            {
                Log?
                    .WriteFatalErrorAsync(nameof(StartupBase), nameof(ConfigureServices), "", e)
                    .GetAwaiter()
                    .GetResult();

                throw;
            }
        }

        protected virtual async Task OnStartApplication()
        {
            await AppContainer
                .Resolve<IStartupManager>()
                .StartAsync();
        }

        protected virtual async Task OnStopApplication()
        {
            await AppContainer
                .Resolve<IShutdownManager>()
                .StopAsync();
        }

        private async Task StartApplication()
        {
            try
            {
                await OnStartApplication();

                await Log.WriteMonitorAsync("", "", "Started");
            }
            catch (Exception ex)
            {
                await Log.WriteFatalErrorAsync(nameof(StartupBase), nameof(StartApplication), "", ex);

                throw;
            }
        }

        private async Task StopApplication()
        {
            try
            {
                await OnStopApplication();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    await Log.WriteFatalErrorAsync(nameof(StartupBase), nameof(StopApplication), "", ex);
                }

                throw;
            }
        }
        
        private async Task CleanUp()
        {
            try
            {
                if (Log != null)
                {
                    await Log.WriteMonitorAsync("", "", "Terminating");
                }

                AppContainer.Dispose();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    await Log.WriteFatalErrorAsync(nameof(StartupBase), nameof(CleanUp), "", ex);

                    (Log as IDisposable)?.Dispose();
                }
                
                throw;
            }
        }
        
        private ILog CreateLogWithSlack(IServiceCollection services)
        {
            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            aggregateLogger.AddLog(consoleLogger);
            
            var dbLogConnectionString = DbLogConnectionStringManager.CurrentValue;

            if (string.IsNullOrEmpty(dbLogConnectionString))
            {
                consoleLogger
                    .WriteWarningAsync("Startup", nameof(CreateLogWithSlack), "Table logger is not initialized")
                    .Wait();

                return aggregateLogger;
            }

            if (dbLogConnectionString.StartsWith("${") && dbLogConnectionString.EndsWith("}"))
            {
                throw new InvalidOperationException($"LogsConnString {dbLogConnectionString} is not filled in settings");
            }
            
            var persistenceManager = new LykkeLogToAzureStoragePersistenceManager
            (
                AzureTableStorage<LogEntity>.Create(DbLogConnectionStringManager, $"{ServiceName}Log", consoleLogger),
                consoleLogger
            );


            var slackNotificationSettings = AppSettings.CurrentValue.SlackNotifications.AzureQueue;

            // Creating slack notification service, which logs own azure queue processing messages to aggregate log
            var slackService = services.UseSlackNotificationsSenderViaAzureQueue
            (
                new AzureQueueIntegration.AzureQueueSettings
                {
                    ConnectionString = slackNotificationSettings.ConnectionString,
                    QueueName = slackNotificationSettings.QueueName
                }, 
                aggregateLogger
            );

            var slackNotificationsManager = new LykkeLogToAzureSlackNotificationsManager(slackService, consoleLogger);

            // Creating azure storage logger, which logs own messages to concole log
            var azureStorageLogger = new LykkeLogToAzureStorage
            (
                persistenceManager,
                slackNotificationsManager,
                consoleLogger
            );

            azureStorageLogger.Start();

            aggregateLogger.AddLog(azureStorageLogger);

            var allMessagesSlackLogger = LykkeLogToSlack.Create
            (
                slackService,
                "BlockChainIntegration",
                LogLevel.All
            );

            aggregateLogger.AddLog(allMessagesSlackLogger);

            var importantMessagesSlackLogger = LykkeLogToSlack.Create
            (
                slackService,
                "BlockChainIntegrationImportantMessages",
                LogLevel.All ^ LogLevel.Info
            );

            aggregateLogger.AddLog(importantMessagesSlackLogger);

            return aggregateLogger;
        }

        private static IConfigurationRoot CreateConfiguration(IHostingEnvironment environment)
        {
            var configurationBuilder = new ConfigurationBuilder();

            return configurationBuilder
                .SetBasePath(environment.ContentRootPath)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
