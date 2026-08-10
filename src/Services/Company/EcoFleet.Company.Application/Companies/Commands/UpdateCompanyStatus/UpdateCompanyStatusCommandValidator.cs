using FluentValidation;

namespace EcoFleet.Company.Application.Companies.Commands.UpdateCompanyStatus
{
public class UpdateCompanyStatusCommandValidator : AbstractValidator<UpdateCompanyStatusCommand>
{
    public UpdateCompanyStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Company ID cannot be empty.");
    }
}
}