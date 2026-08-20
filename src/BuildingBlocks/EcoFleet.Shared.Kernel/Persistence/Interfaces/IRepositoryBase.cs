using System.Linq.Expressions;

namespace EcoFleet.Shared.Kernel.Persistence.Interfaces
{
    public interface IRepositoryBase<TEntity, TId> where TEntity :class
    {
        //Fluent Pattern
        IRepositoryBase<TEntity, TId> IgnoreTenantFilter();
        IRepositoryBase<TEntity, TId> AsNoTracking();
        IRepositoryBase<TEntity, TId> Include(Expression<Func<TEntity, object>> navigationPropertyPath);

        Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null!, CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}