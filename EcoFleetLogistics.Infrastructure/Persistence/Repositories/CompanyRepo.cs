using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Domain.Companies;
using EcoFleetLogistics.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EcoFleetLogistics.Infrastructure.Persistence.Repositories
{
    public class CompanyRepo : ICompanyRepo
    {
        private readonly AppDbContext _context;

        public CompanyRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Company company, CancellationToken cancellationToken = default)
        {
             await _context.Companies.AddAsync(company);
        }

        public async Task<bool> ExistsByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .AnyAsync(x => x.TaxNumber == taxNumber, cancellationToken);
        }

        public async Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        public async Task<Company?> GetCompanyByDomainWithoutTenantFilterAsync(string domain, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                    .IgnoreTenantFilterIf(true)
                    .FirstOrDefaultAsync(x => x.Domain == domain, cancellationToken);
        }

        public async Task<(List<Company> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm, bool? isActive, CancellationToken cancellationToken = default)
        {
            var query = _context.Companies
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

        public void Update(Company company)
        {
            _context.Companies.Update(company);
        }
        public void Remove(Company company)
        {
            _context.Companies.Remove(company);
        }
    }
}