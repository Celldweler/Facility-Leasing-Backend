namespace Leasing.Api.Domain;

public class Equipment : IEntity
{
    public int Code { get; set; }

    public string Name { get; set; }

    public float Area { get; set; }

    public ICollection<Contract> Contracts { get; set; }
}