using EcoFleet.Identity.Application.Common.Authentication.Interfaces;
using EcoFleet.Identity.Application.Common.Interfaces.Authentication;
using EcoFleet.Identity.Application.Common.Interfaces.Services;
using EcoFleet.Identity.Application.Common.Persistence;
using EcoFleet.Identity.Application.Features.Authentication.Common;
using EcoFleet.Identity.Domain.ValueObjects;
using EcoFleet.Shared.Kernel.Grpc;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using MediatR;

namespace EcoFleet.Identity.Application.Features.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthenticationResult>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly IUserRepo _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unityOfWork;
    private readonly ICompanyGrpcClient _companyGrpcClient;

    public LoginCommandHandler(
        IUserRepo userRepo, 
        IPasswordHasher passwordHasher, 
        ITokenService tokenService,
        ICompanyGrpcClient companyGrpcClient,
        IUnitOfWork unityOfWork)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unityOfWork = unityOfWork;
        _companyGrpcClient = companyGrpcClient;
    }
    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var domain = request.Email.Split('@').LastOrDefault();
        if (string.IsNullOrWhiteSpace(domain))
            throw new UnauthorizedAccessException("Invalid email format.");
        
        var companyId = await _companyGrpcClient.GetCompanyByDomain(domain);

        var emailVo = Email.Create(request.Email);
        var user = await _userRepo.IgnoreTenantFilter().FirstOrDefaultAsync(
            predicate: u => u.Email == emailVo && u.TenantId == companyId, 
            cancellationToken: cancellationToken);
            
        if(user is null)
            throw new UnauthorizedAccessException("Incorrect email or password.");
        
        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if(!isPasswordValid)
            throw new UnauthorizedAccessException("Incorrect email or password.");

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