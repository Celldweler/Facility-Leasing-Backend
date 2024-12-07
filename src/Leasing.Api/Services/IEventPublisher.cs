using Leasing.Api.Domain.Events;

namespace Leasing.Api.Services;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent;
}