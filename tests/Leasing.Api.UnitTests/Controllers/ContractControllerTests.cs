using FluentAssertions;
using Leasing.Api.Controllers;
using Leasing.Services.Contracts;
using Leasing.Services.DTOs.Contract;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Leasing.Api.UnitTests.Controllers;

public class ContractControllerTests
{
    private IContractService _contractService;
    private ContractController _controller;

    [SetUp]
    public void Setup()
    {
        _contractService = Substitute.For<IContractService>();
        _controller = new ContractController(_contractService);
    }

    [Test]
    public async Task GetContractsAsync_ShouldReturnOkResultObject()
    {
        // Arrange
        _contractService.GetAllContractsAsync()
            .Returns(new List<ContractDto>().AsReadOnly());

        // Act
        var result = await _controller.GetContractsAsync();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        (result as OkObjectResult)?.Value.Should().BeAssignableTo<IReadOnlyList<ContractDto>>();
    }
}