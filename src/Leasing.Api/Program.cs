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
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;

services.AddControllers();
services.AddSwaggerGen();
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

if(environment.IsProduction())
{
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