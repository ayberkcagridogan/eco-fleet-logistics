using EcoFleet.Shared.Kernel.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EcoFleet.Shipment.Infrastructure.Persistence
{
    public class ShipmentDbContextFactory : IDesignTimeDbContextFactory<ShipmentDbContext>
    {
        public ShipmentDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../EcoFleet.Shipment.Api");

             var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            var optionsBuilder = new DbContextOptionsBuilder<ShipmentDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new ShipmentDbContext(optionsBuilder.Options, new DesignTimeCurrentUserService());
        }

        private class DesignTimeCurrentUserService : ICurrentUserService
        {
            public Guid? UserId => Guid.Empty;
            public Guid? TenantId => null;
            public string? UserEmail => "system@ecofleet.local";
            public string? Role => "System";
            public bool IsAuthenticated => false;
        }
    }
}