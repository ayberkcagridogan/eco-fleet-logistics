using EcoFleet.Identity.Domain.Constants;

namespace EcoFleet.Identity.Api.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddIdentityAuthorizationPolicies(this IServiceCollection services)
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