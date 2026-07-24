using EcoFleetLogistics.Domain.User;

namespace EcoFleetLogistics.Application.Common.Interfaces;


public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}