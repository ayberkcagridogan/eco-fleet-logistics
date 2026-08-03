using System.ComponentModel;
using EcoFleetLogistics.Application.Common.Authentication.Interfaces;
using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Domain.Companies;
using EcoFleetLogistics.Domain.Users;
using EcoFleetLogistics.Domain.Users.Enums;
using MediatR;

namespace EcoFleetLogistics.Application.Companies.Commands.CreateCompany
{
    public record CreateCompanyCommand
    (
        string Name,
        string TaxNumber,
        string AdminEmail
    ) : IRequest<Guid>;

    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Guid>
    {
        private readonly ICompanyRepo _companyRepo;
        private readonly IUnityOfWork _unityOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepo _userRepo;

        public CreateCompanyCommandHandler(ICompanyRepo companyRepo, IUnityOfWork unityOfWork, IPasswordHasher passwordHasher, IUserRepo userRepo)
        {
            _companyRepo = companyRepo;
            _unityOfWork = unityOfWork;
            _passwordHasher = passwordHasher;
            _userRepo = userRepo;
        }
        public async Task<Guid> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var taxNumberExists = await _companyRepo.ExistsByTaxNumberAsync(request.TaxNumber, cancellationToken);
            if (taxNumberExists)
            {
                throw new InvalidOperationException($"Company with Tax Number '{request.TaxNumber}' already exists.");
            }

            var domain = request.AdminEmail.Split('@').LastOrDefault();
            if (string.IsNullOrWhiteSpace(domain))
                throw new UnauthorizedAccessException("Invalid email format.");

            var company = Company.Create(request.Name, request.TaxNumber, request.AdminEmail, domain);
            await _companyRepo.AddAsync(company, cancellationToken);

            var tempPasswordHash = _passwordHasher.HashPassword("Admin123!");

            var initialAdminUser = User.Create(
                firstName: "Admin - " + request.Name,
                lastName: request.Name,
                email: request.AdminEmail,
                passwordHash : tempPasswordHash,
                companyId : company.Id,
                role: UserRole.CompanyAdmin
            );

            await _userRepo.AddAsync(initialAdminUser,cancellationToken);
            await _unityOfWork.SaveChangesAsync(cancellationToken);

            //ToDo : Send an email for Rest Password 
            return company.Id;
        }
    }
}