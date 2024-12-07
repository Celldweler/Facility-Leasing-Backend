using AutoMapper;
using Leasing.Domain.Models;
using Leasing.Services.DTOs.Contract;

namespace Leasing.Services.Mapping;

public class ContractMapper : Profile
{
    public ContractMapper()
    {
        CreateMap<Contract, ContractDto>()
            .ForMember(x => x.EquipmentName, o => o
                    .MapFrom(x => x.Equipment.Name))
            .ForMember(x => x.FacilityName, o => o
                .MapFrom(y => y.Facility.Name))
            .ReverseMap();

        CreateMap<CreateContractDto, Contract>()
            .ForMember(x => x.Equipment, o => o.Ignore())
            .ForMember(x => x.Facility, o => o.Ignore())
            .ReverseMap();
    }
}