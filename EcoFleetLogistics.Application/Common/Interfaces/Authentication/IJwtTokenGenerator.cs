using EcoFleetLogistics.Domain.Users;

namespace EcoFleetLogistics.Application.Common.Authentication.Interfaces;


public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}