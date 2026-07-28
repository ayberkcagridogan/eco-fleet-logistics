using EcoFleetLogistics.Application.Common.Models.Authentication;
using EcoFleetLogistics.Domain.Users;

namespace EcoFleetLogistics.Application.Common.Interfaces.Authentication
{
    public interface ITokenService
    {
        Task<TokenResult> GenerateAndSaveTokensAsync(User user, CancellationToken cancellationToken);
    }
}