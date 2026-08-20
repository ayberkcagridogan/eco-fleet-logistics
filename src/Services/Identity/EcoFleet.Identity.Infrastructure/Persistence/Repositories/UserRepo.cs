using EcoFleet.Identity.Application.Common.Persistence;
using EcoFleet.Identity.Domain.Users;
using EcoFleet.Identity.Domain.ValueObjects;
using EcoFleet.Shared.Kernel.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EcoFleet.Identity.Infrastructure.Persistence.Repositories;


public class UserRepo : IUserRepo
{
    private readonly IdentityDbContext _context;

    public UserRepo(IdentityDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailVo = Email.Create(email);
        return await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == emailVo, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAndCompanyIdWithoutTenantFilterAsync(string email, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var emailVo = Email.Create(email);
        return await _context.Users
                        .IgnoreTenantFilterIf(true)
                        .FirstOrDefaultAsync(u => u.Email == emailVo && u.TenantId == tenantId, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
                    .AnyAsync(u => u.Email == email, cancellationToken);
    }
}