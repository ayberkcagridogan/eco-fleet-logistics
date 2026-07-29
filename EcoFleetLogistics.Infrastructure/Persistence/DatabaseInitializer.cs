using EcoFleetLogistics.Application.Common.Authentication.Interfaces;
using EcoFleetLogistics.Domain.Shipments;
using EcoFleetLogistics.Domain.Users;
using EcoFleetLogistics.Domain.Users.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EcoFleetLogistics.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task MigrateDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateAsyncScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<AppDbContext>>();
        var env = services.GetRequiredService<IHostEnvironment>();
        
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            logger.LogInformation("Database connection is being checked and migrations are being applied...");

            if(context.Database.IsRelational())
                await context.Database.MigrateAsync();

            logger.LogInformation("Database migration operations have been successfully completed.");

            if (env.IsDevelopment())
            {
                logger.LogInformation("Development environment detected. Checking seed data...");
                var passHasher = services.GetRequiredService<IPasswordHasher>();
                await SeedDataAsync(context, logger, passHasher);
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "A critical error occurred while creating the database or applying the migration.");
            throw; 
        }
    }

    private static async Task SeedDataAsync(AppDbContext context, ILogger<AppDbContext> logger, IPasswordHasher passwordHasher)
    {
        if (!await context.Shipments.AnyAsync())
        {
            logger.LogInformation("The Shipment table is empty; sample data is being added...");

            context.Shipments.AddRange(
                Shipment.Create( 
                    "TRK-1001", 
                    "Amazon", 
                    "Hans Zimmer",
                    "Logistics Str 1, Stuttgart, Germany",
                    10
                ),
                Shipment.Create( 
                    "TRK-1002", 
                    "Zalando", 
                    "Gerd Muller",
                    "Daimler Str 1, Stuttgart, Germany",
                    10
                )
            );

            if (!await context.Users.AnyAsync())
            {
                logger.LogInformation("User not found in the database. Creating default SuperAdmin...");

                var adminUser = User.Create(
                    firstName: "System",
                    lastName: "Admin",
                    email: "admin@ecofleet.com",
                    passwordHash: passwordHasher.HashPassword("Admin123!"), 
                    role: UserRole.Admin,
                    companyId: Guid.NewGuid()
                );

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();

                logger.LogInformation("The default SuperAdmin account was successfully created: admin@ecofleet.com");
            }

            await context.SaveChangesAsync();
            logger.LogInformation("Seed data has been successfully saved.");
        }
        else
        {
            logger.LogInformation("Data already exists in the database; the seeding step was skipped.");
        }
    }
}