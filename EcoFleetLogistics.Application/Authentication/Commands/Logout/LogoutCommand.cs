using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Application.Common.Persistence;
using MediatR;

namespace EcoFleetLogistics.Application.Authentication.Commands.Logout
{
    public record LogoutCommand(
        Guid UserId,
        string RefreshToken
    ): IRequest;

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepo _refreshTokenRepo;
        private readonly IUnityOfWork _unityOfWork;

        public LogoutCommandHandler(IRefreshTokenRepo refreshTokenRepo, IUnityOfWork unityOfWork)
        {
            _refreshTokenRepo = refreshTokenRepo;
            _unityOfWork = unityOfWork;
        }
        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken, cancellationToken);

            if(token == null || !token.IsActive || token.UserId != request.UserId)
                throw new UnauthorizedAccessException("Invalid operation or unauthorized token revocation request.");

            token.Revoke();

            await _unityOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}