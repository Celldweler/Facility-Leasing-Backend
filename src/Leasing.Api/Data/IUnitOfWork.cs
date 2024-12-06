using Leasing.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Api.Data;

public interface IUnitOfWork
{
    public DbSet<Contract> Contracts { get; set; }

    public DbSet<Equipment> Equipments { get; set; }

    public DbSet<ProductionFacility> Facilities { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}