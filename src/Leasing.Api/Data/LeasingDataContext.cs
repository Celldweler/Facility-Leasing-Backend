using Leasing.Api.Data.EntityConfigurations;
using Leasing.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Api.Data;

public class LeasingDataContext : DbContext
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
    }
}