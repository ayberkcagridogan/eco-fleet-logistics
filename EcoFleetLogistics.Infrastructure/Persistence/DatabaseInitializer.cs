using EcoFleetLogistics.Application.Common.Authentication.Interfaces;
using EcoFleetLogistics.Domain.Companies;
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
                
                var (adminCompanyId, adminUserId) = await SeedUsersDataAsync(context, logger, passHasher);

                if (adminCompanyId.HasValue && adminUserId.HasValue)
                {
                    await SeedShipmentsDataAsync(context, logger, adminCompanyId.Value, adminUserId.Value);
                }
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "A critical error occurred while creating the database or applying the migration.");
            throw; 
        }
    }

    private static async Task SeedShipmentsDataAsync(AppDbContext context, ILogger<AppDbContext> logger, Guid companyId, Guid createdById)
    {
        if (!await context.Shipments.AnyAsync())
        {
            logger.LogInformation("The Shipment table is empty; sample data is being added...");

            var shipment1 = Shipment.Create(
                senderName: "Amazon",
                receiverName: "Hans Zimmer",
                destinationAddress: "Logistics Str 1, Stuttgart, Germany",
                weight: 10,
                companyId: companyId,
                createdById: createdById
            );

            var shipment2 = Shipment.Create(
                senderName: "Zalando",
                receiverName: "Gerd Muller",
                destinationAddress: "Daimler Str 1, Stuttgart, Germany",
                weight: 15,
                companyId: companyId,
                createdById: createdById
            );

            context.Shipments.AddRange(shipment1, shipment2);
           
            await context.SaveChangesAsync();
            logger.LogInformation("Seed data has been successfully saved.");
        }
        else
        {
            logger.LogInformation("Shipment data already exists in the database; the seeding step was skipped.");
        }
    }
    private static async Task<(Guid? CompanyId, Guid? UserId)> SeedUsersDataAsync(AppDbContext context, ILogger<AppDbContext> logger, IPasswordHasher passwordHasher)
    {
        if (!await context.Users.AnyAsync())
        {
            logger.LogInformation("User not found in the database. Creating default SuperAdmin...");

            var adminCompany = Company.Create(
                name: "EcoFleet Headquarters",
                taxNumber: "9999999999" // Varsa diğer zorunlu alanlar
            );

            await context.Companies.AddAsync(adminCompany);

            var adminUser = User.Create(
                firstName: "System",
                lastName: "Admin",
                email: "admin@ecofleet.com",
                passwordHash: passwordHasher.HashPassword("Admin123!"), 
                role: UserRole.Admin,
                companyId: adminCompany.Id
            );

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
            logger.LogInformation("The default SuperAdmin account was successfully created: admin@ecofleet.com");

            return (adminCompany.Id, adminUser.Id);
        }
        else
        {
            logger.LogInformation("User data already exists in the database; the seeding step was skipped.");

            var existingUser = await context.Users.AsNoTracking().FirstOrDefaultAsync();
            return (existingUser?.CompanyId, existingUser?.Id);
        }
    }
}