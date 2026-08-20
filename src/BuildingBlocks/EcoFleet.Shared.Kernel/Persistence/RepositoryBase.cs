using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using EcoFleet.Shared.Kernel.Persistence.Extensions;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using System.Reflection;

namespace EcoFleet.Shared.Kernel.Persistence
{
    public abstract class RepositoryBase<TEntity, TId, TContext> : IRepositoryBase<TEntity, TId>
    where TEntity : class
    where TContext : DbContext
    {
        protected readonly TContext Context;
        protected DbSet<TEntity> Set;
        protected readonly bool IsTenantFilterIgnored;
        protected readonly bool IsNoTrackingEnabled;
        protected readonly List<Expression<Func<TEntity, object>>> Includes;

        protected RepositoryBase(TContext context) : this(context, false, false,new List<Expression<Func<TEntity, object>>>()) { }

        protected RepositoryBase(
            TContext context,
            bool isTenantFilterIgnored, 
            bool isNoTrackingEnabled,
            List<Expression<Func<TEntity, object>>> includes)
        {
            Context = context;
            Set = context.Set<TEntity>();
            IsTenantFilterIgnored = isTenantFilterIgnored;
            IsNoTrackingEnabled = isNoTrackingEnabled;
            Includes = includes ?? new List<Expression<Func<TEntity, object>>>();
        }

        public virtual IRepositoryBase<TEntity, TId> IgnoreTenantFilter()
        {
            return CreateInstance(isTenantFilterIgnored: true, isNoTrackingEnabled: IsNoTrackingEnabled, Includes);
        }

        public virtual IRepositoryBase<TEntity, TId> AsNoTracking()
        {
            return CreateInstance(isTenantFilterIgnored: IsTenantFilterIgnored, isNoTrackingEnabled: true, Includes);
        }
        public virtual IRepositoryBase<TEntity, TId> Include(Expression<Func<TEntity, object>> navigationPropertyPath)
        {
            
            var updatedIncludes = new List<Expression<Func<TEntity, object>>>(Includes)
            {
                navigationPropertyPath
            };

            return CreateInstance(IsTenantFilterIgnored, IsNoTrackingEnabled, updatedIncludes);
        }
        protected IQueryable<TEntity> Query()
        {
            IQueryable<TEntity> query = Set;

            if (IsTenantFilterIgnored)
                query = query.IgnoreQueryFilters();

            if (IsNoTrackingEnabled)
                query = query.AsNoTracking();

            foreach (var include in Includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        private IRepositoryBase<TEntity, TId> CreateInstance(
            bool isTenantFilterIgnored, 
            bool isNoTrackingEnabled,
            List<Expression<Func<TEntity, object>>> includes)
        {
            return (IRepositoryBase<TEntity, TId>)Activator.CreateInstance(
                GetType(),
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                new object[] { Context, isTenantFilterIgnored, isNoTrackingEnabled ,includes},
                null)!;
        }

        public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            return await Query().FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id")!.Equals(id), cancellationToken);
        }

        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Query().ToListAsync(cancellationToken);
        }

        public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Query().Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Query().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Query().AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null!, CancellationToken cancellationToken = default)
        {
            return predicate == null 
                ? await Query().CountAsync(cancellationToken) 
                : await Query().CountAsync(predicate, cancellationToken);
        }
        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Set.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await Set.AddRangeAsync(entities, cancellationToken);
        }

        public virtual void Update(TEntity entity)
        {
            Set.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            Set.UpdateRange(entities);
        }
        
        public virtual void Remove(TEntity entity)
        {
            Set.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            Set.RemoveRange(entities);
        }
    }
}