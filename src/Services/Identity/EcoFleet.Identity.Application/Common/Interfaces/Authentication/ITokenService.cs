using EcoFleet.Identity.Application.Common.Models.Authentication;
using EcoFleet.Identity.Domain.Users;

namespace EcoFleet.Identity.Application.Common.Interfaces.Authentication
{
    public interface ITokenService
    {
        Task<TokenResult> GenerateAndSaveTokensAsync(User user, CancellationToken cancellationToken);
    }
}