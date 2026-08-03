using EcoFleetLogistics.Application.Common.Persistence;
using MediatR;

namespace EcoFleetLogistics.Application.Companies.Queries.GetCompanies
{
    public record GetCompaniesQuery
    (
        int Page = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        bool? IsActive = null
    ) : IRequest<PagedResponse<CompanyResponse>>;

    public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, PagedResponse<CompanyResponse>>
    {
        private readonly ICompanyRepo _companyRepo;

        public GetCompaniesQueryHandler(ICompanyRepo companyRepo)
        {
            _companyRepo = companyRepo;
        }
        public async Task<PagedResponse<CompanyResponse>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _companyRepo.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.IsActive,
                cancellationToken
            );

            var responses = items.Select(c => new CompanyResponse(
                c.Id,
                c.Name,
                c.TaxNumber,
                c.AdminEmail,
                c.IsActive,
                c.CreatedAt
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.Page);

            return new PagedResponse<CompanyResponse>(responses, request.Page, request.PageSize, totalCount, totalPages);
        }
    }
}