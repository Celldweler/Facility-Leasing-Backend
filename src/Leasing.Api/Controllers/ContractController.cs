using Leasing.Api.DTOs.Contract;
using Leasing.Api.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Leasing.Api.Controllers;

[ApiController]
[Route("api/contracts")]
public class ContractController(IContractService contractService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<ContractDto>))]
    public async Task<IActionResult> GetContractsAsync() =>
        Ok(await contractService.GetAllContractsAsync());

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContractDto))]
    public async Task<IActionResult> CreateContractAsync([FromBody] CreateContractDto createContractDto)
    {
        var contractDto = await contractService.CreateContractAsync(createContractDto);

        return Ok(contractDto);
    }
}