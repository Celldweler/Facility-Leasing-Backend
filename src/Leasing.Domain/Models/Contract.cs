namespace Leasing.Domain.Models;

public class Contract : IEntity
{
    public ProductionFacility Facility { get; set; }

    public int FacilityCode { get; set; }

    public Equipment Equipment { get; set; }

    public int EquipmentCode { get; set; }

    public int EquipmentQuantity { get; set; }
}