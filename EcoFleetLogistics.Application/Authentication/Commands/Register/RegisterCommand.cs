using System.ComponentModel;
using EcoFleetLogistics.Application.Authentication.Common;
using EcoFleetLogistics.Application.Common.Interfaces;
using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Domain.User;
using EcoFleetLogistics.Domain.User.Enums;
using EcoFleetLogistics.Domain.ValueObjects;
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
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(IUserRepo userRepo, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.FirstName, request.LastName, request.Email, passwordHash, User.ResolveRole(request.Role));
        var token = _jwtTokenGenerator.GenerateToken(user);
        await _userRepo.AddAsync(user, cancellationToken);
        
        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.Role.ToString(),
            token);
    }
}