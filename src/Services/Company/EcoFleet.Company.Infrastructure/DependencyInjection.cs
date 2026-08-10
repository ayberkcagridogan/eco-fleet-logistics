using EcoFleet.Company.Application.Common.Persistence;
using EcoFleet.Company.Infrastructure.Persistence;
using EcoFleet.Company.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcoFleet.Company.Infrastructure;
public static class DependencyInjection {

    public static IServiceCollection AddCompanyInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
   
    
       services.AddDbContext<CompanyDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(CompanyDbContext).Assembly.FullName)
            );
        });  

        services.AddScoped<ICompanyRepo, CompanyRepo>();

        return services;
    }
}