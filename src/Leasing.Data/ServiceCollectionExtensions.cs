using Leasing.Data.DataContext;
using Leasing.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Leasing.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLeasingData(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddDbContext<LeasingDataContext>(x =>
            x.UseSqlServer(configuration
                .GetConnectionString(
                    environment.IsDevelopment() ? "SqlServer" : "AzureSQL")));

        services.AddTransient<IContractRepository, ContractRepository>();
        services.AddTransient<IEquipmentRepository, EquipmentRepository>();
        services.AddTransient<IProductionFacilityRepository, ProductionFacilityRepository>();
        
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<LeasingDataContext>());

        return services;
    }
}
