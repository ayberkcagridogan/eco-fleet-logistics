using EcoFleet.Shared.Kernel.Persistence;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace EcoFleetLogistics.Infrastructure.Persistence
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : BaseDbContext<TContext>
    {
        private readonly TContext _dbContext;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(TContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {   
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
            }
        }
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
            }
        }
    }
}