using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Domain.Users;
using EcoFleetLogistics.Domain.ValueObjects;
using EcoFleetLogistics.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EcoFleetLogistics.Infrastructure.Persistence.Repositories;


public class UserRepo : IUserRepo
{
    private readonly AppDbContext _context;

    public UserRepo(AppDbContext context)
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

    public async Task<User?> GetByEmailAndCompanyIdWithoutTenantFilterAsync(string email, Guid companyId, CancellationToken cancellationToken = default)
    {
        var emailVo = Email.Create(email);
        return await _context.Users
                        .IgnoreTenantFilterIf(true)
                        .FirstOrDefaultAsync(u => u.Email == emailVo && u.CompanyId == companyId, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
                    .AnyAsync(u => u.Email == email, cancellationToken);
    }
}