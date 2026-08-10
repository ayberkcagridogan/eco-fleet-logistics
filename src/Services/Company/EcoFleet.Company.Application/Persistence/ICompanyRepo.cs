namespace EcoFleet.Company.Application.Common.Persistence
{
    public interface ICompanyRepo
    {
        Task<Domain.Companies.Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Domain.Companies.Company?> GetCompanyByDomainWithoutTenantFilterAsync(string domain, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default);
        Task<(List<Domain.Companies.Company> Items, int TotalCount)> GetPagedAsync(
                int page, 
                int pageSize, 
                string? searchTerm, 
                bool? isActive, 
                CancellationToken cancellationToken = default);

        Task AddAsync(Domain.Companies.Company company, CancellationToken cancellationToken = default);
        void Update(Domain.Companies.Company company);
        void Remove(Domain.Companies.Company company);
    }
}