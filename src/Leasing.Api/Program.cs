using Azure.Messaging.ServiceBus;
using FluentValidation;
using FluentValidation.AspNetCore;
using Leasing.Api.BackgroundJobs;
using Leasing.Api.Configuration;
using Leasing.Api.Data;
using Leasing.Api.Data.Repository;
using Leasing.Api.Domain.Events;
using Leasing.Api.FluentValidation.Contract;
using Leasing.Api.Middleware;
using Leasing.Api.Services;
using Leasing.Api.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
using Leasing.Api.Common;
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

services.AddDbContext<LeasingDataContext>(x =>
    x.UseSqlServer(configuration
        .GetConnectionString(
            environment.IsDevelopment() ? "SqlServer" : "AzureSQL")));

services.AddTransient<IContractService, ContractService>();
services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<LeasingDataContext>());

services.AddTransient<IContractRepository, ContractRepository>();
services.AddTransient<IEquipmentRepository, EquipmentRepository>();
services.AddTransient<IProductionFacilityRepository, ProductionFacilityRepository>();


if (environment.IsProduction())
{
    configuration.AddAzureAppConfiguration(options =>
    options.Connect(configuration.GetConnectionString("AppConfiguration")!));

    services.AddSingleton<IEventPublisher, AzureServiceBusEventPublisher>();

    services.Configure<ServiceBusOptions>(configuration.GetSection(ServiceBusOptions.Section));
    services.AddSingleton(sp =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        return new ServiceBusClient(config.GetConnectionString("AzureServiceBus"));
    });
}
else
{
    services.AddHostedService<EventProccessorService>();
    services.AddSingleton<IEventPublisher, ChannelEventPublisher>();
    services.AddSingleton(_ => Channel.CreateUnbounded<ContractCreatedEvent>());
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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