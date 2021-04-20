using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Prophet.Logging
{
    public class BatchedProphetSink : IBatchedLogEventSink, IDisposable
    {
        private readonly string ServiceName;
        private readonly string Topic;
        private readonly Guid InstanceId;
        private readonly IProducer<Null, string> Producer;

        private readonly ProducerConfig ProducerConfig = new()
        {
            ClientId = Dns.GetHostName()
        };
        
        public BatchedProphetSink(string serviceName, Guid instanceId, string bootstrapServers, string topic)
        {
            ProducerConfig.BootstrapServers = bootstrapServers;
            ServiceName = serviceName;
            InstanceId = instanceId;
            Topic = topic;
            Producer = new ProducerBuilder<Null, string>(ProducerConfig).Build();
        }
        
        public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            foreach (var logEvent in batch)
            {
                var serialisedLog = JsonSerializer.Serialize(new Logg
                {
                    TraceId = Guid.NewGuid(),
                    Message = logEvent.RenderMessage(),
                    Exception = logEvent.Exception,
                    Timestamp = logEvent.Timestamp,
                    LogLevel = logEvent.Level,
                    Service = ServiceName,
                    InstanceId = InstanceId
                });

                await Producer.ProduceAsync(Topic, new Message<Null, string> {Value = serialisedLog});
            }

            Producer.Flush();
        }

        public async Task OnEmptyBatchAsync()
        {
            await EmitBatchAsync(Enumerable.Empty<LogEvent>());
        }

        public void Dispose()
        {
            Producer?.Flush();
            Producer?.Dispose();
        }
    }
}