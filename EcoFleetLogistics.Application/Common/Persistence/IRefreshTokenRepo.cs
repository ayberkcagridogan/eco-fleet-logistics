using EcoFleetLogistics.Domain.Authentication;

namespace EcoFleetLogistics.Application.Common.Persistence;


public interface IRefreshTokenRepo
{
    Task<RefreshToken?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByTokenWithoutTenantFilterAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

}