using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Prophet.Logging
{
    public class ProphetService : BackgroundService
    {
        private readonly ConsumerConfig ConsumerConfig = new()
        {
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        private readonly string Topic;

        private readonly IServiceProvider ServiceProvider;

        public ProphetService(IServiceProvider serviceProvider, string bootstrapServers, string topic, string group)
        {
            ServiceProvider = serviceProvider;
            Topic = topic;
            ConsumerConfig.GroupId = group;
            ConsumerConfig.BootstrapServers = bootstrapServers;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build();

                consumer.Subscribe(Topic);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(cancellationToken);

                    if (result == null) continue;
                    
                    var log = JsonSerializer.Deserialize<Logg>(result.Message.Value);

                    if (log?.Exception != null) log.Error = new Error {Message = log.Exception.Message, StackTrace = log.Exception.StackTrace};

                    using var scope = ServiceProvider.CreateScope();
                    await using var context = scope.ServiceProvider.GetRequiredService<ProphetContext>();

                    context.Logs.Add(log!);

                    await context.SaveChangesAsync(cancellationToken);
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}