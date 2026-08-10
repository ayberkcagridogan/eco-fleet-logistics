
using EcoFleet.Shipment.Application.Common.Persistence;
using EcoFleet.Shipment.Infrastructure.Persistence;
using EcoFleet.Shipment.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcoFleet.Shipment.Infrastructure;
public static class DependencyInjection {
    public static IServiceCollection AddShipmentInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShipmentDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ShipmentDbContext).Assembly.FullName)
            );
        });        
        services.AddScoped<IShipmentRepo, ShipmentRepo>();
       
        return services;
    }
}