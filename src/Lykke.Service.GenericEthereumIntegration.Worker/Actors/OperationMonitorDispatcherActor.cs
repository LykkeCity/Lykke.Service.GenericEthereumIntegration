using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Akka.Actor;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Messages;
using Lykke.Service.GenericEthereumIntegration.Worker.Actors.Roles.Interfaces;

namespace Lykke.Service.GenericEthereumIntegration.Worker.Actors
{
    [UsedImplicitly]
    public class OperationMonitorDispatcherActor : ReceiveActor
    {
        private readonly IActorRef _operationMonitor;
        private readonly IOperationMonitorDispatcherRole _role;
        

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        public OperationMonitorDispatcherActor(
            IOperationMonitorDispatcherRole role)
        {
            _role = role;
            _operationMonitor = OperationMonitorActor.Create
            (
                parentContext: Context,
                nrOfInstances: role.InProgressOperationsLimit
            );
            
            Receive<TaskCompleted>(
                msg => ProcessMessage(msg));

            ReceiveAsync<ProcessNextTasks>(
                ProcessMessageAsync);
            
        }

        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly
            (
                initialDelay: TimeSpan.Zero,
                interval: TimeSpan.FromSeconds(5),
                receiver: Self,
                message: new ProcessNextTasks(),
                sender: Nobody.Instance
            );
        }

        protected override void PostRestart(Exception reason)
        {
            // This method should be overloaded to prevent PreStart() calls on actor's restarts
        }


        // ReSharper disable once UnusedParameter.Local
        private void ProcessMessage(TaskCompleted message)
        {
            _role.CompleteTaskProcessing();

            Self.Tell(new IndexNextBlocksBatch());
        }

        private async Task ProcessMessageAsync(ProcessNextTasks message)
        {
            var tasks = await _role.BeginNextTasksProcessingAsync();

            foreach (var task in tasks)
            {
                _operationMonitor.Tell(task);
            }
        }
    }
}
