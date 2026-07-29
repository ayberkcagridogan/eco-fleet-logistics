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
                opt.AddPolicy(Policies.RequireAdmin, policy => 
                policy.RequireRole(Roles.Admin));

                opt.AddPolicy(Policies.RequireFleetManager, policy => 
                policy.RequireRole(Roles.FleetManager));

                opt.AddPolicy(Policies.RequireDriver, policy => 
                policy.RequireRole(Roles.Driver));

                opt.AddPolicy(Policies.ManagementOnly, policy => 
                policy.RequireRole(Roles.Admin, Roles.FleetManager));
            });

            return services;
        }
    }
}