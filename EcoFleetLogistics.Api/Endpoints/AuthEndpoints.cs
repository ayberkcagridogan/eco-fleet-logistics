using EcoFleetLogistics.Application.Authentication.Commands.Login;
using EcoFleetLogistics.Application.Authentication.Commands.Register;
using EcoFleetLogistics.Application.Authentication.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EcoFleetLogistics.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAutEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth")
                        .WithTags("Authentication")
                        .AllowAnonymous();
        
        group.MapPost("/register", async (RegisterCommand command, ISender meditor, CancellationToken ct) =>
        {
            var result = await meditor.Send(command, ct);
            return Results.Created($"/api/users/{result.Id}", result);
        })
        .WithName("Register")
        .WithOpenApi();

        //Add Admin Autherize
        group.MapPost("/login", async (LoginCommand command, ISender mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("Login")
        .WithOpenApi();
        }

}