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

app.UseHttpsRedirection();
#endregion

#region Shipment Endpoints
    #region Create Shipment Endpoint

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

#endregion

    #region  Shipment By Id Endpoint

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

#endregion

    #region Change Shipment Status Endpoint

    app.MapPatch("/api/shipments/{id:guid}/status", async (
        Guid id,
        [FromBody] ChangeShipmentStatusRequest request,
        ISender madiator,
        CancellationToken cancellationToken ) => 
        {
            var command = new ChangeShipmentStatusCommand(id, request.NewStatus);
            var isSucces = await madiator.Send(command, cancellationToken);
            return isSucces 
                ? Results.NoContent()
                : Results.NotFound(new {Message = $"Shipment with Id {id} not found."});
        })
    .WithName("ChangeShipmentStatus")
    .WithOpenApi();

#endregion

    #region Update Shipment ReceiverName and/or DestinationAddress Endpoint

    app.MapPut("/api/shipments/{id:guid}", async (
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
    .WithOpenApi();

    #endregion

    #region Shipment Soft Delete

    app.MapDelete("/api/shipments/{id:guid}", async (
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
    .WithOpenApi();

    #endregion
#endregion
app.Run();
