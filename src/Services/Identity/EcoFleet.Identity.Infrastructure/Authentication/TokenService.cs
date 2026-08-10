using EcoFleet.Identity.Application.Common.Authentication.Interfaces;
using EcoFleet.Identity.Application.Common.Interfaces.Authentication;
using EcoFleet.Identity.Application.Common.Models.Authentication;
using EcoFleet.Identity.Application.Common.Persistence;
using EcoFleet.Identity.Domain.Authentication;
using EcoFleet.Identity.Domain.Users;

namespace EcoFleet.Identity.Infrastructure.Authentication
{
    public class TokenService : ITokenService
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IRefreshTokenRepo _refreshTokenRepo;

        public TokenService(
            IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            IRefreshTokenRepo refreshTokenRepo)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async Task<TokenResult> GenerateAndSaveTokensAsync(User user, CancellationToken cancellationToken)
        {
            var accessToken = _jwtTokenGenerator.GenerateToken(user);
            var refreshTokenString = _refreshTokenGenerator.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddDays(7);

            var refreshToken = RefreshToken.Create(
                refreshTokenString,
                expiresAt,
                user.Id);

            await _refreshTokenRepo.AddAsync(refreshToken, cancellationToken);
            
            return new TokenResult(accessToken, refreshTokenString, expiresAt);
        }
    }
}