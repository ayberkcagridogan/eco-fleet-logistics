using System.Linq.Expressions;
using EcoFleet.Identity.Application.Common.Persistence;
using EcoFleet.Identity.Domain.Authentication;
using EcoFleet.Shared.Kernel.Persistence;


namespace EcoFleet.Identity.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepo : RepositoryBase<RefreshToken, Guid, IdentityDbContext> , IRefreshTokenRepo
    {
        public RefreshTokenRepo(IdentityDbContext context) 
        : base(context){}
        protected RefreshTokenRepo(
            IdentityDbContext context, 
            bool ignoreTenantFilter,
            bool isNoTrackingEnabled,
            List<Expression<Func<RefreshToken, object>>> includes)
        : base(context, ignoreTenantFilter, isNoTrackingEnabled, includes){}

        public new IRefreshTokenRepo IgnoreTenantFilter() => (IRefreshTokenRepo)base.IgnoreTenantFilter();
        public new IRefreshTokenRepo AsNoTracking() => (IRefreshTokenRepo)base.AsNoTracking();
        public new IRefreshTokenRepo Include(Expression<Func<RefreshToken, object>> navigationPropertyPath) 
                => (IRefreshTokenRepo)base.Include(navigationPropertyPath);
    }
}