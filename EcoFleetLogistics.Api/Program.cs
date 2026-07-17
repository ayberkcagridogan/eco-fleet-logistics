using EcoFleetLogistics.Application.Common.Behaviors;
using EcoFleetLogistics.Application.Shipments.Commands.CreateShipment;
using EcoFleetLogistics.Application.Shipments.Queries.GetShipmentById;
using EcoFleetLogistics.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddValidatorsFromAssembly(typeof(CreateShipmentCommandValidator).Assembly);
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateShipmentCommandHandler).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();


app.MapPost("/api/shipments", async (
    [FromBody] CreateShipmentCommand command,
    ISender madiator,
    CancellationToken cancellationToken) =>
{
    var shipmentId = await madiator.Send(command, cancellationToken);
    return Results.Created($"/api/shipments/{shipmentId}", new { Id = shipmentId });
})
.WithName("CreateShipment")
.WithOpenApi();

app.MapGet("/api/shipments/{id:guid}", async (
    Guid id,
    ISender madiator,
    CancellationToken cancellationToken) =>
{
    var query = new GetShipmentByIdQuery(id);
    var result = await madiator.Send(query, cancellationToken);
    return result is not null 
        ? Results.Ok(result) 
        : Results.NotFound(new {Message = $"Shipment with Id {id} not found."});
})
.WithName("GetShipmentById")
.WithOpenApi();

app.Run();
