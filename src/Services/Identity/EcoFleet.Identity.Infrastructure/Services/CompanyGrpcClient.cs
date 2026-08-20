using EcoFleet.Identity.Application.Common.Interfaces.Services;
using EcoFleet.Shared.Kernel.Grpc;

namespace EcoFleet.Identity.Infrastructure.Services
{
    public class CompanyGrpcClient : ICompanyGrpcClient
    {
        private readonly CompanyGrpcService.CompanyGrpcServiceClient _client;

        public CompanyGrpcClient(CompanyGrpcService.CompanyGrpcServiceClient client)
        {
            _client = client;
        }
        public async Task<Guid> RegisterSuperAdminAsync(CancellationToken ct)
        {
            var response = await _client.CreateCompanyAsync(new CreateCompanyRequest
            {
                Name = "SuperAdmin Company",
                TaxNumber = "9999999999",
                AdminEmail = "admin@superadmin.com",
                Domain = "superadmin.com"
            }, cancellationToken : ct);

            if (!response.IsSuccess)
                throw new Exception($"Company creation failed via gRPC: {response.Message}");

            return Guid.Parse(response.CompanyId);
        }

        public async Task<Guid> GetCompanyByDomain(string domainName, CancellationToken ct)
        {
            var response = await _client.GetCompanyByDomainAsync(
                new GetCompanyByDomainRequest { Domain = domainName }, cancellationToken: ct);
            
            if(!response.Exists || !response.IsActive)
                throw new UnauthorizedAccessException("The company associated with this email is either inactive or does not exist.");
            
            return Guid.Parse(response.CompanyId);
        }

        public async Task<bool> RollbackCompany(Guid companyId, CancellationToken ct = default)
        {
            var response = await _client.RollbackCompanyAsync(new RollbackCompanyRequest{ CompanyId = companyId.ToString()}, cancellationToken : ct);
            if(!response.IsSuccess)
                throw new Exception(response.Message);

            return response.IsSuccess;
        }
    }
}