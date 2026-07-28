namespace EcoFleetLogistics.Application.Common.Authentication.Interfaces;

public interface IRefreshTokenGenerator
{
    string GenerateRefreshToken();
}