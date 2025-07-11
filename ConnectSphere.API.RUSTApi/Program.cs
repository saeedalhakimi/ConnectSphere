using Asp.Versioning;
using ConnectSphere.API.Application.MediatoRs.Persons.CommandHandlers;
using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Common.IClocking;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.IRepositories;
using ConnectSphere.API.Infrastructure.Data.DataWrapperFactory;
using ConnectSphere.API.Infrastructure.Repositories;
using ConnectSphere.API.RUSTApi.Extensions;
using ConnectSphere.API.RUSTApi.Logging;
using ConnectSphere.API.RUSTApi.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration) // Load from appsettings.json
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("ApplicationName", "Nexus.Api");
});

// Add custom logging service
builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
builder.Services.AddSingleton<IDatabaseConnectionFactory, SqlDatabaseConnectionFactory>();
// clocking service
builder.Services.AddSingleton<ISystemClocking, SystemClocking>();
//builder.Services.AddScoped<IErrorHandlingService, ErrorHandlingService>();
//builder.Services.AddScoped<IPersonRepository, PersonRepository>();

builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

// Add Repositories and services 
builder.Services.AddRegistrationServices();

// Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// Register MediatR for CQRS pattern
builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreatePersonCommandHandler).Assembly));



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseSerilogRequestLogging(); // Logs HTTP requests
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.Items["CorrelationId"]?.ToString());
    };
});

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
