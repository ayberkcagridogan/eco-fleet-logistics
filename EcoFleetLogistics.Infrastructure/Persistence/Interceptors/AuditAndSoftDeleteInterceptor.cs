using System.ComponentModel;
using EcoFleetLogistics.Application.Common.Interfaces;
using EcoFleetLogistics.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EcoFleetLogistics.Infrastructure.Persistence.Interceptors
{
    public class AuditAndSoftDeleteInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditAndSoftDeleteInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if(eventData.Context is null)
                return base.SavingChangesAsync(eventData, result, cancellationToken);

            UpdateAuditAndSoftDaleteProperties(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateAuditAndSoftDaleteProperties(DbContext context)
        {
           var currentUserId = _currentUserService.UserId;
           var utcNow = DateTime.UtcNow;

           foreach (var entry in context.ChangeTracker.Entries())
            {
                // AUDIT INTERCEPTION
                if(entry.Entity is IAuditableEntity auditable)
                {
                    if(entry.State == EntityState.Added)
                    {
                        entry.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue = utcNow;
                        
                        var createdByIdProp = entry.Property(nameof(IAuditableEntity.CreatedById));
                        if(createdByIdProp.CurrentValue is null)
                            createdByIdProp.CurrentValue = currentUserId;

                    }
                    else if(entry.State == EntityState.Modified)
                    {
                        entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = utcNow;
                        entry.Property(nameof(IAuditableEntity.UpdatedById)).CurrentValue = currentUserId;
                    }
                }

                // SOFT DELETE INTERCEPTION
                if (entry.Entity is ISoftDelete && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;

                    entry.Property(nameof(ISoftDelete.IsDeleted)).CurrentValue = true;
                    entry.Property(nameof(ISoftDelete.DeletedAt)).CurrentValue = utcNow;
                    entry.Property(nameof(ISoftDelete.DeletedById)).CurrentValue = currentUserId;
                }
            }
        }
    }
}