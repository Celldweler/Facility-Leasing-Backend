using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using FluentValidation;
using FluentValidation.AspNetCore;
using Leasing.Api.Middleware;
using Leasing.Data;
using Leasing.Data.DataContext;
using Leasing.Domain.Common;
using Leasing.Domain.Configuration;
using Leasing.Infrastructure;
using Leasing.Services;
using Leasing.Services.FluentValidation.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;

services.AddControllers();
services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "Leasing.Api", Version = "1" });
    config.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Scheme = Constants.ApiKeyScheme,
        Type = SecuritySchemeType.ApiKey,
        Name = Constants.ApiKeyHeaderName,
        Description = "Authorization by x-api-key inside request's header",
    });
    
    var key = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    
    var requirement = new OpenApiSecurityRequirement{{ key, new List<string>()}};

    config.AddSecurityRequirement(requirement);
});

services.AddEndpointsApiExplorer();

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<CreateContractDtoValidator>();

if (environment.IsProduction())
{
    try
    {
        var keyVaultUrl = configuration["Azure:KeyVault:Url"];
        var keyVaultUri = new Uri(keyVaultUrl ?? throw new ArgumentNullException("VaultUri is missing in the configuration."));
        var credential = new ClientSecretCredential(
            configuration["Azure:KeyVault:TenantId"],
            configuration["Azure:KeyVault:ClientId"],
            configuration["Azure:KeyVault:ClientSecret"]);
        
        configuration.AddAzureKeyVault(
            keyVaultUri,
            credential,
            new AzureKeyVaultConfigurationOptions
            {
                ReloadInterval = TimeSpan.FromSeconds(30)
            });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to configure Azure Key Vault: {ex.Message}");
        throw;
    }
}

services.AddLeasingData(configuration, environment);
services.AddLeasingServices(configuration, environment);
services.AddLeasingInfrastructure(configuration, environment);
services.Configure<ServiceBusOptions>(
        configuration.GetSection(ServiceBusOptions.Section));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<LeasingDataContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

try
{
    context.Database.Migrate();
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occured during migration");
}

app.Run();