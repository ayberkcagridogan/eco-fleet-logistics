
using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using EcoFleet.Shipment.Application.Common.Persistence;
using EcoFleet.Shipment.Infrastructure.Persistence;
using EcoFleet.Shipment.Infrastructure.Persistence.Repositories;
using EcoFleetLogistics.Infrastructure.Persistence;
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
        services.AddScoped<IUnitOfWork, UnitOfWork<ShipmentDbContext>>();
       
        return services;
    }
}