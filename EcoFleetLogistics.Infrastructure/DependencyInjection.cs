using EcoFleetLogistics.Application.Common.Authentication.Interfaces;
using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Infrastructure.Persistence;
using EcoFleetLogistics.Infrastructure.Authentication;
using EcoFleetLogistics.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EcoFleetLogistics.Application.Authentication.Common;
using EcoFleetLogistics.Application.Common.Interfaces;
using EcoFleetLogistics.Application.Common.Interfaces.Authentication;
using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Infrastructure.Services;
using Serilog;
using EcoFleetLogistics.Infrastructure.Persistence.Interceptors;

namespace EcoFleetLogistics.Infrastructure;
public static class DependencyInjection {
    public static IHostBuilder UseCustomSerilog(this IHostBuilder host)
    {
        return host.UseSerilog((context, services, configuration) =>
        {
           configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Application", "EcoFleet.Api")
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .WriteTo.Console()
                .WriteTo.Seq(context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341");
        });
    }
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<AuditAndSoftDeleteInterceptor>();
    
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditAndSoftDeleteInterceptor>();

             options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("EcoFleetLogistics.Infrastructure"))
                .AddInterceptors(interceptor); 
        });
                
        
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
           
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
           
        services.AddScoped<IShipmentRepo, ShipmentRepo>();
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUnityOfWork, UnityOfWork>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        return services;
    }
}