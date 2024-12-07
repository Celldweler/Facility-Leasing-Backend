namespace Leasing.Domain.Models;

public class ProductionFacility : IEntity
{
    public int Code { get; set; }

    public string Name { get; set; }

    public float Area { get; set; }

    public ICollection<Contract> Contracts { get; set; }
}