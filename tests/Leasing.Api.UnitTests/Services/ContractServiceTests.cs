using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Leasing.Data.DataContext;
using Leasing.Data.Repository;
using Leasing.Domain.Exceptions;
using Leasing.Domain.Models;
using Leasing.Services.Abstractions;
using Leasing.Services.Contracts;
using Leasing.Services.DTOs.Contract;
using Leasing.Services.FluentValidation.Contract;
using Leasing.Services.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace Leasing.Api.UnitTests.Services;

public class ContractServiceTests
{
    private IMapper _mapper;
    private IUnitOfWork _unitOfWork;
    private IEventPublisher _eventPublisher;
    private ILogger<ContractService> _logger;
    private ContractService _contractService;
    private IValidator<CreateContractDto> _validator;
    private IContractRepository _contractRepository;
    private IEquipmentRepository _equipmentRepository;
    private IProductionFacilityRepository _productionFacilityRepository;

    [SetUp]
    public void Setup()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _validator = new CreateContractDtoValidator();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _logger = Substitute.For<ILogger<ContractService>>();
        _contractRepository = Substitute.For<IContractRepository>();
        _equipmentRepository = Substitute.For<IEquipmentRepository>();
        _productionFacilityRepository = Substitute.For<IProductionFacilityRepository>();
        _mapper = new Mapper(new MapperConfiguration(options =>
            options.AddProfiles([
                new ContractMapper()
            ])));

        _contractService = new ContractService(
            _mapper,
            _unitOfWork,
            _eventPublisher,
            _logger,
            _contractRepository,
            _validator,
            _equipmentRepository,
            _productionFacilityRepository);
    }

    [Test]
    public async Task GetAllContractsAsync_ShouldReturnAllContracts()
    {
        // Arrange
        var mockDbSet = Array.Empty<Contract>().BuildMockDbSet();
        _unitOfWork.Contracts.Returns(mockDbSet);

        // Act
        var result = await _contractService.GetAllContractsAsync();

        // Arrange
        result.Should().NotBeNull();
        Assert.IsInstanceOf<IReadOnlyCollection<ContractDto>>(result);
    }

    [Test]
    public void CreateContractAsync_WhenCreateContractDto_Null_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _contractService.CreateContractAsync(null!));
    }

    [Test]
    [TestCase(0, 1, 1)]
    [TestCase(0, 0, 0)]
    [TestCase(-1, 3, 2)]
    public void CreateContractAsync_WhenCreateContractDto_NotValid_ThrowsValidationException(
        int facilityCode,
        int equipmentCode,
        int equipmentQuantity)
    {
        // Arrange
        var dto = new CreateContractDto(facilityCode, equipmentCode, equipmentQuantity);

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(
            async () => await _contractService.CreateContractAsync(dto));
    }

    [Test]
    public void CreateContractAsync_WhenFacilityWithGivenCodeNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new CreateContractDto(1, 1, 3);
        _productionFacilityRepository.GetByIdAsync(dto.FacilityCode)
            .Returns((ProductionFacility)null!);

        // Act & Arrange
        Assert.ThrowsAsync<NotFoundException>(
            async () => await _contractService.CreateContractAsync(dto));
    }

    [Test]
    public void CreateContractAsync_WhenEquipmentWithGivenCodeNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new CreateContractDto(1, 1, 3);

        _productionFacilityRepository.GetByIdAsync(dto.FacilityCode)
            .Returns((ProductionFacility)null!);

        _equipmentRepository.GetByIdAsync(dto.EquipmentCode)
            .Returns((Equipment)null!);

        // Act & Arrange
        Assert.ThrowsAsync<NotFoundException>(
            async () => await _contractService.CreateContractAsync(dto));
    }

    [Test]
    public void CreateContractAsync_WhenFacilityHasNotEnoughArea_ThrowsException()
    {
        // Arrange
        var dto = new CreateContractDto(1, 1, 3);

        _productionFacilityRepository.GetByIdAsync(dto.FacilityCode)
            .Returns(new ProductionFacility { Code = dto.FacilityCode, Name = "test", Area = 1200f, });

        _equipmentRepository.GetByIdAsync(dto.EquipmentCode)
            .Returns(new Equipment { Code = dto.EquipmentCode, Name = "test", Area = 500f, });

        // Act & Assert
        Assert.ThrowsAsync<LeasingException>(
            async () => await _contractService.CreateContractAsync(dto));
    }

    [Test]
    public void CreateContractAsync_WhenContractAlreadyExists_ThrowsException()
    {
        // Arrange
        var dto = new CreateContractDto(1, 1, 3);

        _productionFacilityRepository.GetByIdAsync(dto.FacilityCode)
            .Returns(new ProductionFacility { Code = dto.FacilityCode, Name = "test", Area = 1200f, });

        _equipmentRepository.GetByIdAsync(dto.EquipmentCode)
            .Returns(new Equipment { Code = dto.EquipmentCode, Name = "test", Area = 500f, });

        _contractRepository.SameExists(Arg.Any<Contract>()).Returns(true);

        // Act & Assert
        Assert.ThrowsAsync<LeasingException>(async () => await _contractService.CreateContractAsync(dto));
    }


    [Test]
    public async Task CreateContractAsync()
    {
        // Assert
        var dto = TestDataHelper.CreateContractDto;

        _productionFacilityRepository.GetByIdAsync(dto.FacilityCode)
            .Returns(new ProductionFacility { Code = dto.FacilityCode, Name = "test", Area = 1200f, });

        _equipmentRepository.GetByIdAsync(dto.EquipmentCode)
            .Returns(new Equipment { Code = dto.EquipmentCode, Name = "test", Area = 200f, });

        _contractRepository.SameExists(Arg.Any<Contract>()).Returns(false);

        // Act
        var result = await _contractService.CreateContractAsync(dto);

        // Assert
        result.Should().NotBeNull();
    }

    private static DbSet<T> CreateEmptyDbSet<T>()
        where T : class
    {
        return Array.Empty<T>().BuildMockDbSet();
    }

    private static DbSet<T> CreateDbSetFromList<T>(IEnumerable<T> items)
        where T : class
    {
        return items.BuildMockDbSet();
    }
}