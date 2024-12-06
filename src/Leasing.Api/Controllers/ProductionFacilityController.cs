using Leasing.Api.Data;
using Leasing.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Api.Controllers;

[ApiController]
[Route("api/facilities")]
public class ProductionFacilityController(LeasingDataContext context) : ControllerBase
{
    /// <summary>
    /// Get all Equipments from database.
    /// </summary>
    /// <returns>All equipments.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductionFacility>))]
    public async Task<ActionResult<IEnumerable<ProductionFacility>>> GetEquipmentsAsync()
    {
        var facilities = await context.Facilities
            .AsNoTracking()
            .ToListAsync();

        return Ok(facilities);
    }
}