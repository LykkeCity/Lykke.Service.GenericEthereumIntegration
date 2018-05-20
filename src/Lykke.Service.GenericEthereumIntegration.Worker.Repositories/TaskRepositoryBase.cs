﻿using System;
using System.Threading.Tasks;
using AzureStorage.Queue;
using Common;
using JetBrains.Annotations;
using Lykke.Service.GenericEthereumIntegration.Worker.Core.Repositories.Interfaces;
using MessagePack;
using Microsoft.WindowsAzure.Storage.Queue;


namespace Lykke.Service.GenericEthereumIntegration.Worker.Repositories
{
    public abstract class TaskRepositoryBase<T> : ITaskRepository<T>
        where T : class, new()
    {
        private readonly IQueueExt _queue;

        protected internal TaskRepositoryBase(
            IQueueExt queue)
        {
            ValidateGenericParameter();

            _queue = queue;
        }

        public async Task CompleteAsync(string completionToken)
        {
            var (messageId, popReceipt) = DeserializeObject<CompletionToken>(completionToken);

            await _queue.FinishRawMessageAsync(new CloudQueueMessage(messageId, popReceipt));
        }

        public async Task EnqueueAsync(T task)
        {
            await _queue.PutRawMessageAsync
            (
                SerializeObject(task)
            );
        }

        public async Task<(T Task, string CompletionToken)> TryGetAsync(TimeSpan visibilityTimeout)
        {
            var queueMessage = await _queue.GetRawMessageAsync((int) visibilityTimeout.TotalSeconds);

            if (queueMessage != null)
            {
                var task = DeserializeObject<T>(queueMessage.AsString);

                var token = SerializeObject(new CompletionToken
                {
                    MessageId = queueMessage.Id,
                    PopReceipt = queueMessage.PopReceipt
                });


                return (task, token);
            }
            else
            {
                return (null, null);
            }
        }

        private static string SerializeObject(object obj)
        {
            return MessagePackSerializer
                .Serialize(obj)
                .ToHexString();
        }

        private static TObj DeserializeObject<TObj>(string str)
        {
            return MessagePackSerializer
                .Deserialize<TObj>(Utils.HexToArray(str));
        }

        private static void ValidateGenericParameter()
        {
            var genericParameterType = typeof(T);
            var requiredAttributeType = typeof(MessagePackObjectAttribute);

            if (Attribute.GetCustomAttribute(genericParameterType, requiredAttributeType) == null)
            {
                throw new NotSupportedException($"Specified generic parameter type [{genericParameterType.Name}] is not marked with MessagePackObject attribute");
            }
        }


        [MessagePackObject]
        private class CompletionToken
        {
            [Key(0), UsedImplicitly(ImplicitUseKindFlags.Access)]
            public string MessageId { get; set; }

            [Key(1), UsedImplicitly(ImplicitUseKindFlags.Access)]
            public string PopReceipt { get; set; }

            public void Deconstruct(out string messageId, out string popReceipt)
            {
                messageId = MessageId;
                popReceipt = PopReceipt;
            }
        }
    }
}
