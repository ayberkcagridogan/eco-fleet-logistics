using EcoFleetLogistics.Application.Common.Interfaces.Persistence;

namespace EcoFleetLogistics.Infrastructure.Persistence
{
    public class UnityOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnityOfWork(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}