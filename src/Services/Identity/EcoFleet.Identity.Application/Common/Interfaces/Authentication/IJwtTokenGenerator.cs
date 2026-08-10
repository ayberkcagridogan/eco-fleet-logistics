

using EcoFleet.Identity.Domain.Users;

namespace EcoFleet.Identity.Application.Common.Interfaces.Authentication;


public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}