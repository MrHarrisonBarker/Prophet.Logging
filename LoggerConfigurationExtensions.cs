using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Sinks.PeriodicBatching;

namespace Prophet.Logging
{
    public static class ProphetSinkExtensions
    {
        public static LoggerConfiguration Prophet(this LoggerSinkConfiguration loggerConfiguration, string serviceName, Guid instanceId, string bootstrapServers = "localhost:9092",
            string topic = "prophet")
        {
            return loggerConfiguration.Sink(new ProphetSink(serviceName, instanceId, bootstrapServers, topic));
        }

        public static LoggerConfiguration ProphetAsync(this LoggerSinkConfiguration loggerConfiguration, string serviceName, Guid instanceId, string bootstrapServers = "localhost:9092",
            string topic = "prophet")
        {
            return ProphetAsync(loggerConfiguration, new PeriodicBatchingSinkOptions()
            {
                BatchSizeLimit = 100,
                Period = TimeSpan.FromSeconds(2),
                EagerlyEmitFirstEvent = true,
                QueueLimit = 10000
            }, serviceName, instanceId, bootstrapServers, topic);
        }

        public static LoggerConfiguration ProphetAsync(this LoggerSinkConfiguration loggerConfiguration, PeriodicBatchingSinkOptions batchingSinkOptions, string serviceName, Guid instanceId,
            string bootstrapServers = "localhost:9092",
            string topic = "prophet")
        {
            var batchedSink = new PeriodicBatchingSink(new BatchedProphetSink(serviceName, instanceId, bootstrapServers, topic), batchingSinkOptions);

            return loggerConfiguration.Sink(batchedSink);
        }
    }
}