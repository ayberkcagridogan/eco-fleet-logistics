using EcoFleet.Identity.Application.Common.Persistence;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using MediatR;

namespace EcoFleet.Identity.Application.Features.Authentication.Commands.Logout
{
    public record LogoutCommand(
        Guid UserId,
        string RefreshToken
    ): IRequest;

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepo _refreshTokenRepo;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(IRefreshTokenRepo refreshTokenRepo ,IUnitOfWork unitOfWork)
        {
            _refreshTokenRepo = refreshTokenRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepo.FirstOrDefaultAsync(
                rf => rf.Token == request.RefreshToken,
                cancellationToken);

            if(token == null || !token.IsActive || token.UserId != request.UserId)
                throw new UnauthorizedAccessException("Invalid operation or unauthorized token revocation request.");

            token.MarkAsUsed();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}