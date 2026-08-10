using EcoFleet.Identity.Application.Features.Users.Commands.CreateUser;
using EcoFleet.Identity.Domain.Constants;
using MediatR;

namespace EcoFleet.Identity.Api.Endpoints
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