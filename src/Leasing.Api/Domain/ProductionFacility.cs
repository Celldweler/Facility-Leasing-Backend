using System.ComponentModel.DataAnnotations;
using Leasing.Api.Common;

namespace Leasing.Api.Domain;

public class ProductionFacility : IEntity
{
    public int Code { get; set; }
    
    public string Name { get; set; }

    [Range(Constants.MinProductionFacilityArea, float.MaxValue, ErrorMessage = "Area must be greater than 1000")]
    public float Area { get; set; }
}