
using System.Runtime.CompilerServices;
using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Domain.Authentication;
using Microsoft.EntityFrameworkCore;

namespace EcoFleetLogistics.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepo : IRefreshTokenRepo
    {
        private readonly AppDbContext _context;
        public RefreshTokenRepo(AppDbContext context)
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
                                    .FirstAsync(rt => rt.Token == refreshToken, cancellationToken);
        }
    }
}