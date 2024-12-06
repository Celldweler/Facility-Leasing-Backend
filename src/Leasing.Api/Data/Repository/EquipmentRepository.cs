using Leasing.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Api.Data.Repository;

public class EquipmentRepository : EntityRepositoryBase<int, Equipment>, IEquipmentRepository
{
    public EquipmentRepository(LeasingDataContext context)
        : base(context)
    {
    }

    public override async Task<Equipment> GetByIdAsync(int id)
        => await _dbEntities.FirstOrDefaultAsync(x => x.Code == id);
}