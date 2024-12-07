namespace Leasing.Services.DTOs.Contract;

public record CreateContractDto(
    int FacilityCode,
    int EquipmentCode,
    int EquipmentQuantity);