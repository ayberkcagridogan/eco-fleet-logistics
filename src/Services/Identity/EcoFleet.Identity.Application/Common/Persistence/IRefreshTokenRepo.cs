using System.Linq.Expressions;
using EcoFleet.Identity.Domain.Authentication;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;

namespace EcoFleet.Identity.Application.Common.Persistence;


public interface IRefreshTokenRepo : IRepositoryBase<RefreshToken, Guid>
{
    new IRefreshTokenRepo IgnoreTenantFilter();
    new IRefreshTokenRepo AsNoTracking();
    new IRefreshTokenRepo Include(Expression<Func<RefreshToken, object>> navigationPropertyPath);
}