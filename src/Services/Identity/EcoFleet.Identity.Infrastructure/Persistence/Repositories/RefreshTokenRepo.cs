
using System.Runtime.CompilerServices;
using EcoFleet.Identity.Application.Common.Persistence;
using EcoFleet.Identity.Domain.Authentication;
using Microsoft.EntityFrameworkCore;

namespace EcoFleet.Identity.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepo : IRefreshTokenRepo
    {
        private readonly IdentityDbContext _context;
        public RefreshTokenRepo(IdentityDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                     .Include(rt => rt.User)
                     .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenWithoutTenantFilterAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                            //     .IgnoreTenantFilterIf(true)
                                 .Include(rt => rt.User)
                                 .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);
        }
    }
}