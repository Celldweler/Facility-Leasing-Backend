using Leasing.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Leasing.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLeasingServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddTransient<IContractService, ContractService>();

        return services;
    }
}
