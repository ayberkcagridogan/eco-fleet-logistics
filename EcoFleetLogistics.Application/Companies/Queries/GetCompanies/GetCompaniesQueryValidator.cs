using FluentValidation;

namespace EcoFleetLogistics.Application.Companies.Queries.GetCompanies
{
    public class GetCompaniesQueryValidator : AbstractValidator<GetCompaniesQuery>
    {
        public GetCompaniesQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage("The page number must be at least 1.");
    
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("The page size must be between 1 and 100.");
        }
    }
}