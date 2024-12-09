using Azure.Messaging.ServiceBus;
using Leasing.Domain.Models.Events;
using Leasing.Infrastructure.BackgroundJobs;
using Leasing.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Leasing.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLeasingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddSingleton<IEventPublisher, AzureServiceBusEventPublisher>();
        services.AddSingleton<IMessageHandler<ContractCreatedEvent>, ContractCreatedEventHandler>();

        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            return new ServiceBusClient(config.GetConnectionString("AzureServiceBus"));
        });


        services.AddHostedService<ServiceBusProccessorJob<ContractCreatedEvent>>();

        // Add AzureBus Processor
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<ServiceBusClient>();
            var queue = configuration["Azure:ServiceBusOptions:QueueName"];

            return client.CreateProcessor(queue);
        });

        // Add ServiceBus Sender
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<ServiceBusClient>();
            var queue = configuration["Azure:ServiceBusOptions:QueueName"];

            return client.CreateSender(queue);
        });

        return services;
    }
}
