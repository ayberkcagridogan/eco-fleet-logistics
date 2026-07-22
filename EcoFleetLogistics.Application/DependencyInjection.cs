using System.Reflection;
using EcoFleetLogistics.Application.Common.Behaviors;
using EcoFleetLogistics.Application.Shipments.Commands.CreateShipment;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EcoFleetLogistics.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));   
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}