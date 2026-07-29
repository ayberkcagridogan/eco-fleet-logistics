using System.Security.Claims;
using EcoFleetLogistics.Application.Authentication.Commands.Login;
using EcoFleetLogistics.Application.Authentication.Commands.Logout;
using EcoFleetLogistics.Application.Authentication.Commands.RefreshToken;
using EcoFleetLogistics.Application.Authentication.Commands.Register;
using EcoFleetLogistics.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EcoFleetLogistics.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAutEndPoints(this IEndpointRouteBuilder app)
    {
            var group = app.MapGroup("api/auth")
                            .WithTags("Authentication");
        
            group.MapPost("/register", async (RegisterCommand command, ISender meditor, CancellationToken ct) =>
            {
                var result = await meditor.Send(command, ct);
                return Results.Created($"/api/users/{result.Id}", result);
            })
            .WithName("Register")
            .RequireAuthorization(Policies.ManagementOnly)
            .WithOpenApi();

            
            group.MapPost("/login", async (LoginCommand command, ISender mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return Results.Ok(result);
            })
            .WithName("Login")
            .AllowAnonymous()
            .WithOpenApi();

            group.MapPost("/refresh-token", async (RefreshTokenCommand command, ISender meditor, CancellationToken ct) =>
            {
                var result = await meditor.Send(command, ct);
                return Results.Created($"/api/users/{result.Id}", result);
            })
            .WithName("RefreshToken")
            .AllowAnonymous()
            .WithOpenApi();

            group.MapPost("logout", async (
                [FromBody] LogoutRequest request,
                ClaimsPrincipal user,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if(!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();
                
                var command = new LogoutCommand(userId, request.RefreshToken);

                await sender.Send(command, cancellationToken);

                return Results.Ok(new { message = "The session was successfully closed."});
            })
            .RequireAuthorization()
            .WithName("Logout");
        }
}