using Leasing.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Api.Data.Repository;

public class ContractRepository :
    EntityRepositoryBase<(int facilityCode, int equipmentCode), Contract>,
    IContractRepository
{
    public ContractRepository(LeasingDataContext context)
        : base(context)
    {
    }
    
    public override async Task<Contract> GetByIdAsync((int facilityCode, int equipmentCode) id)
        => (await _dbEntities
            .FirstOrDefaultAsync(x => x.FacilityCode.Equals(id.facilityCode) &&
                                      x.EquipmentCode.Equals(id.equipmentCode)))!;

    public async Task<bool> SameExists(Contract entity) =>
        await _dbEntities.AnyAsync(x => x.FacilityCode.Equals(entity.FacilityCode) &&
                                                x.EquipmentCode.Equals(entity.EquipmentCode));
}