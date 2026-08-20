namespace EcoFleet.Identity.Application.Common.Interfaces.Services
{
    public interface ICompanyGrpcClient
    {
        Task<Guid> RegisterSuperAdminAsync(CancellationToken ct = default);
        Task<Guid> GetCompanyByDomain(string domainName, CancellationToken ct = default);
        Task<bool> RollbackCompany(Guid companyId, CancellationToken ct = default);
    }
}