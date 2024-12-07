using AutoMapper;
using FluentValidation;
using Leasing.Domain.Exceptions;
using Leasing.Data.Repository;
using Leasing.Domain.Models;
using Leasing.Domain.Models.Events;
using Leasing.Services.DTOs.Contract;
using Leasing.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Leasing.Data.DataContext;

namespace Leasing.Services.Contracts;

public class ContractService : IContractService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;
    private readonly IContractRepository _contractRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IProductionFacilityRepository _productionFacilityRepository;
    private readonly IValidator<CreateContractDto> _validator;

    public ContractService(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        IValidator<CreateContractDto> validator,
        IContractRepository contractRepository,
        IEquipmentRepository equipmentRepository,
        IProductionFacilityRepository productionFacilityRepository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _eventPublisher = eventPublisher;
        _contractRepository = contractRepository;
        _equipmentRepository = equipmentRepository;
        _productionFacilityRepository = productionFacilityRepository;
    }

    public async Task<IReadOnlyList<ContractDto>> GetAllContractsAsync()
    {
        var contracts = await _unitOfWork.Contracts
            .Include(x => x.Equipment)
            .Include(x => x.Facility)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IReadOnlyList<ContractDto>>(contracts);
    }

    public async Task<ContractDto> CreateContractAsync(CreateContractDto createContractDto)
    {
        ArgumentNullException.ThrowIfNull(createContractDto);
        ThrowIfNotValidModel(createContractDto);

        // find facility
        var facility = await _productionFacilityRepository.GetByIdAsync(createContractDto.FacilityCode);
        NotFoundException.ThrowIfNull(facility, $"Facility with code {createContractDto.FacilityCode} not found.");

        // find equipment
        var equipment = await _equipmentRepository.GetByIdAsync(createContractDto.EquipmentCode);
        NotFoundException.ThrowIfNull(equipment, $"Equipment with code {createContractDto.EquipmentCode} not found.");

        var isFacilityHasEnoughArea = CheckIfFacilityHasEnoughArea(facility!.Area, equipment!.Area, createContractDto.EquipmentQuantity);
        if (!isFacilityHasEnoughArea)
        {
            throw new LeasingException($"Facility {facility.Name} does not have enough area.");
        }

        var newContract = _mapper.Map<Contract>(createContractDto);
        if (await _contractRepository.SameExists(newContract))
        {
            throw new LeasingException($"Contract already exists.");
        }

        await _contractRepository.AddAsync(newContract);
        await _eventPublisher.PublishAsync(
            new ContractCreatedEvent(newContract.FacilityCode, newContract.EquipmentCode));

        return _mapper.Map<ContractDto>(newContract);
    }

    private bool CheckIfFacilityHasEnoughArea(float facilityArea, float equipmentArea, int equipmentQuantity)
    {
        return facilityArea >= equipmentArea * equipmentQuantity;
    }

    private void ThrowIfNotValidModel(CreateContractDto createContractDto)
    {
        var validationResult = _validator.Validate(createContractDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }
}