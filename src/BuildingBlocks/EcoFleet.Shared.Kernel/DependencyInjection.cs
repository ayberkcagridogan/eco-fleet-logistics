using EcoFleet.Shared.Kernel.Authentication;
using EcoFleet.Shared.Kernel.Behaviors;
using EcoFleet.Shared.Kernel.Middlewares;
using EcoFleet.Shared.Kernel.Services;
using EcoFleet.Shared.Kernel.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcoFleet.Shared.Kernel
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedKernel(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddCustomJwtAuthentication(configuration);

            return services;
        }

        public static IApplicationBuilder UseSharedKernelMiddlewares(this IApplicationBuilder app)
        {      
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseExceptionHandler();
            app.UseAuthentication();

            app.UseMiddleware<SecurityAuditMiddleware>();

            return app;
        }
    }
}