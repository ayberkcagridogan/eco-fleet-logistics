using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EcoFleet.Shared.Kernel.Persistence.Extensions
{
public static class DatabaseExtensions
{
        public static async Task ApplyMigrationsAndSeedAsync<TContext>(this IApplicationBuilder app)
            where TContext : DbContext
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            
            try
            {
                var context = services.GetService<TContext>();
                if (context != null)
                {
                    logger.LogInformation("Checking for pending migrations for {DbContext}...", typeof(TContext).Name);
                    
                    if ((await context.Database.GetPendingMigrationsAsync()).Any())
                    {
                        logger.LogInformation("Applying pending migrations...");
                        await context.Database.MigrateAsync();
                        logger.LogInformation("Migrations applied successfully.");
                    }
                    var initializer = services.GetService<IDbInitializer>();
                    if (initializer != null)
                    {
                        logger.LogInformation("Running Database Initializer / Seed data...");
                        await initializer.InitializeAsync();
                        logger.LogInformation("Database Initializer completed successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating or seeding the database for {DbContext}.", typeof(TContext).Name);
                throw;
            }
        }
    }
}