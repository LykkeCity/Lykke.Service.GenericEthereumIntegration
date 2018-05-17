using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch;
using Akka.Event;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Log.Extensions;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Log
{
    [PublicAPI]
    public class LykkeLogger : ReceiveActor, IRequiresMessageQueue<ILoggerMessageQueueSemantics>
    {
        private static ILog _lykkeLog;


        public LykkeLogger()
        {
            if (_lykkeLog == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(LykkeLogger)} {nameof(Configure)} method should be called prior to actor system creation.");
            }


            Receive<InitializeLogger>(
                msg => ProcessMessage(msg));

            ReceiveAsync<LogEvent>(
                ProcessMessageAsync);

            SubscribeAndReceiveAsync<LykkeLogEvent>(
                ProcessMessageAsync);
        }

        public static void Configure(ILog log)
        {
            _lykkeLog = log;
        }


        private void ProcessMessage(InitializeLogger message)
        {
            Sender.Tell(new LoggerInitialized());
        }

        private async Task ProcessMessageAsync(LogEvent message)
        {
            await _lykkeLog.LogEventAsync(message);
        }

        private async Task ProcessMessageAsync(LykkeLogEvent message)
        {
            await _lykkeLog.LogEventAsync(message);
        }

        private void SubscribeAndReceiveAsync<T>(Func<T, Task> handler)
        {
            Context.System.EventStream.Subscribe<T>(Self);

            ReceiveAsync(handler);
        }
    }
}
