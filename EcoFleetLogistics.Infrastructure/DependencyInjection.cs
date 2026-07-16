namespace EcoFleetLogistics.Infrastructure
{
    using EcoFleetLogistics.Application.Common.Interfaces;
    using EcoFleetLogistics.Infrastructure.Persistence;
    using EcoFleetLogistics.Infrastructure.Persistence.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("EcoFleetLogistics.Infrastructure")));
                
            services.AddScoped<IShipmentRepo, ShipmentRepo>();

            return services;
        }
    }
}