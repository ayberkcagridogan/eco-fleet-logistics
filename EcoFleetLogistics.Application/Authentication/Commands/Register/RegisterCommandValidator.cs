using System.Net.Cache;
using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Domain.User.Enums;
using EcoFleetLogistics.Domain.ValueObjects;
using FluentValidation;

namespace EcoFleetLogistics.Application.Authentication.Commands.Register;


public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IUserRepo _userRepo;

    public RegisterCommandValidator(IUserRepo userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256).WithMessage("E-Mail name must not exceed 256 characters.")
            .MustAsync(BeUniqueEmail).WithMessage("The specified email address is already in use.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.");
        
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .IsEnumName(typeof(UserRole), caseSensitive: false)
            .WithMessage("Invalid role specified. Valid roles are: User, Admin vs");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken token)
    {    
        try
        {
            var emailVo = Email.Create(email);
            return !await _userRepo.ExistsByEmailAsync(emailVo, token);
        }
        catch
        {
            return false;
        }
        
    }
}