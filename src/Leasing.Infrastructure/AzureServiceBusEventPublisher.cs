using Azure.Messaging.ServiceBus;
using Leasing.Domain.Models.Events;
using Leasing.Services.Abstractions;
using System.Text.Json;

namespace Leasing.Infrastructure;

public class AzureServiceBusEventPublisher : IEventPublisher
{
    private readonly ServiceBusSender _serviceBusSender;

    public AzureServiceBusEventPublisher(
        ServiceBusSender serviceBusSender)
    {
        _serviceBusSender = serviceBusSender;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        var message = new ServiceBusMessage(JsonSerializer.Serialize(@event));

        await _serviceBusSender.SendMessageAsync(message);
    }
}
