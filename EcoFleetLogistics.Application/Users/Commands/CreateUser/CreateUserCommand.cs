using EcoFleetLogistics.Application.Authentication.Common;
using EcoFleetLogistics.Application.Common.Authentication.Interfaces;
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
    Guid CompanyId,
    string Role = "Customer"
) : IRequest<CreateUserResponse>;


public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserRepo _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unityOfWork;

    public CreateUserCommandHandler(IUserRepo userRepo, IPasswordHasher passwordHasher, ITokenService tokenService, IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unityOfWork = unitOfWork;
    }
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.FirstName, request.LastName, request.Email, passwordHash, request.CompanyId, User.ResolveRole(request.Role));

        await _userRepo.AddAsync(user, cancellationToken);
        var tokenResult = await _tokenService.GenerateAndSaveTokensAsync(user, cancellationToken);
        await _unityOfWork.SaveChangesAsync(cancellationToken);
        
        return new CreateUserResponse(
            user.Id,
            user.Email.Value,
            user.Role.ToString());
    }
}