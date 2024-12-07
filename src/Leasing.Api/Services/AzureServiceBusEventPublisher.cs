using Azure.Messaging.ServiceBus;
using Leasing.Api.Configuration;
using Leasing.Api.Domain.Events;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Leasing.Api.Services;

public class AzureServiceBusEventPublisher : IEventPublisher
{
    private ServiceBusOptions _serviceBusOptions;
    private ServiceBusClient _serviceBusClient;

    public AzureServiceBusEventPublisher(
        IOptions<ServiceBusOptions> options,
        ServiceBusClient serviceBusClient)
    {
        _serviceBusOptions = options.Value;
        _serviceBusClient = serviceBusClient;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        await using var serviceBusSender = _serviceBusClient.CreateSender(_serviceBusOptions.QueueName);

        var message = new ServiceBusMessage(JsonSerializer.Serialize(@event));
        await serviceBusSender.SendMessageAsync(message);
    }
}
