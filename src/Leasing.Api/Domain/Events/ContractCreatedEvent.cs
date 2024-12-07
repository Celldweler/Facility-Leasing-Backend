namespace Leasing.Api.Domain.Events;

public record ContractCreatedEvent(
    int FacilityCode,
    int EquipmentCode,
    Guid EventId = default,
    DateTime CreatedAt = default) : IDomainEvent
{
    public ContractCreatedEvent(int FacilityCode, int EquipmentCode)
       : this(FacilityCode, EquipmentCode, Guid.NewGuid(), DateTime.UtcNow)
    {
    }
};