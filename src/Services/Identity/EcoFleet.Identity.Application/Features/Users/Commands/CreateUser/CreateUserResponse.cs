namespace EcoFleet.Identity.Application.Features.Users.Commands.CreateUser
{
    public record CreateUserResponse(Guid Id, string Email, string Role);
}