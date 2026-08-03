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
    private readonly IUnityOfWork _unityOfWork;
    private readonly ICompanyRepo _companyRepo;

    public LoginCommandHandler(IUserRepo userRepo, IPasswordHasher passwordHasher,ICompanyRepo companyRepo, ITokenService tokenService, IUnityOfWork unityOfWork)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unityOfWork = unityOfWork;
        _companyRepo = companyRepo;
    }
    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var domain = request.Email.Split('@').LastOrDefault();
        if (string.IsNullOrWhiteSpace(domain))
            throw new UnauthorizedAccessException("Invalid email format.");
        
        var company = await _companyRepo.GetCompanyByDomainWithoutTenantFilterAsync(domain, cancellationToken);
        if(company is null || !company.IsActive)
            throw new UnauthorizedAccessException("The company associated with this email is either inactive or does not exist.");

        var user = await _userRepo.GetByEmailAndCompanyIdWithoutTenantFilterAsync(request.Email,company.Id, cancellationToken);
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