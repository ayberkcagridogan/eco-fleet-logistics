using EcoFleetLogistics.Domain.Constants;
using Microsoft.Extensions.Options;

namespace EcoFleetLogistics.Api.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(Policies.RequireSuperAdmin, policy => 
                policy.RequireRole(Roles.SuperAdmin));

                opt.AddPolicy(Policies.RequireCompanyAdmin, policy => 
                policy.RequireRole(Roles.CompanyAdmin));

                opt.AddPolicy(Policies.RequireDriver, policy => 
                policy.RequireRole(Roles.Driver));

                opt.AddPolicy(Policies.ManagementOnly, policy => 
                policy.RequireRole(Roles.SuperAdmin, Roles.CompanyAdmin));
            });

            return services;
        }
    }
}