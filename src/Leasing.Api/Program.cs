using FluentValidation;
using FluentValidation.AspNetCore;
using Leasing.Api.Data;
using Leasing.Api.Data.Repository;
using Leasing.Api.FluentValidation.Contract;
using Leasing.Api.Middleware;
using Leasing.Api.Services.Contracts;
using Microsoft.EntityFrameworkCore;

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