using Leasing.Services.DTOs.Contract;

namespace Leasing.Services.Contracts;

public interface IContractService
{
    Task<IReadOnlyList<ContractDto>> GetAllContractsAsync();

    Task<ContractDto> CreateContractAsync(CreateContractDto createContractDto);
}