

using System.Linq.Expressions;
using EcoFleet.Identity.Domain.Users;
using EcoFleet.Identity.Domain.ValueObjects;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;

namespace EcoFleet.Identity.Application.Common.Persistence;

public interface IUserRepo : IRepositoryBase<User, Guid>
{
    new IUserRepo IgnoreTenantFilter();
    new IUserRepo AsNoTracking();
    new IUserRepo Include(Expression<Func<User, object>> navigationPropertyPath);
}