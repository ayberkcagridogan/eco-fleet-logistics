using EcoFleetLogistics.Application.Users.Commands.CreateUser;
using EcoFleetLogistics.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.HttpLogging;

namespace EcoFleetLogistics.Api.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/users")
                        .WithTags("Users")
                        .RequireAuthorization(Policies.ManagementOnly);

            
            group.MapPost("/", async(CreateUserCommand command, ISender mediator, CancellationToken ct) =>
            {
               var result = await mediator.Send(command , ct);
               return Results.Created($"/api/v1/users/{result.Id}", result);
            })
            .WithName("CreateUser")
            .WithOpenApi();
        }
    }
}