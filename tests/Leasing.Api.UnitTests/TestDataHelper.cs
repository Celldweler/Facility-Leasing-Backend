using Leasing.Api.DTOs.Contract;

namespace Leasing.Api.UnitTests;

public static class TestDataHelper
{
    public static CreateContractDto CreateContractDto => new CreateContractDto(1, 1, 3);
}