namespace EcoFleet.Identity.Application.Common.Authentication.Interfaces;

public interface IRefreshTokenGenerator
{
    string GenerateRefreshToken();
}