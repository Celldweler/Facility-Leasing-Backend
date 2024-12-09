using Leasing.Data.DataContext;
using Leasing.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Data.Repository;

public class ProductionFacilityRepository :
    EntityRepositoryBase<int, ProductionFacility>,
    IProductionFacilityRepository
{
    public ProductionFacilityRepository(LeasingDataContext context)
        : base(context)
    {
    }

    public override async Task<ProductionFacility> GetByIdAsync(int id)
        => await _dbEntities.FirstOrDefaultAsync(x => x.Code == id);
}