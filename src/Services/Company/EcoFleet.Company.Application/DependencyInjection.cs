
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EcoFleet.Company.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCompanyApplication(this IServiceCollection services)
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