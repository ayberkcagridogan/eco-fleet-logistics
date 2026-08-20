using System.Linq.Expressions;
using EcoFleet.Company.Application.Common.Persistence;
using EcoFleet.Shared.Kernel.Persistence;
using EcoFleet.Shared.Kernel.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EcoFleet.Company.Infrastructure.Persistence.Repositories
{
    public class CompanyRepo : RepositoryBase<Domain.Companies.Company, Guid, CompanyDbContext>, ICompanyRepo
    {

        public CompanyRepo(CompanyDbContext context)
        : base(context){}

        protected CompanyRepo(
            CompanyDbContext context, 
            bool isTenantFilterIgnored,
            bool isNoTrackingEnabled,
            List<Expression<Func<Domain.Companies.Company, object>>> includes)
        :base (context, isTenantFilterIgnored , isNoTrackingEnabled , includes){}

        public new ICompanyRepo IgnoreTenantFilter() => (ICompanyRepo)base.IgnoreTenantFilter();
        public new ICompanyRepo AsNoTracking() => (ICompanyRepo)base.AsNoTracking();
        public new ICompanyRepo Include(Expression<Func<Domain.Companies.Company, object>> navigationPropertyPath) 
                => (ICompanyRepo)base.Include(navigationPropertyPath);
                
        public async Task<(List<Domain.Companies.Company> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm, bool? isActive, CancellationToken cancellationToken = default)
        {
            var query = Query()
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearchTerm = searchTerm.Trim().ToLower();
                query = query.Where(c => 
                    c.Name.ToLower().Contains(normalizedSearchTerm) || 
                    c.TaxNumber.Contains(normalizedSearchTerm));
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(c => c.CreatedAt) // En yeni kayıtlar en üstte gösterilir
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

             return (items, totalCount);
        }


        public async Task<bool> HardDeleteCompanyAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var affectedRows = await Query()
                    .Where(c => c.Id == companyId)
                    .ExecuteDeleteAsync(cancellationToken);

            return affectedRows > 0;
        }
    }
}