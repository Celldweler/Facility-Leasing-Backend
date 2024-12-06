namespace Leasing.Api.DTOs.Contract;

public record ContractDto(
    string FacilityName,
    string EquipmentName,
    int EquipmentQuantity);