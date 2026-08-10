using System.Linq.Expressions;
using EcoFleet.Shared.Kernel.Primitives.Interfaces;
using EcoFleet.Shared.Kernel.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoFleet.Shared.Kernel.Persistence
{
    public class BaseDbContext<TContext> : DbContext where TContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        protected BaseDbContext(DbContextOptions<TContext> options, ICurrentUserService currentUserService) 
        : base(options)
        {
            _currentUserService = currentUserService;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditAndSoftDeleteConcepts();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditAndSoftDeleteConcepts()
        {
            var utcNow = DateTimeOffset.UtcNow;
            var userId = _currentUserService.UserId;

            foreach (var entry in ChangeTracker.Entries())
            {
                // 1. Audit Handling
                if (entry.Entity is IAuditableEntity auditable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue = utcNow;
                        var createdByIdProp = entry.Property(nameof(IAuditableEntity.CreatedById));
                        if(createdByIdProp.CurrentValue is null)
                            createdByIdProp.CurrentValue = userId;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = utcNow;
                        entry.Property(nameof(IAuditableEntity.UpdatedById)).CurrentValue = userId;
                    }
                }

                // 2. Soft Delete Handling
                if (entry.Entity is ISoftDelete && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;

                    entry.Property(nameof(ISoftDelete.IsDeleted)).CurrentValue = true;
                    entry.Property(nameof(ISoftDelete.DeletedAt)).CurrentValue = utcNow;
                    entry.Property(nameof(ISoftDelete.DeletedById)).CurrentValue = userId;
                }

                // 3. Multi-Tenant Auto Assignment
                if (entry.Entity is IMultiTenant multiTenant && entry.State == EntityState.Added)
                {
                    var tenantIdProp = entry.Property(nameof(IMultiTenant.TenantId));
                    if (tenantIdProp.CurrentValue is null && _currentUserService.TenantId.HasValue)
                    {
                        tenantIdProp.CurrentValue = _currentUserService.TenantId.Value;
                    }
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            base.OnModelCreating(modelBuilder);

            // Global Query Filters (SoftDelete & TenantFilter)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                Expression? filter = null;

                // Soft Delete Filter
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    var isDeletedProperty = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
                    filter = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                }

                // Multi-Tenant Filter
                if (typeof(IMultiTenant).IsAssignableFrom(entityType.ClrType))
                {
                    var tenantProperty = Expression.Property(parameter, nameof(IMultiTenant.TenantId));

                    var currentTenantId = Expression.Property(
                        Expression.Constant(_currentUserService), 
                        nameof(ICurrentUserService.TenantId)
                    );

                    var tenantPropertyAsNullable = Expression.Convert(tenantProperty, typeof(Guid?));

                    var tenantFilter = Expression.Equal(tenantPropertyAsNullable, currentTenantId);

                    filter = filter == null ? tenantFilter : Expression.AndAlso(filter, tenantFilter);
                }

                if (filter != null)
                {
                    var lambda = Expression.Lambda(filter, parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}