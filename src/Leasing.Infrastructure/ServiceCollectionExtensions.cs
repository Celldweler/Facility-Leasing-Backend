using Azure.Messaging.ServiceBus;
using Leasing.Domain.Models.Events;
using Leasing.Infrastructure.BackgroundJobs;
using Leasing.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;

namespace Leasing.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLeasingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (environment.IsProduction())
        {
            services.AddSingleton<IEventPublisher, AzureServiceBusEventPublisher>();

            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                return new ServiceBusClient(config.GetConnectionString("AzureServiceBus"));
            });
        }
        else
        {
            services.AddHostedService<EventProccessorService>();
            services.AddSingleton<IEventPublisher, ChannelEventPublisher>();
            services.AddSingleton(_ => Channel.CreateUnbounded<ContractCreatedEvent>());
        }

        return services;
    }
}
