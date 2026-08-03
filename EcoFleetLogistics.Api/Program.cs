using EcoFleetLogistics.Infrastructure;
using EcoFleetLogistics.Api.Middleware;
using EcoFleetLogistics.Application;
using Serilog;
using EcoFleetLogistics.Infrastructure.Persistence;
using EcoFleetLogistics.Api.Endpoints;
using EcoFleetLogistics.Api.Extensions;
using EcoFleetLogistics.WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

#region Services & Dependencies (DI)
builder.Host.UseCustomSerilog();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddApplicationServices();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
#endregion

var app = builder.Build();

#region Middleware Pipeline

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseHttpsRedirection();
app.UseMiddleware<SecurityAuditMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

#endregion

app.MapShipmentEndPoints();
app.MapAutEndPoints();
app.MapUsersEndpoints();
app.MapCompanyEndpoints();
await app.MigrateDatabaseAsync();


app.Run();
