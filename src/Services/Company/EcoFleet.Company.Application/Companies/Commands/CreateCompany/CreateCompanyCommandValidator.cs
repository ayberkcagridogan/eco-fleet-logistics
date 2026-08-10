using EcoFleet.Company.Application.Common.Persistence;
using FluentValidation;

namespace EcoFleet.Company.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {

        public CreateCompanyCommandValidator(ICompanyRepo companyRepo)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The company name cannot be empty.")
                .MaximumLength(150).WithMessage("The company name can be a maximum of 150 characters.");

            RuleFor(x => x.TaxNumber)
                .NotEmpty().WithMessage("The tax number cannot be blank.")
                .Matches(@"^\d{10}$").WithMessage("The tax number must consist of 10 digits.");

            RuleFor(x => x.AdminEmail)
                .NotEmpty().WithMessage("The e-mail address cannot be empty.")
                .EmailAddress().WithMessage("Please enter a valid e-mail address.");
        }

        
    }
}