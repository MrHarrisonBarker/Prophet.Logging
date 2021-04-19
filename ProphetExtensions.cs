using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Prophet.Logging
{
    public static class ProphetExtensions
    {
        public static ProphetBuilder AddProphet(this IServiceCollection services, ProphetConfig prophetConfig)
            => new ProphetBuilder(services, prophetConfig);

        public static ProphetBuilder AddProphet(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptions)
            => new ProphetBuilder(services, new ProphetConfig()
            {
                Topic = "prophet",
                BoostrapServers = "localhost:9092",
                DbContextOptions = dbContextOptions,
                Group = "prophet"
            });

        public static IHost EnsureProphet(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ProphetContext>();

            context.Database.EnsureCreated();

            return host;
        }
    }

    public class ProphetBuilder
    {
        public ProphetBuilder(IServiceCollection services, ProphetConfig prophetConfig)
        {
            services.AddDbContext<ProphetContext>(prophetConfig.DbContextOptions);
            services.AddHostedService(provider => new ProphetService(provider, prophetConfig.BoostrapServers, prophetConfig.Topic, prophetConfig.Group));
        }
    }
}