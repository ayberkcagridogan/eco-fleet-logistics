
using System.Threading.Tasks;
using EcoFleet.Identity.Application.Common.Authentication.Interfaces;
using EcoFleet.Identity.Application.Common.Interfaces.Authentication;
using EcoFleet.Identity.Application.Common.Interfaces.Services;
using EcoFleet.Identity.Application.Common.Persistence;
using EcoFleet.Identity.Application.Features.Authentication.Common;
using EcoFleet.Identity.Infrastructure.Authentication;
using EcoFleet.Identity.Infrastructure.Persistence;
using EcoFleet.Identity.Infrastructure.Persistence.Repositories;
using EcoFleet.Identity.Infrastructure.Services;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using EcoFleetLogistics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcoFleet.Identity.Infrastructure;
public static class DependencyInjection {

    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("identity-db"),
                b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)
            );
        });   
        
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));    
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();   
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUnitOfWork, UnitOfWork<IdentityDbContext>>();
        services.AddScoped<ICompanyGrpcClient, CompanyGrpcClient>();
        
        return services;
    }

    public static async Task SeedIdentityDatabaseAsync(this IServiceProvider serviceProvider, CancellationToken ct= default)
    {
        await DataSeeder.SeedSuperAdminAsync(serviceProvider,ct);
    }
}