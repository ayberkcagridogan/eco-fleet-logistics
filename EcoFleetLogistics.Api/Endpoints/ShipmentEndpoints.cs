using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcoFleetLogistics.Application.Shipments.Commands.ChangeShipmentStatus;
using EcoFleetLogistics.Application.Shipments.Commands.CreateShipment;
using EcoFleetLogistics.Application.Shipments.Commands.DeleteShipment;
using EcoFleetLogistics.Application.Shipments.Commands.UpdateShipment;
using EcoFleetLogistics.Application.Shipments.Queries.GetShipmentById;
using EcoFleetLogistics.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EcoFleetLogistics.Api.Endpoints
{
    public static class ShipmentEndpoints
    {
        public static void MapShipmentEndPoints(this IEndpointRouteBuilder app)
        {
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
            .RequireAuthorization(Policies.ManagementOnly)
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
                var command = new ChangeShipmentStatusCommand(id, request.NewStatus);
                var isSucces = await madiator.Send(command, cancellationToken);
                return isSucces 
                    ? Results.NoContent()
                    : Results.NotFound(new {Message = $"Shipment with Id {id} not found."});
            })
            .WithName("ChangeShipmentStatus")
            .RequireAuthorization(Policies.ManagementOnly)
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
            .RequireAuthorization(Policies.ManagementOnly)
            .WithOpenApi();


            //Shipment Soft Delete
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
            .RequireAuthorization(Policies.ManagementOnly)
            .WithOpenApi();
        }
    }
}