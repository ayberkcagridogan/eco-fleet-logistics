using System.Linq.Expressions;
using EcoFleet.Identity.Application.Common.Persistence;
using EcoFleet.Identity.Domain.Authentication;
using EcoFleet.Identity.Domain.Users;
using EcoFleet.Shared.Kernel.Persistence;

namespace EcoFleet.Identity.Infrastructure.Persistence.Repositories;


public class UserRepo : RepositoryBase<User, Guid, IdentityDbContext>, IUserRepo
{

    public UserRepo(IdentityDbContext context) 
        : base(context) { }
    protected UserRepo(
        IdentityDbContext context,
        bool isTenantFilterIgnored,
        bool isNoTrackingEnabled,
        List<Expression<Func<User, object>>> includes) 
        : base(context, isTenantFilterIgnored,isNoTrackingEnabled,includes) { }
        

        public new IUserRepo IgnoreTenantFilter() => (IUserRepo)base.IgnoreTenantFilter();
        public new IUserRepo AsNoTracking() => (IUserRepo)base.AsNoTracking();
        public new IUserRepo Include(Expression<Func<User, object>> navigationPropertyPath) 
                => (IUserRepo)base.Include(navigationPropertyPath);

}