using Leasing.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Data.DataContext;

public interface IUnitOfWork
{
    public DbSet<Contract> Contracts { get; set; }

    public DbSet<Equipment> Equipments { get; set; }

    public DbSet<ProductionFacility> Facilities { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}