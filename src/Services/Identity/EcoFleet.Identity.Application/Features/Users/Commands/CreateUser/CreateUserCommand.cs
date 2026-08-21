
using EcoFleet.Identity.Application.Common.Authentication.Interfaces;
using EcoFleet.Identity.Application.Common.Interfaces.Services;
using EcoFleet.Identity.Application.Common.Persistence;
using EcoFleet.Identity.Domain.Users;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using EcoFleet.Shared.Kernel.Services.Interfaces;
using MediatR;

namespace EcoFleet.Identity.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role = "Customer"
) : IRequest<CreateUserResponse>;


public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserRepo _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICompanyGrpcClient _companyGrpcClient;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
            IUserRepo userRepo, 
            IPasswordHasher passwordHasher, 
            ICurrentUserService currentUserService, 
            IUnitOfWork unitOfWork,
            ICompanyGrpcClient companyGrpcClient)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _companyGrpcClient = companyGrpcClient;
    }
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var tensantId = _currentUserService.TenantId 
            ?? throw new UnauthorizedAccessException("Tenant context is missing.");

        var user = User.Create(request.FirstName, request.LastName, request.Email, passwordHash, tensantId, User.ResolveRole(request.Role));

        await _userRepo.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new CreateUserResponse(
            user.Id,
            user.Email.Value,
            user.Role.ToString());
    }
}