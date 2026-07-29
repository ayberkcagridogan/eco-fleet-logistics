using EcoFleetLogistics.Application.Authentication.Common;
using EcoFleetLogistics.Application.Common.Authentication.Interfaces;
using EcoFleetLogistics.Application.Common.Interfaces.Authentication;
using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Domain.Authentication;
using MediatR;

namespace EcoFleetLogistics.Application.Authentication.Commands.RefreshToken;


public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<AuthenticationResult>;


public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticationResult>
{
    private readonly IUserRepo _userRepo;
    private readonly IUnityOfWork _unityOfWork;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepo _refreshTokenRepo;

    public RefreshTokenCommandHandler(
        IUserRepo userRepo,
        IUnityOfWork unityOfWork,
        ITokenService tokenService,
        IRefreshTokenRepo refreshTokenRepo)
    {
        _userRepo = userRepo;
        _unityOfWork = unityOfWork;
        _tokenService = tokenService;
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<AuthenticationResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if(existingToken == null || !existingToken.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        
        var user = await _userRepo.GetByIdAsync(existingToken.UserId , cancellationToken);
        if(user == null)
            throw new UnauthorizedAccessException("User associated with token not found.");

        existingToken.Revoke();
        var tokenResult = await _tokenService.GenerateAndSaveTokensAsync(user, cancellationToken);
        await _unityOfWork.SaveChangesAsync(cancellationToken);

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
   