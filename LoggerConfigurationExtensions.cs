using Serilog;
using Serilog.Configuration;

namespace Prophet.Logging
{
    public static class ProphetSinkExtensions
    {
        public static LoggerConfiguration Prophet(this LoggerSinkConfiguration loggerConfiguration, string serviceName, string bootstrapServers = "localhost:9092", string topic = "prophet")
        {
            return loggerConfiguration.Sink(new ProphetSink(serviceName, bootstrapServers, topic));
        }
    }
}