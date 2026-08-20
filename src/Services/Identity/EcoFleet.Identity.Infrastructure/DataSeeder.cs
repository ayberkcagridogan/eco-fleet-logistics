using EcoFleet.Identity.Application.Common.Authentication.Interfaces;
using EcoFleet.Identity.Application.Common.Interfaces.Services;
using EcoFleet.Identity.Domain.Users;
using EcoFleet.Identity.Domain.Users.Enums;
using EcoFleet.Identity.Infrastructure.Persistence;
using EcoFleet.Shared.Kernel.Persistence.Extensions;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EcoFleet.Identity.Infrastructure
{
    public static class DataSeeder
    {
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<IdentityDbContext>>();
            var env = services.GetRequiredService<IHostEnvironment>();
            Guid? adminCompanyId = null; 
            var companyGrpcClient = services.GetRequiredService<ICompanyGrpcClient>();
            try
            {
                var context = services.GetRequiredService<IdentityDbContext>();
                var passHasher = services.GetRequiredService<IPasswordHasher>();
                
                if (!await context.Users.IgnoreTenantFilterIf(true).AnyAsync(ct))
                {
                    logger.LogInformation("User not found in the database. Creating default SuperAdmin...");

                    adminCompanyId = await companyGrpcClient.RegisterSuperAdminAsync(ct);

                    var adminUser = User.Create(
                        firstName: "System",
                        lastName: "Admin",
                        email: "admin@superadmin.com",
                        passwordHash: passHasher.HashPassword("Admin123!"), 
                        tenantId: adminCompanyId.Value,
                        role: UserRole.SuperAdmin
                    );

                    await context.Users.AddAsync(adminUser, ct);
                    await context.SaveChangesAsync(ct);
                    logger.LogInformation("The default SuperAdmin account was successfully created: admin@superadmin.com");
                }
                else
                {
                    logger.LogInformation("User data already exists in the database; the seeding step was skipped.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during SuperAdmin seeding.");
                if (adminCompanyId.HasValue)
                {
                    logger.LogWarning("Rolling back company creation for CompanyId: {CompanyId}", adminCompanyId.Value);
                    await companyGrpcClient.RollbackCompany(adminCompanyId.Value, ct);
                }
                
                throw;
            }
            
        }
    }
}