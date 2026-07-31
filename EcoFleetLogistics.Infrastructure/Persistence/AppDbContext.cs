using System.Linq.Expressions;
using EcoFleetLogistics.Application.Common.Interfaces;
using EcoFleetLogistics.Domain.Authentication;
using EcoFleetLogistics.Domain.Common;
using EcoFleetLogistics.Domain.Common.Interfaces;
using EcoFleetLogistics.Domain.Companies;
using EcoFleetLogistics.Domain.Shipments;
using EcoFleetLogistics.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace EcoFleetLogistics.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        public Guid? CurrentCompanyId => _currentUserService.CompanyId;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Company> Companies => Set<Company>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                Expression? finalFilter = null;

                //  Soft Delete Filter (IsDeleted == false)
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    var isDeletedProp = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
                    var isNotDeleted = Expression.Equal(isDeletedProp, Expression.Constant(false));

                    finalFilter = isNotDeleted;
                }

                // Multi-Tenant Filter (CompanyId == CurrentCompanyId)
                if (typeof(ICompanyResource).IsAssignableFrom(entityType.ClrType))
                {
                    var tenantFilter = GetMultiTenantFilter(entityType.ClrType, parameter);

                    finalFilter = finalFilter == null 
                        ? tenantFilter 
                        : Expression.AndAlso(finalFilter, tenantFilter);
                }

                // All Filters
                if (finalFilter != null)
                {
                    var lambda = Expression.Lambda(finalFilter, parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        private Expression GetMultiTenantFilter(Type entityType, ParameterExpression parameter)
        {
          
            var companyIdProperty = Expression.Property(parameter, nameof(ICompanyResource.CompanyId));

          
            var currentCompanyIdProperty = Expression.Property(
                Expression.Constant(this), 
                nameof(CurrentCompanyId)
            );
        
            var leftConverted = Expression.Convert(companyIdProperty, typeof(Guid?));
            var rightConverted = Expression.Convert(currentCompanyIdProperty, typeof(Guid?));

            return Expression.Equal(leftConverted, rightConverted);
        }
    }
}