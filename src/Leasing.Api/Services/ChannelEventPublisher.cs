using Leasing.Api.Domain.Events;
using System.Threading.Channels;

namespace Leasing.Api.Services;

public class ChannelEventPublisher : IEventPublisher
{
    private IServiceProvider _serviceProvider;

    public ChannelEventPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var channel = scope.ServiceProvider.GetRequiredService<Channel<TEvent>>();

        await channel.Writer.WriteAsync(@event);
    }
}
