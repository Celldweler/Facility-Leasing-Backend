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

services.AddLeasingData(configuration, environment);
services.AddLeasingServices(configuration, environment);
services.AddLeasingInfrastructure(configuration, environment);

if (environment.IsProduction())
{
    configuration.AddAzureAppConfiguration(options =>
        options.Connect(configuration.GetConnectionString("AppConfiguration")!));

    services.Configure<ServiceBusOptions>(
        configuration.GetSection(ServiceBusOptions.Section));
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