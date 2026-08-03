namespace EcoFleetLogistics.Application.Companies.Queries.GetCompanies
{
    public record CompanyResponse
    (
        Guid Id,
        string Name,
        string TaxNumber,
        string AdminEmail,
        bool IsActive,
        DateTime CreatedAt);
    
    public record PagedResponse<T>(List<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);
}