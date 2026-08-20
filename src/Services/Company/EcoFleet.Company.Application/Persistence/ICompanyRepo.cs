using System.Linq.Expressions;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;

namespace EcoFleet.Company.Application.Common.Persistence
{
    public interface ICompanyRepo : IRepositoryBase<Domain.Companies.Company, Guid>
    {
        new ICompanyRepo IgnoreTenantFilter();
        new ICompanyRepo AsNoTracking();
        new ICompanyRepo Include(Expression<Func<Domain.Companies.Company, object>> navigationPropertyPath);
        Task<(List<Domain.Companies.Company> Items, int TotalCount)> GetPagedAsync(
                int page, 
                int pageSize, 
                string? searchTerm, 
                bool? isActive, 
                CancellationToken cancellationToken = default);
        Task<bool> HardDeleteCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);
    }
}