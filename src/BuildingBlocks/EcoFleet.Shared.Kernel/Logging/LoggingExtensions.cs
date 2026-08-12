
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace EcoFleet.Shared.Kernel.Logging
{
    public static class LoggingExtensions
    {
       public static void UseCustomSerilog(this ConfigureHostBuilder host)
        {   
            host.UseSerilog((context, services, configuration) =>
            {
               configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .WriteTo.Console()
                    .WriteTo.Seq(context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341");
            });
        }
    }
}
