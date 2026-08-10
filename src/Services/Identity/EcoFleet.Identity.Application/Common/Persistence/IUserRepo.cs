

using EcoFleet.Identity.Domain.Users;
using EcoFleet.Identity.Domain.ValueObjects;

namespace EcoFleet.Identity.Application.Common.Persistence;

public interface IUserRepo
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAndCompanyIdWithoutTenantFilterAsync(string email, Guid companyId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
}