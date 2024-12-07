using Leasing.Domain.Models.Events;

namespace Leasing.Services.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent;
}