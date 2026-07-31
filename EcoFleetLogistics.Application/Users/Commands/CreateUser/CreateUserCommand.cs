using EcoFleetLogistics.Application.Authentication.Common;
using EcoFleetLogistics.Application.Common.Authentication.Interfaces;
using EcoFleetLogistics.Application.Common.Interfaces;
using EcoFleetLogistics.Application.Common.Interfaces.Authentication;
using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Domain.Users;
using MediatR;

namespace EcoFleetLogistics.Application.Users.Commands.CreateUser;

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
    private readonly IUnityOfWork _unityOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateUserCommandHandler(IUserRepo userRepo, IPasswordHasher passwordHasher, ICurrentUserService currentUserService, IUnityOfWork unityOfWork)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _unityOfWork = unityOfWork;
        _currentUserService = currentUserService;
    }
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var companyId = _currentUserService.CompanyId 
            ?? throw new UnauthorizedAccessException("Tenant/Company context is missing.");

        var user = User.Create(request.FirstName, request.LastName, request.Email, passwordHash, companyId, User.ResolveRole(request.Role));

        await _userRepo.AddAsync(user, cancellationToken);
        await _unityOfWork.SaveChangesAsync(cancellationToken);
        
        return new CreateUserResponse(
            user.Id,
            user.Email.Value,
            user.Role.ToString());
    }
}