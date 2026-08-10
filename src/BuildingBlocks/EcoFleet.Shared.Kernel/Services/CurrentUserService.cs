using System.Security.Claims;
using EcoFleet.Shared.Kernel.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EcoFleet.Shared.Kernel.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
        public Guid? UserId
        {
            get
            {
                var userIdStr = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User?.FindFirst("sub")?.Value;

                return Guid.TryParse(userIdStr, out var userId) ? userId : null;
            }
        }

        public string? UserEmail => User?.FindFirst(ClaimTypes.Email)?.Value 
                             ?? User?.FindFirst("email")?.Value;

        public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value 
                        ?? User?.FindFirst("role")?.Value;

       public Guid? CompanyId
       {
            get
            {
                var companyIdStr = User?.FindFirst("CompanyId")?.Value;
                return Guid.TryParse(companyIdStr, out var companyId) ? companyId : null;
            }
        }

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public Guid? TenantId => throw new NotImplementedException();
    }
}