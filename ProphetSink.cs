using System;
using System.Net;
using System.Text.Json;
using Confluent.Kafka;
using Serilog.Core;
using Serilog.Events;

namespace Prophet.Logging
{
    public class ProphetSink : ILogEventSink
    {
        private readonly string ServiceName;
        private readonly string Topic;
        private readonly Guid InstanceId;
        private readonly IProducer<Null, string> Producer;

        private readonly ProducerConfig ProducerConfig = new()
        {
            ClientId = Dns.GetHostName()
        };

        public ProphetSink(string serviceName, Guid instanceId, string bootstrapServers, string topic)
        {
            ProducerConfig.BootstrapServers = bootstrapServers;
            ServiceName = serviceName;
            InstanceId = instanceId;
            Topic = topic;
            Producer = new ProducerBuilder<Null, string>(ProducerConfig).Build();
        }

        public void Emit(LogEvent logEvent)
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

            Producer.Produce(Topic, new Message<Null, string> {Value = serialisedLog});

            Producer.Flush();
        }
    }
}