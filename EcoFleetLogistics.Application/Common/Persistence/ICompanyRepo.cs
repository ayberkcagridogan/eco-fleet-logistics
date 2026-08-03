using EcoFleetLogistics.Domain.Companies;

namespace EcoFleetLogistics.Application.Common.Persistence
{
    public interface ICompanyRepo
    {
        Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Company?> GetCompanyByDomainWithoutTenantFilterAsync(string domain, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default);
        Task<(List<Company> Items, int TotalCount)> GetPagedAsync(
                int page, 
                int pageSize, 
                string? searchTerm, 
                bool? isActive, 
                CancellationToken cancellationToken = default);

        Task AddAsync(Company company, CancellationToken cancellationToken = default);
        void Update(Company company);
        void Remove(Company company);
    }
}