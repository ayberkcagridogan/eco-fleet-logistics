using System.Runtime.ConstrainedExecution;
using EcoFleetLogistics.Application.Authentication.Common;
using EcoFleetLogistics.Application.Common.Interfaces;
using EcoFleetLogistics.Application.Common.Persistence;
using MediatR;

namespace EcoFleetLogistics.Application.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthenticationResult>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly IUserRepo _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(IUserRepo userRepo, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email, cancellationToken);
        if(user is null)
            throw new UnauthorizedAccessException("Incorrect email or password.");
        
        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if(!isPasswordValid)
            throw new UnauthorizedAccessException("Incorrect email or password.");

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.Role.ToString(),
            token);
    }
}