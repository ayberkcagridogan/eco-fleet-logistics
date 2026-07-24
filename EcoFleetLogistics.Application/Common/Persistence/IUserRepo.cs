using EcoFleetLogistics.Domain.User;
using EcoFleetLogistics.Domain.ValueObjects;

namespace EcoFleetLogistics.Application.Common.Persistence;

public interface IUserRepo
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
}