using System.Text.Json.Serialization;

namespace Leasing.Domain.Models.Events;

public record ContractCreatedEvent(
    int FacilityCode,
    int EquipmentCode,
    Guid EventId = default,
    DateTime CreatedAt = default) : IDomainEvent
{
    [JsonConstructor]
    public ContractCreatedEvent(int FacilityCode, int EquipmentCode)
       : this(FacilityCode, EquipmentCode, Guid.NewGuid(), DateTime.UtcNow)
    {
    }
};