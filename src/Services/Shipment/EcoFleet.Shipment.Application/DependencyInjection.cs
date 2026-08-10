using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EcoFleet.Shipment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddShipmentApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}