using Leasing.Data.EntityConfigurations;
using Leasing.Data.Seed;
using Leasing.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Data.DataContext;

public class LeasingDataContext : DbContext, IUnitOfWork
{
    public LeasingDataContext(DbContextOptions<LeasingDataContext> options)
        : base(options)
    {
    }

    public DbSet<Contract> Contracts { get; set; }

    public DbSet<Equipment> Equipments { get; set; }

    public DbSet<ProductionFacility> Facilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EquipmentConfiguration());
        modelBuilder.ApplyConfiguration(new ProductionFacilityConfiguration());
        modelBuilder.ApplyConfiguration(new ContractConfiguration());

        modelBuilder.Seed();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
    }
}