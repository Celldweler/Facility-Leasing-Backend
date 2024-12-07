namespace Leasing.Api.Domain.Events;

public interface IDomainEvent
{
    public Guid EventId { get; init; }
    
    public DateTime CreatedAt { get; init; }
}