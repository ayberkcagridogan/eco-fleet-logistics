using EcoFleetLogistics.Application.Authentication.Common;
using EcoFleetLogistics.Application.Common.Authentication.Interfaces;
using EcoFleetLogistics.Application.Common.Interfaces.Authentication;
using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Application.Common.Persistence;
using MediatR;

namespace EcoFleetLogistics.Application.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthenticationResult>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly IUserRepo _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(IUserRepo userRepo, IPasswordHasher passwordHasher, ITokenService tokenService, IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }
    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email, cancellationToken);
        if(user is null)
            throw new UnauthorizedAccessException("Incorrect email or password.");
        
        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if(!isPasswordValid)
            throw new UnauthorizedAccessException("Incorrect email or password.");

        var tokenResult = await _tokenService.GenerateAndSaveTokensAsync(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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