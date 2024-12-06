using FluentValidation;
using Leasing.Api.DTOs.Contract;

namespace Leasing.Api.FluentValidation.Contract;

public class CreateContractDtoValidator : AbstractValidator<CreateContractDto>
{
    public CreateContractDtoValidator()
    {
        RuleFor(x => x.EquipmentQuantity)
            .GreaterThan(0)
            .WithMessage("EquipmentQuantity must be greater than 0");

        RuleFor(x => x.EquipmentCode)
            .GreaterThan(0)
            .WithMessage("EquipmentCode must be greater than 0");

        RuleFor(x => x.FacilityCode)
            .GreaterThan(0)
            .WithMessage("FacilityCode must be greater than 0");
    }
}