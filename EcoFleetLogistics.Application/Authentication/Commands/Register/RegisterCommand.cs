using EcoFleetLogistics.Application.Authentication.Common;
using EcoFleetLogistics.Application.Common.Authentication.Interfaces;
using EcoFleetLogistics.Application.Common.Interfaces.Authentication;
using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Domain.Users;
using MediatR;

namespace EcoFleetLogistics.Application.Authentication.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role = "User"
) : IRequest<AuthenticationResult>;


public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthenticationResult>
{
    private readonly IUserRepo _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unityOfWork;

    public RegisterCommandHandler(IUserRepo userRepo, IPasswordHasher passwordHasher, ITokenService tokenService, IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unityOfWork = unitOfWork;
    }
    public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.FirstName, request.LastName, request.Email, passwordHash, User.ResolveRole(request.Role));

        await _userRepo.AddAsync(user, cancellationToken);
        var tokenResult = await _tokenService.GenerateAndSaveTokensAsync(user, cancellationToken);
        await _unityOfWork.SaveChangesAsync(cancellationToken);
        
        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.Role.ToString(),
            tokenResult.AccessToken,
            tokenResult.RefreshToken);
    }
}