using Leasing.Data.DataContext;
using Leasing.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Api.Controllers;

[ApiController]
[Route("api/equipments")]
public class EquipmentController(LeasingDataContext context) : ControllerBase
{
    /// <summary>
    /// Get all Equipments from database.
    /// </summary>
    /// <returns>All equipments.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Equipment>))]
    public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipmentsAsync()
    {
        var equipments = await context.Equipments
            .AsNoTracking()
            .ToListAsync();

        return Ok(equipments);
    }
}