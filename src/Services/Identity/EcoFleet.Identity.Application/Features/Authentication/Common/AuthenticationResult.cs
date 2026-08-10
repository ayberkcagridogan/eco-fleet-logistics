namespace EcoFleet.Identity.Application.Features.Authentication.Common;

public record AuthenticationResult(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken
);