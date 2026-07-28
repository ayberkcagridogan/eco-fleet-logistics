using EcoFleetLogistics.Application.Common.Behaviors;
using EcoFleetLogistics.Application.Shipments.Commands.CreateShipment;
using EcoFleetLogistics.Application.Shipments.Queries.GetShipmentById;
using EcoFleetLogistics.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EcoFleetLogistics.Api.Middleware;
using EcoFleetLogistics.Domain.Shipments.Enums;
using EcoFleetLogistics.Application.Shipments.Commands.ChangeShipmentStatus;
using EcoFleetLogistics.Application.Shipments.Commands.UpdateShipment;
using EcoFleetLogistics.Application.Shipments.Commands.DeleteShipment;
using EcoFleetLogistics.Application;
using Serilog;
using EcoFleetLogistics.Infrastructure.Persistence;
using EcoFleetLogistics.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

#region Services & Dependencies (DI)
builder.Host.UseCustemSerilog();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddApplicationServices();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


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
#endregion

app.MapShipmentEndPoints();
app.MapAutEndPoints();
await app.MigrateDatabaseAsync();
app.Run();
