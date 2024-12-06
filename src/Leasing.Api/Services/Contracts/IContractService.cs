using Leasing.Api.DTOs.Contract;

namespace Leasing.Api.Services.Contracts;

public interface IContractService
{
    Task<IReadOnlyList<ContractDto>> GetAllContractsAsync();
    
    Task<ContractDto> CreateContractAsync(CreateContractDto createContractDto);
}