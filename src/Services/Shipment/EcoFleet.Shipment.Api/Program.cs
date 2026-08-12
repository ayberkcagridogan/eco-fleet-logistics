using EcoFleet.Shared.Kernel;
using EcoFleet.Shipment.Infrastructure;
using EcoFleet.Shipment.Application.Shipments.Commands.ChangeShipmentStatus;
using EcoFleet.Shipment.Application.Shipments.Commands.CreateShipment;
using EcoFleet.Shipment.Application.Shipments.Commands.DeleteShipment;
using EcoFleet.Shipment.Application.Shipments.Commands.UpdateShipment;
using EcoFleet.Shipment.Application.Shipments.Queries.GetShipmentById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EcoFleet.Shipment.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSharedKernel(builder);
builder.Services.AddShipmentInfrastructure(builder.Configuration);
builder.Services.AddShipmentApplication();
builder.Services.AddOpenApi();

builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "shipment-db-check",
        tags: new[] { "db", "ready" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var group = app.MapGroup("api/v1/shipments")
            .WithTags("Shipments");

//Create Shipment Endpoint
group.MapPost("", async (
[FromBody] CreateShipmentCommand command,
ISender madiator,
CancellationToken cancellationToken) =>
{
    var shipmentId = await madiator.Send(command, cancellationToken);
    return Results.Created($"/api/shipments/{shipmentId}", new { Id = shipmentId });
})
.WithName("CreateShipment")
//.RequireAuthorization(Policies.ManagementOnly)
.WithOpenApi();


//Shipment By Id Endpoint
group.MapGet("{id:guid}", async (
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
.RequireAuthorization()
.WithOpenApi();


//Change Shipment Status Endpoint
group.MapPatch("{id:guid}/status", async (
Guid id,
[FromBody] ChangeShipmentStatusRequest request,
ISender madiator,
CancellationToken cancellationToken ) => 
{
    var command = new ChangeShipmentStatusCommand(id, request.NewStatus, request.DriverId);
    var isSucces = await madiator.Send(command, cancellationToken);
    return isSucces 
        ? Results.NoContent()
        : Results.NotFound(new {Message = $"Shipment with Id {id} not found."});
})
.WithName("ChangeShipmentStatus")
//.RequireAuthorization(Policies.ManagementOnly)
.WithOpenApi();


//Update Shipment ReceiverName and/or DestinationAddress Endpoint
group.MapPut("{id:guid}", async (
Guid id,
[FromBody] UpdateShipmentRequest request,
ISender mediator,
CancellationToken cancellationToken) =>
{
    var command = new UpdateShipmentCommand(id, request.ReceiverName, request.DestinationAddress);
    var isSuccess = await mediator.Send(command, cancellationToken);
    return isSuccess 
        ? Results.NoContent() 
        : Results.NotFound(new { Message = $"Shipment with Id {id} not found." });
})
.WithName("UpdateShipment")
//.RequireAuthorization(Policies.ManagementOnly)
.WithOpenApi();


//Shipment Soft Delete
group.MapDelete("{id:guid}", async (
Guid id, 
IMediator mediator) =>
{
    var command = new DeleteShipmentCommand(id);
    var result = await mediator.Send(command);
    
    return result 
        ? Results.NoContent()
        : Results.NotFound(new { Message = $"Shipment with ID '{id}' was not found." });
})
.WithName("DeleteShipment")
//.RequireAuthorization(Policies.ManagementOnly)
.WithOpenApi();

app.UseSharedKernelMiddlewares();
app.UseSharedKernelEndpoints();

app.Run();

