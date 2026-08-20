
using EcoFleet.Identity.Application.Common.Interfaces.Authentication;
using EcoFleet.Identity.Application.Common.Persistence;
using EcoFleet.Identity.Application.Features.Authentication.Common;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EEcoFleet.Identity.Application.Features.Authentication.Commands.RefreshToken;


public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<AuthenticationResult>;


public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticationResult>
{
    private readonly IUserRepo _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepo _refreshTokenRepo;

    public RefreshTokenCommandHandler(IUserRepo userRepo, IUnitOfWork unitOfWork, ITokenService tokenService, IRefreshTokenRepo refreshTokenRepo)
    {
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<AuthenticationResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepo
                                    .IgnoreTenantFilter()
                                    .Include(rt => rt.User)
                                    .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if(existingToken == null || !existingToken.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        
        if (existingToken.IsUsed)
        {
            var activeTokens = await _refreshTokenRepo.IgnoreTenantFilter().FindAsync(
                rt => rt.UserId == existingToken.UserId && rt.IsActive, 
                cancellationToken);
                
            foreach (var token in activeTokens)
            {
                token.MarkAsDeleted(token.UserId);
            }

            _refreshTokenRepo.RemoveRange(activeTokens);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            throw new UnauthorizedAccessException("Security warning: Refresh token reuse detected. All sessions have been revoked.");
        }

        if(!existingToken.IsActive)
            throw new UnauthorizedAccessException("Expired or revoked refresh token.");
        
        var user = await _userRepo.IgnoreTenantFilter().GetByIdAsync(existingToken.UserId, cancellationToken);
        if(user == null)
            throw new UnauthorizedAccessException("User associated with token not found.");

        existingToken.MarkAsUsed(existingToken.UserId);

        var tokenResult = await _tokenService.GenerateAndSaveTokensAsync(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
   