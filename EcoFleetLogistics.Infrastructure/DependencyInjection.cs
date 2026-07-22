namespace EcoFleetLogistics.Infrastructure
{
    using EcoFleetLogistics.Application.Common.Interfaces;
    using EcoFleetLogistics.Infrastructure.Persistence;
    using EcoFleetLogistics.Infrastructure.Persistence.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public static class DependencyInjection
    {
        public static IHostBuilder UseCustemSerilog(this IHostBuilder host)
        {
            return host.UseSerilog((context, services, configuration) =>
            {
                var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "logs");
               configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProperty("Application", "EcoFleet.Api")
                    .Enrich.WithProperty("Enviroment", context.HostingEnvironment.EnvironmentName)
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: Path.Combine(logFolder, "ecofleet-.txt"),
                        rollingInterval:RollingInterval.Hour,
                        flushToDiskInterval: TimeSpan.FromSeconds(1))
                    .WriteTo.Seq("http://localhost:5341");
            });
        }
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