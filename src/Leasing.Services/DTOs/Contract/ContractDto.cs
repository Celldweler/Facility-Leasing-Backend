namespace Leasing.Services.DTOs.Contract;

public record ContractDto(
    string FacilityName,
    string EquipmentName,
    int EquipmentQuantity);