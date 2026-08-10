
using EcoFleet.Identity.Application.Common.Interfaces.Authentication;
using EcoFleet.Identity.Application.Common.Persistence;
using EcoFleet.Identity.Application.Features.Authentication.Common;
using MediatR;

namespace EEcoFleet.Identity.Application.Features.Authentication.Commands.RefreshToken;


public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<AuthenticationResult>;


public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticationResult>
{
    private readonly IUserRepo _userRepo;
 //   private readonly IUnityOfWork _unityOfWork;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepo _refreshTokenRepo;

    public RefreshTokenCommandHandler(
        IUserRepo userRepo,
    //    IUnityOfWork unityOfWork,
        ITokenService tokenService,
        IRefreshTokenRepo refreshTokenRepo)
    {
        _userRepo = userRepo;
     //   _unityOfWork = unityOfWork;
        _tokenService = tokenService;
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<AuthenticationResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepo.GetByTokenWithoutTenantFilterAsync(request.RefreshToken, cancellationToken);

        if(existingToken == null || !existingToken.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        
        var user = await _userRepo.GetByEmailAndCompanyIdWithoutTenantFilterAsync(existingToken.User.Email.Value, existingToken.User.TenantId, cancellationToken);
        if(user == null)
            throw new UnauthorizedAccessException("User associated with token not found.");

        existingToken.Revoke();
        var tokenResult = await _tokenService.GenerateAndSaveTokensAsync(user, cancellationToken);
     //   await _unityOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.Role.ToString(),
            tokenResult.AccessToken,
            tokenResult.RefreshToken
        );
    }
}
   