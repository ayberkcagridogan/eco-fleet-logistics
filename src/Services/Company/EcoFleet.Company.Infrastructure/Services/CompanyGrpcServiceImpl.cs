using EcoFleet.Company.Application.Common.Persistence;
using EcoFleet.Shared.Kernel.Grpc;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace EcoFleet.Company.Infrastructure.Services
{
    public class CompanyGrpcServiceImpl : CompanyGrpcService.CompanyGrpcServiceBase
    {
        private readonly ILogger<CompanyGrpcServiceImpl> _logger;
        private readonly ICompanyRepo _companyRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyGrpcServiceImpl(ILogger<CompanyGrpcServiceImpl> logger, ICompanyRepo companyRepo, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _companyRepo = companyRepo;
            _unitOfWork = unitOfWork;
        }

        public override Task<ValidateCompanyResponse> ValidateCompany(
        ValidateCompanyRequest request, 
        ServerCallContext context)
        {
            _logger.LogInformation("gRPC Request received for CompanyId: {CompanyId}", request.CompanyId);

    
            var isValid = !string.IsNullOrWhiteSpace(request.CompanyId);

            return Task.FromResult(new ValidateCompanyResponse
            {
                IsValid = isValid,
                CompanyName = isValid ? "EcoFleet Logistics Inc." : string.Empty,
                TaxNumber = isValid ? "1234567890" : string.Empty
            });
        }

        public override async Task<GetCompanyByDomainResponse> GetCompanyByDomain(
        GetCompanyByDomainRequest request, 
        ServerCallContext context)
        {
            _logger.LogInformation("Company By Domain: {Domain}", request.Domain);
            var company = await _companyRepo.IgnoreTenantFilter().FirstOrDefaultAsync(
                predicate:x => x.Domain == request.Domain,
                cancellationToken: context.CancellationToken);

            if (company is null)
            {
                return new GetCompanyByDomainResponse { Exists = false };
            }

            return new GetCompanyByDomainResponse
            {
                Exists = true,
                IsActive = company.IsActive,
                CompanyId = company.Id.ToString(),
                CompanyName = company.Name
            };
        }
        public override async Task<CreateCompanyResponse> CreateCompany(CreateCompanyRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Creating company via gRPC: {Name}", request.Name);

                var createdCompany = Domain.Companies.Company.Create(request.Name, request.TaxNumber, request.AdminEmail, request.Domain);
                await _companyRepo.AddAsync(createdCompany);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Company Created with CompanyId : {CompanyId}", createdCompany.Id);

                return new CreateCompanyResponse
                {
                    CompanyId = createdCompany.Id.ToString(),
                    IsSuccess = true,
                    Message = "Company created successfully via gRPC."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating company via gRPC");
                return new CreateCompanyResponse
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public override async Task<RollbackCompanyResponse> RollbackCompany(RollbackCompanyRequest request, ServerCallContext context)
        {
            try
            {
                  _logger.LogInformation("Start Company roll back with CompanyId : {CompanyId}", request.CompanyId);

                if (Guid.TryParse(request.CompanyId, out var companyId))
                {
                    var company = await _companyRepo.GetByIdAsync(companyId);
                    if (company != null)
                    {
                        await _companyRepo.HardDeleteCompanyAsync(company.Id);
                    }
                }
                _logger.LogInformation("Company rolled back successfully. with CompanyId : {CompanyId}", request.CompanyId);
                return new RollbackCompanyResponse { IsSuccess = true, Message = "Company rolled back successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rollback company {CompanyId}", request.CompanyId);
                return new RollbackCompanyResponse { IsSuccess = false, Message = ex.Message };
            }
        }
    }
}