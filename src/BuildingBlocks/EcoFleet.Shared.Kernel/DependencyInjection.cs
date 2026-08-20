using EcoFleet.Shared.Kernel.Authentication;
using EcoFleet.Shared.Kernel.Behaviors;
using EcoFleet.Shared.Kernel.Logging;
using EcoFleet.Shared.Kernel.Middlewares;
using EcoFleet.Shared.Kernel.Services;
using EcoFleet.Shared.Kernel.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EcoFleet.Shared.Kernel
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedKernel(this IServiceCollection services,IHostApplicationBuilder builder)
        {
            builder.AddServiceDefaults();

            if (builder is WebApplicationBuilder webBuilder)
            {
                webBuilder.Host.UseCustomSerilog();
            }
            services.AddHttpContextAccessor();
            services.AddGrpc();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddCustomJwtAuthentication(builder.Configuration);

            return services;
        }

        public static IApplicationBuilder UseSharedKernelMiddlewares(this IApplicationBuilder app)
        {      
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseExceptionHandler();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<SecurityAuditMiddleware>();
           
            return app;
        }

        public static WebApplication UseSharedKernelEndpoints(this WebApplication app)
        {
            app.MapDefaultEndpoints();
            return app;
        }
    }
}