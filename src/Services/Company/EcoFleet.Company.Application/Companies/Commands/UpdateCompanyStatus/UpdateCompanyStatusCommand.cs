using EcoFleet.Company.Application.Common.Persistence;
using MediatR;

namespace EcoFleet.Company.Application.Companies.Commands.UpdateCompanyStatus
{
    public record UpdateCompanyStatusCommand
    (
        Guid Id,
        bool IsActive
    ) : IRequest<bool>;

    public class UpdateCompanyStatusCommandHandler : IRequestHandler<UpdateCompanyStatusCommand, bool>
    {
        private readonly ICompanyRepo _companyRepo;
     //   private readonly IUnityOfWork _unityOfWork;
       // private readonly ICurrentUserService _currentUserService;

        public UpdateCompanyStatusCommandHandler(ICompanyRepo companyRepo
        //, IUnityOfWork unityOfWork, ICurrentUserService currentUserService
        )
        {
            _companyRepo = companyRepo;
       //     _unityOfWork = unityOfWork;
         //   _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(UpdateCompanyStatusCommand request, CancellationToken cancellationToken)
        {
            /*
            if(_currentUserService.CompanyId != request.Id)
            {
                throw new UnauthorizedAccessException("You do not have the authority to modify information belonging to another company.");
            }
*/
            var company = await _companyRepo.GetByIdAsync(request.Id, cancellationToken);
            if(company is null)
                return false;

            company.UpdateStatus(request.IsActive);
            _companyRepo.Update(company);
         //   await _unityOfWork.SaveChangesAsync();

            return true;
        }
    }
}