using EcoFleet.Company.Application.Common.Persistence;
using EcoFleet.Company.Infrastructure.Persistence;
using EcoFleet.Company.Infrastructure.Persistence.Repositories;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using EcoFleetLogistics.Infrastructure.Persistence;
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
                configuration.GetConnectionString("company-db"),
                b => b.MigrationsAssembly(typeof(CompanyDbContext).Assembly.FullName)
            );
        });  

        services.AddScoped<ICompanyRepo, CompanyRepo>();
        services.AddScoped<IUnitOfWork, UnitOfWork<CompanyDbContext>>();

        return services;
    }
}